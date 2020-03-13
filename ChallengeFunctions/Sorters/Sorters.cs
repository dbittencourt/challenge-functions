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
            var prods = new Dictionary<string, Product>();

            foreach (var product in products)
            {
                if (prods.ContainsKey(product.Name))
                    prods[product.Name].Quantity += product.Quantity;
                else
                    prods[product.Name] = product;
            }

            return prods.Values.OrderByDescending(p => p.Quantity);

            var prods2 = products.GroupBy(p => p.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.FirstOrDefault()).ToArray();
        }
    }
}