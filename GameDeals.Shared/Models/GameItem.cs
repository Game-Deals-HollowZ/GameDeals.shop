using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDeals.Shared.Models
{
    public class GameItem
    {
        public string Title, Description, ImageUrl, Link;
        public double Price;

        public GameItem(string title, string desc, string img, double price, string link)
        {
            Title = title; Description = desc; ImageUrl = img; Price = price; Link = link;
        }
    }
}
