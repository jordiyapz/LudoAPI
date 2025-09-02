using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BoardService : IBoardService
{
  private readonly LudoDbContext _db;
  public BoardService(LudoDbContext dbContext)
  {
    _db = dbContext;
  }

  public async Task<IEnumerable<BoardDTO>> GetAllBoardsAsync()
  {
    var boards = await _db.Boards.ToListAsync();
    return boards.Select(b => new BoardDTO
    (
      b.Id, b.NumOfPlayers, b.GetStateString(), b.Turn, b.DieValue, b.Created
    ));
  }
  public async Task<CreateBoardDTO> CreateBoardAsync(CreateBoardBody body)
  {
    var result = await _db.Boards.AddAsync(Board.Create(body.NumOfPlayers));
    var board = result.Entity;
    await _db.SaveChangesAsync();
    return new CreateBoardDTO(
      board.Id, board.Secret, board.NumOfPlayers, board.GetStateString(), board.Turn, board.DieValue, board.Created);
  }
  public async Task<BoardDTO?> GetBoardAsync(Guid id)
  {
    var board = await _db.Boards.FindAsync(id);
    if (board == null) return null;
    return new BoardDTO(
      board.Id, board.NumOfPlayers, board.GetStateString(), board.Turn, board.DieValue, board.Created);
  }

  public async Task<string?> GetBoardViewAsync(Guid id)
  {
    var board = await _db.Boards.FindAsync(id);
    if (board == null) return null;
    return
@"+-----------------+--+--+--+-----------------+
|                 |  |  |  |                 |
|  +-----+-----+  +--+--+--+  +-----+-----+  |
|  |     |     |  |  |     |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|                 |  |  |  |                 |
+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+
|  |  |  |  |  |  |        |  |  |  |  |  |  |
+--+  +--+--+--+--+        +--+--+--+--+--+--+
|  |                                      |  |
+--+--+--+--+--+--+        +--+--+--+--+  +--+
|  |  |  |  |  |  |        |  |  |  |  |  |  |
+--+--+--+--+--+--+--+  +--+--+--+--+--+--+--+
|                 |  |  |  |                 |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |  |  |  |  |     |     |  |
|  +-----+-----+  +--+  +--+  +-----+-----+  |
|  |     |     |  |  |  |  |  |     |     |  |
|  |     |     |  +--+  +--+  |     |     |  |
|  |     |     |  |     |  |  |     |     |  |
|  +-----+-----+  +--+--+--+  +-----+-----+  |
|                 |  |  |  |                 |
+-----------------+--+--+--+-----------------+";
  }

  public async Task<BoardDTO?> RollDice(Guid boardId, Guid playerKey)
  {
    var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
    if (board == null) return null;

    if (board.State != BoardState.Roll) throw new Exception("Board is not waiting for rolling.");

    var player = await _db.Players.FirstOrDefaultAsync(p => p.BoardId == boardId && p.Key == playerKey);
    if (player == null) throw new Exception("Player not found.");

    if (player.Order != board.Turn) throw new Exception("Not your turn.");

    board.State = BoardState.Move;
    var number = new Random().Next(1, 7);
    board.DieValue = number;
    await _db.SaveChangesAsync();

    return new BoardDTO(
      board.Id, board.NumOfPlayers, board.GetStateString(), board.Turn, board.DieValue, board.Created);
  }
}