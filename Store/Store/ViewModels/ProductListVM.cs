using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Store.Models
{
    public class ProductListVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSummary { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Image Images { get; set; }

    }
}