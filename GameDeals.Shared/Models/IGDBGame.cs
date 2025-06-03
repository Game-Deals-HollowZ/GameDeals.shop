using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDeals.Shared.Models
{
    public class IGDBGame
    {
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public double? Rating { get; set; }
        public long? First_Release_Date { get; set; }
        public Cover? Cover { get; set; }
    }

    public class Cover
    {
        public string? Url { get; set; }
    }

}
