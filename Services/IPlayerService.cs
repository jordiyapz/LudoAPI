public interface IPlayerService
{
  public Task<CreatePlayerDTO> CreatePlayerAsync(CreatePlayerBody payload);
}