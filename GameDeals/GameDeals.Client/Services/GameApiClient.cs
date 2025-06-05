using GameDeals.Shared.Entities;
using GameDeals.Shared.Models;
using GameDeals.Shared.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<IGDBGame>> GetGamesAsync()
        {
            var response = await _http.GetAsync("api/games");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IGDBGame>>(json, _jsonOptions) ?? new List<IGDBGame>();
        }

        public async Task<List<IGDBGame>> SearchGamesAsync(string query)
        {
            var url = $"api/games/search?query={Uri.EscapeDataString(query)}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IGDBGame>>(json, _jsonOptions) ?? new List<IGDBGame>();
        }

        public async Task<List<IGDBGame>> GetGamesPagedAsync(int offset, int limit)
        {
            var url = $"api/games/paged?offset={offset}&limit={limit}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IGDBGame>>(json, _jsonOptions) ?? new List<IGDBGame>();
        }

        public async Task<List<IGDBGame>> SearchGamesPagedAsync(string query, int offset, int limit)
        {
            var url = $"api/games/searchpaged?query={Uri.EscapeDataString(query)}&offset={offset}&limit={limit}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IGDBGame>>(json, _jsonOptions) ?? new List<IGDBGame>();
        }


        public async Task<List<IGDBGameEntity>> LoadMoreGamesFromDb(int offset, int limit)
        {
            var response = await _http.GetAsync($"api/gamesdb?offset={offset}&limit={limit}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            // Deserialisiere mit den Optionen
            var games = JsonSerializer.Deserialize<List<IGDBGameEntity>>(json, _jsonOptions);
            return games;
        }


    }


}
