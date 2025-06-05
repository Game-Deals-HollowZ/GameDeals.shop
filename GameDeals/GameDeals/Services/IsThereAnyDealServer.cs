using GameDeals.Shared.Models;
using GameDeals.Shared.Services;
using System.Text.Json;

namespace GameDeals.Services
{
    public class IsThereAnyDealServer: IsThereAnyDealService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public IsThereAnyDealServer(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["IsThereAnyDeal:ApiKey"] ?? throw new ArgumentNullException("API Key missing");
        }

        public async Task<string?> GetGameUuidAsync(string title)
        {
            var url = $"https://api.isthereanydeal.com/games/lookup/v1?key={_apiKey}&title={Uri.EscapeDataString(title)}";
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            // Prüfe, ob "found" == true
            if (!root.TryGetProperty("found", out var foundProp) || !foundProp.GetBoolean())
                return null;

            // Prüfe, ob "game" vorhanden ist
            if (!root.TryGetProperty("game", out var gameProp))
                return null;

            // Hole die ID
            if (!gameProp.TryGetProperty("id", out var idProp))
                return null;

            return idProp.GetString();
        }



        public async Task<List<PriceEntry>> GetPricesAsync(string uuid)
        {
            var url = $"https://api.isthereanydeal.com/games/prices/v3?key={_apiKey}&country=DE&region=eu";

            var response = await _http.PostAsJsonAsync(url, new[] { uuid });

            if (!response.IsSuccessStatusCode)
                return new List<PriceEntry>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
                return new List<PriceEntry>();

            foreach (var item in root.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString();
                if (id != uuid)
                    continue;

                if (!item.TryGetProperty("deals", out var dealsArray))
                    return new List<PriceEntry>();

                var result = new List<PriceEntry>();

                foreach (var deal in dealsArray.EnumerateArray())
                {
                    var shop = deal.GetProperty("shop").GetProperty("name").GetString();
                    var price = deal.GetProperty("price").GetProperty("amount").GetDecimal();
                    var urlLink = deal.GetProperty("url").GetString();
                    if (price <= 0.01m || shop == null || shop.ToLower().Contains("mod"))
                        continue;
                    result.Add(new PriceEntry
                    {
                        Shop = new ShopInfo { Name = shop },
                        PriceNew = price,
                        Url = urlLink
                    });
                }

                return result;
            }

            return new List<PriceEntry>(); // falls uuid nicht gefunden wurde
        }



        public async Task<List<PriceEntry>> GetPricesByTitleAsync(string title)
        {
            var uuid = await GetGameUuidAsync(title);
            if (string.IsNullOrEmpty(uuid))
                return new List<PriceEntry>();

            return await GetPricesAsync(uuid);
        }
    }
}
