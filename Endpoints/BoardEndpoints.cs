using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public static class BoardEndpoints
{
  public static void MapBoardEndpoints(this IEndpointRouteBuilder routes)
  {
    var boardApi = routes.MapGroup("/boards").WithTags("Boards");

    boardApi.MapGet("/", async (IBoardService service) =>
    {
      var boards = await service.GetAllBoardsAsync();
      return TypedResults.Ok(boards);
    });

    boardApi.MapPost("/", async (IBoardService service, CreateBoardBody body) =>
    {
      var board = await service.CreateBoardAsync(body);
      return TypedResults.Created($"/boards/{board.Id}", board);
    });

    boardApi.MapGet("/{id}", async (IBoardService service, Guid id) =>
    {
      var board = await service.GetBoardAsync(id);
      if (board is null) return (IResult)TypedResults.NotFound("Board not found.");
      return TypedResults.Ok(board);
    });

    boardApi.MapGet("/{id}/view", async (IBoardService service, Guid id) =>
    {
      var result = await service.GetBoardViewAsync(id);
      if (result is null) return (IResult)TypedResults.NotFound("Board not found.");
      return TypedResults.Text(result, "text/plain");
    });

    boardApi.MapPost("/{id}/roll", async (IBoardService service, Guid id, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
    {
      var board = await service.RollDice(id, playerKey);
      if (board is null) return (IResult)TypedResults.NotFound("Board not found.");
      return TypedResults.Ok(board);
    });

  }
}