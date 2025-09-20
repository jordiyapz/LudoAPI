using LudoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.HttpResults;
using LudoAPI.DTOs;

namespace LudoAPI.Endpoints;

public static class PegEndpoints
{
    public static void MapPegEndpoints(this IEndpointRouteBuilder routes)
    {
        var pegApi = routes.MapGroup("/pegs").WithTags("Pegs");

        pegApi.MapPost("/",
            async Task<Results<Created<PegDTO>, BadRequest<string>>>
            ([FromServices] IPegService service, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
        {
            try
            {
                var newPeg = await service.CreatePegAsync(playerKey);
                return TypedResults.Created($"/pegs/{newPeg.Id}", newPeg);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
            .WithName("SpawnPeg")
            .WithSummary("Spawn a peg")
            .WithDescription("Create a new peg.");

        pegApi.MapPut("/{order}/position",
            async Task<Results<Ok<PegDTO>, NotFound<string>, BadRequest<string>>>
            (
                IPegService service,
                [Description("Peg's order id. (E.g. if you have 3 pegs, it will be 0, 1, and 2 respectively)")]
                int order,
                [FromHeader(Name = "X-Player-Key")] Guid playerKey
            ) =>
        {
            try
            {
                var res = await service.MovePegAsync(order, playerKey);
                return res is null ? TypedResults.NotFound("Peg not found") : TypedResults.Ok(res);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
            .WithName("MovePeg")
            .WithSummary("Move a peg")
            .WithDescription("Move a peg of order id to new position depending on last dice value.");
    }
}

