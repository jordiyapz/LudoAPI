using Microsoft.AspNetCore.Http.HttpResults;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder routes)
    {
        var playerApi = routes.MapGroup("/players").WithTags("Players");
        playerApi.MapPost("/",
            async Task<Results<Created<CreatePlayerDTO>, BadRequest<string>>>
            (IPlayerService service, CreatePlayerBody payload) =>
        {
            try
            {
                CreatePlayerDTO result = await service.CreatePlayerAsync(payload);
                return TypedResults.Created($"/players/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }).WithName("CreatePlayer")
            .WithSummary("Create player")
            .WithDescription("Create player on specific board.");
    }
}