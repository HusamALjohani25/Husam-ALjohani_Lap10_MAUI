using System;
using System.Collections.Generic;
using System.Text;

namespace HusamALjohani_Lap10_MAUI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int Price { get; set; }
        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        public string info { get; set; }
        public string Category { get; set; }
    }
}
