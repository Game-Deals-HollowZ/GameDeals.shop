using GameDeals.Shared.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GameDeals.Shared.Services
{
    public interface IGDBService
    {
        Task<List<IGDBGame>> GetGamesAsync();

        Task<List<IGDBGame>> SearchGamesAsync(string query);
    }

}
