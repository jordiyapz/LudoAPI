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

        public async Task<PegDTO?> MovePegAsync(int pegOrder, Guid playerKey)
        {
            var player = _db.Players.FirstOrDefault(p => p.Key == playerKey) ?? throw new Exception("Invalid player key");
            var peg = _db.Pegs.FirstOrDefault(p => p.Order == pegOrder && p.Owner == player.Id);
            if (peg == null) return null;

            var board = _db.Boards.Find(player.BoardId) ?? throw new Exception("Board not found");
            if (board.State != BoardState.Move) throw new Exception("Board state is not 'Move'");
            if (board.Turn != player.Order) throw new Exception("Not your turn");
            if (board.LastDieValue == null) throw new Exception("Dice has not been rolled");


            var players = await _db.Players.Where(p => p.BoardId == board.Id).ToArrayAsync() ?? [];
            var tasks = players.Select(i => _db.Pegs.Where(p => p.Owner == i.Id).ToArrayAsync());
            var pegs = (await Task.WhenAll(tasks)).SelectMany(x => x).ToArray() ?? [];

            Dictionary<int, XYCoord> pegCoords = [];
            XYCoord c;
            foreach (Peg _peg in pegs)
            {
                if (_peg.Id == peg.Id) continue;
                Player _player = players.First(pl => pl.Id == _peg.Owner);
                c = BoardView.CalcCoord(_peg.Position);
                c = BoardView.RotateCoord(c, _player.Quadrant);
                pegCoords.Add(_peg.Id, c);
            }

            var newPosition = peg.Position + (int)board.LastDieValue;
            c = BoardView.CalcCoord(newPosition);
            c = BoardView.RotateCoord(c, player.Quadrant);

            List<int> enemyStack = [];
            foreach (var kvp in pegCoords)
            {
                if (kvp.Value == c)
                {
                    var targetted = pegs.First(p => p.Id == kvp.Key);
                    if (targetted.Owner == peg.Owner)
                        // Stack this peg
                        break;
                    else enemyStack.Add(kvp.Key);

                }
            }

            if (enemyStack.Count == 1)
            {
                // Delete enemy peg
                _db.Pegs.Where(p => p.Id == enemyStack.First()).ExecuteDelete();
                _db.SaveChanges();
            }

            if (enemyStack.Count <= 1 || newPosition <= 56)
                peg.Position = newPosition;

            board.State = BoardState.Roll;
            if (board.LastDieValue != 6)
                board.TurnNext();
            await _db.SaveChangesAsync();
            return new PegDTO(peg.Id, peg.Owner, peg.Position, peg.Order);
        }
    }
}
