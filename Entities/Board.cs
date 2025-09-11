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
    public int? LastDieValue { get; set; }

    private Board()
    {
        Turn = new Random().Next(1, NumOfPlayers + 1);
        State = BoardState.Waiting;
        LastDieValue = null;
    }

    public string GetStateString()
    {
        return State switch
        {
            BoardState.Waiting => "waiting",
            BoardState.Roll => "roll",
            BoardState.Move => "move",
            _ => "unknown",
        };
    }

    public int TurnNext()
    {
        Turn = (Turn % NumOfPlayers) + 1;
        return Turn;
    }
    public void EndTurn()
    {
        TurnNext();
        State = BoardState.Roll;
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