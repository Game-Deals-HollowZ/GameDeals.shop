using GameDeals.Client.Services;
using GameDeals.Shared.Data;
using GameDeals.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IGDBService, IGDBApiClient>();
builder.Services.AddScoped<IsThereAnyDealService, IsThereAnyDealClient>();
builder.Services.AddDbContext<AppDbContext>();


await builder.Build().RunAsync();
