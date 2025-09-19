using LudoAPI.DTOs;
using LudoAPI.Entities;

namespace LudoAPI.Services;

public interface IPegService
{
    public Task<DetailedPegDTO[]> ListPegAsync(Guid playerKey);
    public Task<PegDTO> CreatePegAsync(Guid playerKey);
    public Task<Dictionary<int, XYCoord>> GetPegsCoord(Guid boardId);
    public Task<PegDTO?> MovePegAsync(int pegOrder, Guid playerKey);
}

