using LudoAPI.DTOs;

public interface IBoardService
{
    public Task<IEnumerable<BoardDTO>> GetAllBoardsAsync();
    public Task<CreateBoardDTO> CreateBoardAsync(CreateBoardBody body);
    public Task<BoardDTO?> GetBoardAsync(Guid id);
    public Task<BoardDTO> DeleteBoardAsync(Guid id, Guid boardSecret);
    public Task<string?> GetBoardViewAsync(Guid id);
    public Task<DetailedPegDTO[]?> ListPegsOnBoardAsync(Guid id);
    public Task<BoardDTO?> RollDice(Guid boardId, Guid playerKey, int? seed = null);
}