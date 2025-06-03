using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameDeals.Shared.Services
{
    public class TwitchTokenService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private string? _accessToken;
        private DateTime _expiresAt;

        public TwitchTokenService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _expiresAt)
                return _accessToken;

            var clientId = _config["Twitch:ClientId"];
            var clientSecret = _config["Twitch:ClientSecret"];
            var tokenUrl = _config["Twitch:TokenUrl"];

            var response = await _http.PostAsync($"{tokenUrl}?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", null);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TwitchTokenResponse>();
            _accessToken = result!.AccessToken;
            _expiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60); // -60s als Sicherheitspuffer

            return _accessToken;
        }

        private class TwitchTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }
    }

}
