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

        pegApi.MapPut("/{order}/position", async (
            IPegService service,
            int order, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
        {
            try
            {
                var res = await service.MovePegAsync(order, playerKey);
                if (res == null) return (IResult)TypedResults.NotFound("Peg not found");
                return TypedResults.Ok(res);
            }
            catch (Exception ex)
            {
                return (IResult)TypedResults.BadRequest(ex.Message);
            }
        });
    }
}

