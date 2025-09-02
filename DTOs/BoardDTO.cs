public record BoardDTO(Guid Id, int NumOfPlayers, string? State, int Turn, int? DieValue, DateTimeOffset CreatedAt);
public record CreateBoardBody(int NumOfPlayers);
public record CreateBoardDTO(Guid Id, Guid Secret, int NumOfPlayers, string? State, int Turn, int? DieValue, DateTimeOffset CreatedAt);