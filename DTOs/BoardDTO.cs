public record BoardDTO(Guid Id, int NumOfPlayers, string? State, int Turn, int? LastDieValue, DateTimeOffset CreatedAt);
public record CreateBoardBody(int NumOfPlayers);
public record CreateBoardDTO(Guid Id, Guid Secret, int NumOfPlayers, string? State, int Turn, int? LastDieValue, DateTimeOffset CreatedAt);
public record DeleteBoardBody(Guid Secret);