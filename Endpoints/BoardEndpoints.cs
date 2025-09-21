using LudoAPI.DTOs;
using LudoAPI.Services;
using Microsoft.AspNetCore.Http;
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
        }).WithName("ListBoards")
            .WithSummary("List boards")
            .WithDescription("List all existing boards");

        boardApi.MapPost("/", async Task<Results<Created<CreateBoardDTO>, BadRequest<string>>> (IBoardService service, CreateBoardBody body) =>
        {
            try
            {
                var board = await service.CreateBoardAsync(body);
                return TypedResults.Created($"/boards/{board.Id}", board);
            }
            catch (Exception e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }).WithName("CreateBoard")
            .WithSummary("Create boards")
            .WithDescription("Create new board");

        boardApi.MapGet("/{id}", async Task<Results<Ok<BoardDTO>, NotFound<string>>> (IBoardService service, Guid id) =>
        {
            var board = await service.GetBoardAsync(id);
            if (board == null) return TypedResults.NotFound("Board not found.");
            return TypedResults.Ok(board);
        }).WithName("GetBoard")
            .WithSummary("Get board")
            .WithDescription("Get board by id");

        boardApi.MapDelete("/{id}",
            async Task<Results<Ok<DeleteBoardDTO>, BadRequest<string>>>
            (
                IBoardService service,
                IPegService pegService,
                IPlayerService playerService,
                Guid id,
                [FromBody] DeleteBoardBody body
            ) =>
        {
            try
            {
                var board = await service.DeleteBoardAsync(id, body.Secret);
                var pegs = await pegService.DeleteBoardPegsAsync(id);
                var players = await playerService.DeleteBoardPlayersAsync(id);
                return TypedResults.Ok(
                    new DeleteBoardDTO(
                        board.Id, board.NumOfPlayers, board.State, board.Turn, board.LastDieValue, board.CreatedAt, players, pegs
                    )
                );
            }
            catch (Exception e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }).WithName("DeleteBoard")
            .WithSummary("Delete board")
            .WithDescription("Delete a board by id. Use 'secret' at the body.");

        boardApi.MapGet("/{id}/view",
            async Task<Results<ContentHttpResult, NotFound<string>>>
            (IBoardService service, Guid id) =>
        {
            var result = await service.GetBoardViewAsync(id);
            if (result is null) return TypedResults.NotFound("Board not found.");
            return TypedResults.Text(result, "text/plain");
        }).WithName("GetBoardView")
            .WithSummary("View board")
            .WithDescription("Get board's view")
            .Produces<string>(StatusCodes.Status200OK, "text/plain");

        boardApi.MapGet("/{id}/pegs",
            async Task<Results<Ok<DetailedPegDTO[]>, NotFound>>
            (IBoardService service, Guid id) =>
            {
                var pegs = await service.ListPegsOnBoardAsync(id);
                return pegs is null ? TypedResults.NotFound() : TypedResults.Ok(pegs);
            })
            .WithName("ListPegsOnBoard")
            .WithSummary("List pegs on board")
            .WithDescription("List pegs on board where the player exist.");

        boardApi.MapPut("/{id}/dice", async Task<Results<Ok<BoardDTO>, NotFound<string>, BadRequest<string>>> (IBoardService service, Guid id, [FromHeader(Name = "X-Player-Key")] Guid playerKey) =>
        {
            try
            {
                BoardDTO? board = await service.RollDice(id, playerKey);
                return board is null ? TypedResults.NotFound("Board not found.") : TypedResults.Ok(board);
            }
            catch (Exception e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }).WithName("RollDice")
            .WithSummary("Roll dice")
            .WithDescription("Roll the dice of a board");




    }
}