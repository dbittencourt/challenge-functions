using Challenge.Models;

namespace Challenge.Sorters
{
    public interface IProductSorterFactory
    {
        IProductSorter GetProductSorter(SortOption option);
    }
}