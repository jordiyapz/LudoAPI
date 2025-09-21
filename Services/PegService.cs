using LudoAPI.DTOs;
using LudoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LudoAPI.Services
{
    public class PegService(LudoDbContext dbContext) : IPegService
    {
        private readonly LudoDbContext _db = dbContext;

        public async Task<PegDTO> CreatePegAsync(Guid playerKey)
        {
            var player = _db.Players.FirstOrDefault(p => p.Key == playerKey) ?? throw new Exception("Wrong player key");
            var board = _db.Boards.FirstOrDefault(b => b.Id == player.BoardId) ?? throw new Exception("Board does not exist");

            if (board.Turn != player.Order) throw new Exception("Not your turn");
            if (board.State != BoardState.Move) throw new Exception("Board state is not 'Move'");
            if (board.LastDieValue != 6) throw new Exception("Dice value must be 6 to spawn a peg");

            var pegs = _db.Pegs.Where(p => p.Owner == player.Id).ToList();
            if (pegs.Count >= 4) throw new Exception("Pegs is full");

            var newPeg = _db.Pegs.Add(Peg.Create(player.Id, pegs.Count)).Entity;
            board.State = BoardState.Roll;
            await _db.SaveChangesAsync();

            return new PegDTO(newPeg.Id, newPeg.Owner, newPeg.Position, newPeg.Order);
        }

        public async Task<List<Peg>> GetBoardPegsFlat(Guid boardId)
        {
            var players = await _db.Players.Where(p => p.BoardId == boardId).ToArrayAsync();
            var tasks = players.Select(pl => GetPlayersPeg(pl.Id));
            var pegs = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
            return pegs ?? [];
        }

        public async Task<List<Peg>> GetPlayersPeg(int playerId)
        {
            var pegs = await _db.Pegs.Where(p => p.Owner == playerId).ToListAsync();
            return pegs ?? [];
        }

        public async Task<Dictionary<int, XYCoord>> GetPegsCoord(Guid boardId)
        {
            var players = await _db.Players.Where(p => p.BoardId == boardId).ToArrayAsync() ?? [];
            var pegs = await GetBoardPegsFlat(boardId);
            Dictionary<int, XYCoord> pegCoords = [];
            XYCoord c;
            foreach (Peg _peg in pegs)
            {
                Player _player = players.First(pl => pl.Id == _peg.Owner);
                c = BoardView.CalcCoord(_peg.Position);
                c = BoardView.RotateCoord(c, _player.Quadrant);
                pegCoords.Add(_peg.Id, c);
            }
            return pegCoords;
        }

        public async Task<bool> IsPlayerWinning(int playerId)
        {
            var pegs = await GetPlayersPeg(playerId);
            if (pegs == null) return false;
            foreach (var peg in pegs)
                if (peg.Position != 56) return false;
            return true;
        }

        public async Task<PegDTO?> MovePegAsync(int pegOrder, Guid playerKey)
        {
            var player = _db.Players.FirstOrDefault(p => p.Key == playerKey) ?? throw new Exception("Invalid player key");
            var peg = _db.Pegs.FirstOrDefault(p => p.Order == pegOrder && p.Owner == player.Id);
            if (peg == null) return null;
            if (!peg.Movable) throw new Exception("This peg is not movable");

            var board = _db.Boards.Find(player.BoardId) ?? throw new Exception("Board not found");
            if (board.State != BoardState.Move) throw new Exception("Board state is not 'Move'");
            if (board.Turn != player.Order) throw new Exception("Not your turn");
            if (board.LastDieValue == null) throw new Exception("Dice has not been rolled");

            var newPosition = peg.Position + (int)board.LastDieValue;
            XYCoord c = BoardView.CalcCoord(newPosition);
            c = BoardView.RotateCoord(c, player.Quadrant);

            var pegs = await GetBoardPegsFlat(board.Id);
            var pegCoords = await GetPegsCoord(board.Id);
            pegCoords.Remove(peg.Id);

            List<int> enemyStack = [];
            foreach (var pair in pegCoords)
            {
                if (pair.Value == c)
                {
                    var targetted = pegs.First(p => p.Id == pair.Key);
                    if (targetted.Owner == peg.Owner) break;
                    enemyStack.Add(pair.Key);
                }
            }

            if (enemyStack.Count == 1)
            {
                // Delete enemy peg
                _db.Pegs.Where(p => p.Id == enemyStack.First()).ExecuteDelete();
                _db.SaveChanges();
            }

            if (enemyStack.Count <= 1 || newPosition <= 56)
            {
                peg.Position = newPosition;
                if (newPosition == 56) peg.Movable = false;
            }
            await _db.SaveChangesAsync();

            if (await IsPlayerWinning(player.Id))
                board.State = BoardState.GameOver;
            else
            {
                board.State = BoardState.Roll;
                if (board.LastDieValue != 6)
                    board.TurnNext();
            }
            await _db.SaveChangesAsync();

            return new PegDTO(peg.Id, peg.Owner, peg.Position, peg.Order);
        }
        public async Task<PegDTO[]> DeleteBoardPegsAsync(Guid boardId)
        {
            var pegs = await GetBoardPegsFlat(boardId);
            var results = pegs.Select(p => new PegDTO(p.Id, p.Owner, p.Position, p.Order));
            var pegIdList = pegs.Select(p => p.Id).ToArray();
            await _db.Pegs.Where(p => pegIdList.Contains(p.Id)).ExecuteDeleteAsync();
            return [.. results];
        }
    }
}
