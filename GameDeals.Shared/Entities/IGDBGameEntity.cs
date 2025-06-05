using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDeals.Shared.Entities
{
    public class IGDBGameEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public double? Rating { get; set; }
        public long? FirstReleaseDate { get; set; }
        public int? GameType { get; set; }

        public CoverEntity? Cover { get; set; }
        public List<PriceEntryEntity>? Prices { get; set; }
    }

    public class CoverEntity
    {
        public int Id { get; set; }
        public string? Url { get; set; }

        public int IGDBGameEntityId { get; set; }
        public IGDBGameEntity? Game { get; set; }
    }

    public class PriceEntryEntity
    {
        public int Id { get; set; }

        public string ShopName { get; set; } = "";
        public decimal PriceNew { get; set; }
        public string Url { get; set; } = "";

        public int IGDBGameEntityId { get; set; }
        public IGDBGameEntity? Game { get; set; }
    }

}
