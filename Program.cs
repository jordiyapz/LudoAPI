using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using LudoAPI.Services;
using LudoAPI.Endpoints;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy => { policy.WithOrigins("http://localhost:5173"); });
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LudoDbContext>(opt => opt.UseSqlite("Data Source=LudoSQLite.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<IBoardService, LudoAPI.Services.BoardService>();
builder.Services.AddTransient<IPlayerService, LudoAPI.Services.PlayerService>();
builder.Services.AddTransient<IPegService, LudoAPI.Services.PegService>();
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
app.UseCors(MyAllowSpecificOrigins);
app.MapBoardEndpoints();
app.MapPlayerEndpoints();
app.MapPegEndpoints();
app.Run();