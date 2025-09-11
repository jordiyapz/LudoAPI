using LudoAPI.DTOs;

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

            var newPeg = _db.Pegs.Add(Peg.Create(player.Id)).Entity;
            board.State = BoardState.Roll;
            await _db.SaveChangesAsync();

            return new PegDTO(newPeg.Id, newPeg.Owner, newPeg.Position, newPeg.Order);

        }

        public async Task<PegDTO?> MovePegAsync(int pegId)
        {
            var peg = _db.Pegs.FirstOrDefault(p => p.Id == pegId);
            if (peg == null) return null;

            var player = _db.Players.Find(peg.Owner) ?? throw new Exception("Peg's owner not found");
            var board = _db.Boards.Find(player.BoardId) ?? throw new Exception("Board not found");
            if (board.State != BoardState.Move) throw new Exception("Board state is not 'Move'");
            if (board.LastDieValue == null) throw new Exception("Dice has not been rolled");

            peg.Position += (int)board.LastDieValue;
            board.State = BoardState.Roll;
            board.TurnNext();
            await _db.SaveChangesAsync();
            return new PegDTO(peg.Id, peg.Owner, peg.Position, peg.Order);
        }
    }
}
