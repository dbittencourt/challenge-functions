using System.Collections.Generic;
using Challenge.Models;

namespace Challenge.Sorters
{
    public interface IProductSorter
    {
        IEnumerable<Product> Sort(IEnumerable<Product> products);
    }
}