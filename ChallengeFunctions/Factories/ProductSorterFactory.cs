using Challenge.Models;

namespace Challenge.Sorters
{
    public class ProductSorterFactory : IProductSorterFactory
    {
        public IProductSorter GetProductSorter(SortOption option)
        {
            return option switch
            {
                SortOption.Low => new LowSorter(),
                SortOption.High => new HighSorter(),
                SortOption.Ascending => new AscendingSorter(),
                SortOption.Descending => new DescendingSorter(),
                SortOption.Recommended => new PopularitySorter(),
                _ => throw new System.NotImplementedException()
            };
        }
    }
}