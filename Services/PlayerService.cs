using Microsoft.EntityFrameworkCore;

public class PlayerService : IPlayerService
{
  private readonly LudoDbContext _db;
  public PlayerService(LudoDbContext dbContext) { _db = dbContext; }

  public async Task<CreatePlayerDTO> CreatePlayerAsync(CreatePlayerBody payload)
  {
    var board = await _db.Boards
      .AsNoTracking()
      .FirstOrDefaultAsync(b => b.Id == payload.BoardId);

    if (board == null)
    {
      throw new ArgumentException("Board does not exist.", nameof(payload.BoardId));
    }

    var players = await _db.Players
      .Where(p => p.BoardId == payload.BoardId)
      .ToListAsync();

    if (players.Count >= board.NumOfPlayers)
    {
      throw new ArgumentException(
        $"Board {payload.BoardId} is full. It already has {board.NumOfPlayers} players.",
        nameof(payload.BoardId));
    }

    if (payload.Symbol != null && !Player.IsValidSymbol(payload.Symbol.Value))
    {
      throw new ArgumentException(
        $"Symbol must be one of {string.Join(", ", Enum.GetNames<PegSymbol>())}.",
        nameof(payload.Symbol));
    }

    if (payload.Symbol != null && players.Any(p => p.Symbol == (PegSymbol)payload.Symbol))
    {
      throw new ArgumentException(
        $"Player with symbol {payload.Symbol} already exists on board.",
        nameof(payload.Symbol));
    }

    var order = players.Count + 1;
    var symbol = payload.Symbol ?? (char)('a' + order - 1);
    var newPlayer = Player.Create(payload.BoardId, Player.SymbolFromChar(symbol), order);
    var result = await _db.Players.AddAsync(newPlayer);

    if (board.NumOfPlayers == players.Count + 1) board.State = BoardState.Roll;

    await _db.SaveChangesAsync();

    return new CreatePlayerDTO(
      newPlayer.Id,
      newPlayer.Key,
      newPlayer.BoardId,
      newPlayer.CharSymbol,
      newPlayer.Order);
  }
}