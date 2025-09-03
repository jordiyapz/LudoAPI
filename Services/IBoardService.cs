public interface IBoardService
{
  public Task<IEnumerable<BoardDTO>> GetAllBoardsAsync();
  public Task<CreateBoardDTO> CreateBoardAsync(CreateBoardBody body);
  public Task<BoardDTO?> GetBoardAsync(Guid id);
  public Task<string?> GetBoardViewAsync(Guid id);
  public Task<BoardDTO?> RollDice(Guid boardId, Guid playerKey, int? seed=null);
}