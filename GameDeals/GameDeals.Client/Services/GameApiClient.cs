using GameDeals.Shared.Models;
using GameDeals.Shared.Services;
using System.Net.Http.Json;

namespace GameDeals.Client.Services
{
    // Im Client-Projekt
    public class IGDBApiClient : IGDBService
    {
        private readonly HttpClient _http;

        public IGDBApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<IGDBGame>> GetGamesAsync()
        {
            return await _http.GetFromJsonAsync<List<IGDBGame>>("api/games");
        }

        public async Task<List<IGDBGame>> SearchGamesAsync(string query)
        {
            var url = $"api/games/search?query={Uri.EscapeDataString(query)}";
            return await _http.GetFromJsonAsync<List<IGDBGame>>(url) ?? new();
        }

    }


}
