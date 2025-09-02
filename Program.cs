using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LudoDbContext>(opt => opt.UseInMemoryDatabase("BoardList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<IBoardService, BoardService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapBoardEndpoints(); 
app.MapPlayerEndpoints();
app.Run();
