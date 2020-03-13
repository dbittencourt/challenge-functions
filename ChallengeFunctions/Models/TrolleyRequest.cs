namespace Challenge.Models
{
    public class TrolleyProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductQuantity
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }

    public class Special
    {
        public ProductQuantity[] Quantities { get; set; }
        public decimal Total { get; set; }
    }
    
    public class TrolleyRequest
    {
        public TrolleyProduct[] Products { get; set; }
        public Special[] Specials { get; set; }
        public ProductQuantity[] Quantities { get; set; }
    }
}