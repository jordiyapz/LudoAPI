using LudoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace LudoAPI.Endpoints;

public static class PegEndpoints
{
    public static void MapPegEndpoints(this IEndpointRouteBuilder routes)
    {
        var pegApi = routes.MapGroup("/pegs").WithTags("Pegs");

        pegApi.MapGet("/", async (IPegService service, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
        {
            return await service.ListPegAsync(playerKey);
        })
            .WithName("ListPegsOnBoard")
            .WithSummary("List pegs on board")
            .WithDescription("List pegs on board where the player exist."); ;

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
        })
            .WithName("SpawnPeg")
            .WithSummary("Spawn a peg")
            .WithDescription("Create a new peg.");

        pegApi.MapPut("/{order}/position", async (
            IPegService service,
            [Description("Peg's order id. (E.g. if you have 3 pegs, it will be 0, 1, and 2 respectively)")]
            int order,
            [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
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
        })
            .WithName("MovePeg")
            .WithSummary("Move a peg")
            .WithDescription("Move a peg of order id to new position depending on last dice value.");
    }
}

