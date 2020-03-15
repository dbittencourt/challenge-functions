using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.Models;

namespace Challenge.Services
{
    public interface ITrolleyService
    {
        Task<decimal> GetTrolleyTotalAsync(IEnumerable<TrolleyProduct> products, IEnumerable<Special> specials, 
            IEnumerable<ProductQuantity> quantities);
        
        decimal CalculateTrolleyTotal(IEnumerable<TrolleyProduct> products, IEnumerable<Special> specials, 
            IEnumerable<ProductQuantity> quantities);
    }
}