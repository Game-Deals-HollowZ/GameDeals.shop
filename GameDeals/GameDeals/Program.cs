using GameDeals.Components;
using GameDeals.Services;
using GameDeals.Shared.Data;
using GameDeals.Shared.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Definiere die erlaubte Client-Origin (z.B. Blazor WebAssembly Client-Port)
var clientOrigin = "https://localhost:7142";

builder.Services.AddMudServices();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


builder.Services.AddHttpClient(); // HttpClientFactory verfügbar machen

builder.Services.AddHostedService<DailyRefreshService>();

builder.Services.AddScoped<TwitchTokenService>();
builder.Services.AddScoped<IGDBService, IGDBServerService>();
builder.Services.AddDbContext<AppDbContext>();


// Für IsThereAnyDealServer wird HttpClient injiziert via IHttpClientFactory, daher:
// Du kannst optional einen benannten Client definieren, hier einfach Standard:
builder.Services.AddHttpClient<IsThereAnyDealServer>();
builder.Services.AddScoped<IsThereAnyDealService, IsThereAnyDealServer>();

// CORS konfigurieren
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://localhost:7142", // Client HTTPS
            "http://localhost:7142"   // Optional: Client HTTP, falls verwendet
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Razor Components and WASM hosting
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(); // CORS Middleware aktivieren
app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GameDeals.Client._Imports).Assembly);

app.MapGet("/api/games", async (IGDBService igdb) =>
{
    var games = await igdb.GetGamesAsync();
    return Results.Ok(games);
});

app.MapGet("/api/games/search", async (string query, IGDBService igdb) =>
{
    if (string.IsNullOrWhiteSpace(query))
        return Results.BadRequest("Query darf nicht leer sein.");

    var games = await igdb.SearchGamesAsync(query);
    return Results.Ok(games);
});

app.MapGet("/api/games/paged", async (IGDBService igdb, int offset = 0, int limit = 12) =>
{
    return await igdb.GetGamesPagedAsync(offset, limit);
});

app.MapGet("/api/games/searchpaged", async (string query, IGDBService igdb, int offset = 0, int limit = 12) =>
{
    return await igdb.SearchGamesPagedAsync(query, offset, limit);
});


app.Run();
