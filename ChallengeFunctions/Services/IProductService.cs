using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.Models;

namespace Challenge.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetSortedProductsAsync(SortOption type);
    }
}