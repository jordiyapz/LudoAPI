using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using LudoAPI.Services;
using LudoAPI.Endpoints;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

var AllowedOrigins = builder.Configuration["AllowedOrigins"];
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy => { policy.WithOrigins(AllowedOrigins ?? "*"); });
});

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(conn);
builder.Services.AddDbContext<LudoDbContext>(opt => opt.UseSqlite(conn));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<IBoardService, LudoAPI.Services.BoardService>();
builder.Services.AddTransient<IPlayerService, LudoAPI.Services.PlayerService>();
builder.Services.AddTransient<IPegService, LudoAPI.Services.PegService>();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LudoDbContext>();
    context.Database.Migrate(); // This creates the database if it doesn't exist
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
app.MapOpenApi();
app.MapScalarApiReference("/docs",
    options =>
    {
        options.WithTitle("Ludo API Documentation");
        options.WithDotNetFlag(true); options.AddPreferredSecuritySchemes("ApiKey");
        options.AddApiKeyAuthentication("ApiKey", apiKey =>
        {
            apiKey.Name = "X-Player-K";
            apiKey.Value = "sk-demo-key-12345";
        });
        options.WithPersistentAuthentication();
        options.Favicon = "/favicon.svg";
    });

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.MapBoardEndpoints();
app.MapPlayerEndpoints();
app.MapPegEndpoints();
app.MapGet("/", () => Results.Redirect("/docs")).ExcludeFromDescription();
app.MapStaticAssets();
app.Run();