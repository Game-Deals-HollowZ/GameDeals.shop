using GameDeals.Shared.Data;
using GameDeals.Shared.Entities;
using GameDeals.Shared.Models;
using GameDeals.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;

namespace GameDeals.Services
{
    // Im Server-Projekt
    public class IGDBServerService : IGDBService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly TwitchTokenService _tokenService;
        private readonly AppDbContext _dbContext;  // Neu: DB Context

        public IGDBServerService(HttpClient http, IConfiguration config, TwitchTokenService tokenService, AppDbContext dbContext)
        {
            _http = http;
            _config = config;
            _tokenService = tokenService;
            _dbContext = dbContext;
        }

        public async Task<List<IGDBGame>> GetGamesAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            var clientId = _config["Twitch:ClientId"];

            var games = new List<IGDBGame>();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var month = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds();

            var strictQuery = $@"
        fields name, summary, cover.url, first_release_date, total_rating, rating_count;
        where 
            first_release_date >= {month} & 
            first_release_date <= {now} & 
            (total_rating >= 70 | rating_count >= 10) & 
            cover != null;
        sort total_rating desc;
        limit 12;";

            var strictResult = await QueryIGDBAsync(clientId, token, strictQuery);
            games.AddRange(strictResult);

            if (games.Count < 12)
            {
                var looseQuery = $@"
            fields name, summary, cover.url, first_release_date, total_rating, rating_count;
            where 
                first_release_date >= {month} & 
                first_release_date <= {now} & 
                cover != null;
            sort rating_count desc;
            limit {12 - games.Count};";

                var looseResult = await QueryIGDBAsync(clientId, token, looseQuery);

                // Optionale Duplikatsfilterung
                var newGames = looseResult
                    .Where(x => !games.Any(g => g.Name == x.Name))
                    .Take(12 - games.Count);
                games.AddRange(newGames);
            }

            return games;
        }

        private async Task<List<IGDBGame>> QueryIGDBAsync(string clientId, string token, string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.igdb.com/v4/games");
            request.Headers.Add("Client-ID", clientId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(query, Encoding.UTF8, "text/plain");

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Fehlerhafte IGDB-Query:\n" + query); // optional: mit ILogger ersetzen
                throw new Exception($"IGDB Fehler: {response.StatusCode} - {errorText}");
            }


            return await response.Content.ReadFromJsonAsync<List<IGDBGame>>() ?? new();
        }

        public async Task<List<IGDBGame>> SearchGamesAsync(string query)
        {
            var token = await _tokenService.GetTokenAsync();
            var clientId = _config["Twitch:ClientId"];

            var searchQuery = $@"
    search ""{query.Replace("\"", "")}"";
    fields name, summary, cover.url, first_release_date, total_rating, rating_count, category;
    where cover != null & category = 0 & (total_rating >= 60 | rating_count >= 10);
    limit 12;";


            return await QueryIGDBAsync(clientId, token, searchQuery);
        }

        public async Task<List<IGDBGame>> GetGamesPagedAsync(int offset, int limit)
        {
            var token = await _tokenService.GetTokenAsync();
            var clientId = _config["Twitch:ClientId"];

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var month = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds();

            var query = $@"
        fields name, summary, cover.url, first_release_date, total_rating, rating_count;
        where 
            first_release_date >= {month} & 
            first_release_date <= {now} & 
            (total_rating >= 70 | rating_count >= 10) & 
            cover != null;
        sort total_rating desc;
        limit {limit};
        offset {offset};";

            return await QueryIGDBAsync(clientId, token, query);
        }

        public async Task<List<IGDBGame>> SearchGamesPagedAsync(string query, int offset, int limit)
        {
            var token = await _tokenService.GetTokenAsync();
            var clientId = _config["Twitch:ClientId"];

            var cleanQuery = query.Replace("\"", "").Trim();
            var escapedQuery = cleanQuery.Replace("\"", "\"\"");

            var searchQuery = $@"
            fields name, summary, cover.url, first_release_date, total_rating, rating_count, game_type;
            where name ~ *""{escapedQuery}""* & game_type = (0,8,9,10) & cover != null;
            sort total_rating desc;
            limit {limit};
            offset {offset};
            ";





            var results = await QueryIGDBAsync(clientId, token, searchQuery);

            if (results == null || results.Count == 0)
            {
                var fallbackQuery = $@"
        fields name, summary, cover.url, first_release_date, total_rating, rating_count;
        where name ~ /.*{cleanQuery}.*/i;
        limit {limit};
        offset {offset};";

                results = await QueryIGDBAsync(clientId, token, fallbackQuery);
            }

            return results ?? new List<IGDBGame>();
        }

        public async Task<List<IGDBGameEntity>> LoadMoreGamesFromDb(int offset, int limit)
        {
            return await _dbContext.Games
                .Include(g => g.Cover)
                .Include(g => g.Prices)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }

}
