public interface IPlayerService
{
    public Task<CreatePlayerDTO> CreatePlayerAsync(CreatePlayerBody payload);
    public Task<PlayerDTO> ValidatePlayerKey(Guid playerKey, Guid boardId);
}