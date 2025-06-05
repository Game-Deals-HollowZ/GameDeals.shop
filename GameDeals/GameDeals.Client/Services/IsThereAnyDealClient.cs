using GameDeals.Shared.Models;
using GameDeals.Shared.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameDeals.Client.Services
{
    public class IsThereAnyDealClient : IsThereAnyDealService
    {
        private readonly HttpClient _http;

        public IsThereAnyDealClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<string?> GetGameUuidAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            // ruft GET api/IsThereAnyDeal/uuid?title={title} auf
            return await _http.GetFromJsonAsync<string?>($"api/IsThereAnyDeal/uuid?title={Uri.EscapeDataString(title)}");
        }

        public async Task<List<PriceEntry>> GetPricesAsync(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return new List<PriceEntry>();

            // ruft GET api/IsThereAnyDeal/prices/{uuid} auf
            return await _http.GetFromJsonAsync<List<PriceEntry>>($"api/IsThereAnyDeal/prices/{Uri.EscapeDataString(uuid)}") ?? new List<PriceEntry>();
        }

        public async Task<List<PriceEntry>> GetPricesByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<PriceEntry>();

            var response = await _http.GetAsync($"api/IsThereAnyDeal/pricesByTitle?title={Uri.EscapeDataString(title)}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);  // oder Debug-Ausgabe

            // Falls JSON kein Array ist, hier anpassen
            return JsonSerializer.Deserialize<List<PriceEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            }) ?? new List<PriceEntry>();
        }
    }
}
