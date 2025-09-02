public record CreatePlayerBody(Guid BoardId, char? Symbol);
public record CreatePlayerDTO(int Id, Guid Key, Guid BoardId, char Symbol, int Order);

public record PlayerDTO(int Id, Guid BoardId, PegSymbol Symbol, int Order);