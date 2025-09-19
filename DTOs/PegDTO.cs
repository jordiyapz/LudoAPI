namespace LudoAPI.DTOs;

public record PegDTO(int Id, int Owner, int Position, int Order);
public record DetailedPegDTO(int Id, int Owner,  int Position, int Order, char Symbol);