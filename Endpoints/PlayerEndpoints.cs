public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder routes)
    {
        var playerApi = routes.MapGroup("/players").WithTags("Players");
        playerApi.MapPost("/", async (IPlayerService service, CreatePlayerBody payload) =>
        {
            try
            {
                CreatePlayerDTO result = await service.CreatePlayerAsync(payload);
                return TypedResults.Created($"/players/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return (IResult)TypedResults.BadRequest(ex.Message);
            }
        })
            .WithName("CreatePlayer")
            .WithDescription("Create player on specific board.");
    }
}