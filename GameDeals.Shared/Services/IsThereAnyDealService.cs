using GameDeals.Shared.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameDeals.Shared.Services
{
    public interface IsThereAnyDealService
    {
           Task<string?> GetGameUuidAsync(string title);

           Task<List<PriceEntry>> GetPricesAsync(string uuid);

           Task<List<PriceEntry>> GetPricesByTitleAsync(string title);
        
    }
}
