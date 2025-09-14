using LudoAPI.DTOs;
using LudoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using System.Collections.Immutable;

namespace LudoAPI.Services;

public class BoardService(LudoDbContext dbContext) : IBoardService
{
    private readonly LudoDbContext _db = dbContext;
    public async Task<IEnumerable<BoardDTO>> GetAllBoardsAsync()
    {
        var boards = await _db.Boards.ToListAsync();
        return boards.Select(b => new BoardDTO
        (
          b.Id, b.NumOfPlayers, b.GetStateString(), b.Turn, b.LastDieValue, b.Created
        ));
    }
    public async Task<CreateBoardDTO> CreateBoardAsync(CreateBoardBody body)
    {
        var result = await _db.Boards.AddAsync(Board.Create(body.NumOfPlayers));
        var board = result.Entity;
        await _db.SaveChangesAsync();
        return new CreateBoardDTO(
          board.Id, board.Secret, board.NumOfPlayers, board.GetStateString(), board.Turn, board.LastDieValue, board.Created);
    }
    public async Task<BoardDTO?> GetBoardAsync(Guid id)
    {
        var board = await _db.Boards.FindAsync(id);
        if (board == null) return null;
        return new BoardDTO(
          board.Id, board.NumOfPlayers, board.GetStateString(), board.Turn, board.LastDieValue, board.Created);
    }

    public async Task<string?> GetBoardViewAsync(Guid id)
    {
        var board = await _db.Boards.FindAsync(id);
        if (board == null) return null;
        var players = await _db.Players.Where(p => p.BoardId == board.Id).ToArrayAsync() ?? [];
        var tasks = players.Select(i => _db.Pegs.Where(p => p.Owner == i.Id).ToArrayAsync());
        var pegs = (await Task.WhenAll(tasks)).SelectMany(x => x).ToArray() ?? [];
        return new BoardView(board, players, pegs).render();
    }

    public async Task<BoardDTO?> RollDice(Guid boardId, Guid playerKey, int? seed = null)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null) return null;
        if (board.State != BoardState.Roll) throw new Exception("Board is not waiting for rolling.");

        var player = await _db.Players.AsNoTracking().FirstOrDefaultAsync(p => p.BoardId == boardId && p.Key == playerKey)
            ?? throw new Exception("Player not found.");
        if (player.Order != board.Turn) throw new Exception("Not your turn.");

        var pegs = await _db.Pegs.AsNoTracking().Where(p => p.Owner == player.Id).ToListAsync();
        var newDiceNumber = (seed != null ? new Random((int)seed) : new Random()).Next(1, 7);

        board.LastDieValue = newDiceNumber;

        if (board.LastDieValue == 6) board.LastConsecutiveSixes++;
        else board.LastConsecutiveSixes = 0;

        if (board.LastConsecutiveSixes == 3)
        {
            board.LastConsecutiveSixes = 0;
            board.TurnNext();
        }
        else if (pegs.Count == 0 && newDiceNumber != 6) board.TurnNext();
        else board.State = BoardState.Move;

        await _db.SaveChangesAsync();

        return new BoardDTO(
          board.Id, board.NumOfPlayers, board.GetStateString(), board.Turn, board.LastDieValue, board.Created);
    }
}