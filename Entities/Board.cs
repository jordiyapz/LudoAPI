using System.ComponentModel.DataAnnotations;

public class Board : EntityBase
{
  [Key]
  public Guid Id { get; private init; } = Guid.NewGuid();
  public DateTimeOffset Created { get; private set; } = DateTimeOffset.UtcNow;
  // For managing board
  public Guid Secret { get; private set; } = Guid.NewGuid();
  public int NumOfPlayers { get; set; }
  public int Turn { get; set; }
  public BoardState? State { get; set; }
  public int? DieValue { get; set; }

  private Board()
  {
    Turn = new Random().Next(1, NumOfPlayers + 1);
    State = BoardState.Waiting;
    DieValue = null;
  }

  public string GetStateString()
  {
    switch (State)
    {
      case BoardState.Waiting: return "waiting";
      case BoardState.Roll: return "roll";
      case BoardState.Move: return "move";
      default: return "unknown";
    }
  }

  public static Board Create(int numOfPlayers)
  {
    if (numOfPlayers < 2 || numOfPlayers > 4) throw new ArgumentException("Number of players must be between 2 and 4.");
    return new() { NumOfPlayers = numOfPlayers };
  }
}

public enum BoardState
{
  Waiting,
  Roll,
  Move
}