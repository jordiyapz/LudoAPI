using LudoAPI.DTOs;

namespace LudoAPI.Services;

public interface IPegService
{
    public Task<PegDTO> CreatePegAsync(Guid playerKey);
    public Task<PegDTO?> MovePegAsync(int pegOrder, Guid playerKey);
}

