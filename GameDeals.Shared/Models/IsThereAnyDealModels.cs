using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDeals.Shared.Models
{
    public class PlainResponse
    {
        public PlainData Data { get; set; }
    }

    public class PlainData
    {
        public string Plain { get; set; }
    }

    public class PriceResponse
    {
        public Dictionary<string, GamePriceData> Data { get; set; }
    }

    public class GamePriceData
    {
        public List<PriceEntry> List { get; set; }
    }

    public class PriceEntry
    {
        public ShopInfo Shop { get; set; } = new();
        public decimal PriceNew { get; set; }
        public string Url { get; set; } = "";
    }

    public class ShopInfo
    {
        public string Name { get; set; } = "";
    }
}
