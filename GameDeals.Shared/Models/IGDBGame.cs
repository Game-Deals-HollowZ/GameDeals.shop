using GameDeals.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameDeals.Shared.Models
{
    public class IGDBGame
    {
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public double? Rating { get; set; }
        public long? First_Release_Date { get; set; }
        [JsonPropertyName("game_type")]
        public int? GameType { get; set; }
        public Cover? Cover { get; set; }

        public List<PriceEntry>? Prices { get; set; }

        public IGDBGameEntity ToEntity()
        {
            return new IGDBGameEntity
            {
                Name = this.Name,
                Summary = this.Summary,
                Rating = this.Rating,
                FirstReleaseDate = this.First_Release_Date,
                GameType = this.GameType,
                Cover = this.Cover != null
                    ? new CoverEntity { Url = this.Cover.Url }
                    : null,
                Prices = this.Prices?.Select(p => new PriceEntryEntity
                {
                    ShopName = p.Shop?.Name ?? "",
                    PriceNew = p.PriceNew,
                    Url = p.Url
                }).ToList()
            };
        }


    }

    public class Cover
    {
        public string? Url { get; set; }
    }
}
