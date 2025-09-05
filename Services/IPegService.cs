using LudoAPI.DTOs;

namespace LudoAPI.Services;

public interface IPegService
{
    public Task<PegDTO> CreatePegAsync(Guid playerKey);
}

