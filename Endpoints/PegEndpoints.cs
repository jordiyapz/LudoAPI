using LudoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LudoAPI.Endpoints;

public static class PegEndpoints
{
    public static void MapPegEndpoints(this IEndpointRouteBuilder routes)
    {
        var pegApi = routes.MapGroup("/pegs").WithTags("Pegs");
        pegApi.MapPost("/", async ([FromServices] IPegService service, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
        {
            try
            {
                var boards = await service.CreatePegAsync(playerKey);
                return TypedResults.Ok(boards);
            }
            catch (Exception ex)
            {
                return (IResult)TypedResults.BadRequest(ex.Message);
            }
        });
    }
}

