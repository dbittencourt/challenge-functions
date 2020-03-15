using System.Collections.Generic;
using System.Linq;
using Challenge.Models;

namespace Challenge.Sorters
{
    public class LowSorter : IProductSorter
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Price);
        }
    }
    
    public class HighSorter : IProductSorter
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderByDescending(p => p.Price);
        }
    }
    
    public class AscendingSorter : IProductSorter
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Name);
        }
    }
    
    public class DescendingSorter : IProductSorter
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderByDescending(p => p.Name);
        }
    }
    
    public class PopularitySorter : IProductSorter
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            // I'm not sure whats the definition of popularity...
            // returns products ordered by number of appearances in customers lists of products
            return products.GroupBy(p => p.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.FirstOrDefault()).ToList();
            
            // returns products ordered by the sum of its quantities from customers lists of products
            var prods = new Dictionary<string, Product>();
            foreach (var product in products)
            {
                if (prods.ContainsKey(product.Name))
                    prods[product.Name].Quantity += product.Quantity;
                else
                    prods[product.Name] = product;
            }
            return prods.Values.OrderByDescending(p => p.Quantity);
        }
    }
}