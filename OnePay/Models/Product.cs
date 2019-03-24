using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnePay.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhotoName { get; set; }

        public decimal Price { get; set; }
    }

    public class ProductItems
    {
        public static List<Product> lstProducts;

        public List<Product> GetProductItems
        {
            get
            {
                lstProducts = new List<Product>();
                lstProducts.Add(new Product { Id = 1, Name = "Iphone 4s 16G", PhotoName = "iphone-4s-black.jpg", Price = 199 });
                lstProducts.Add(new Product { Id = 2, Name = "Iphone 5 16G", PhotoName = "iPhone-5.jpg", Price = 199 });
                return lstProducts;
            }
        }
    }
}