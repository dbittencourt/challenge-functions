namespace Challenge.Models
{
    public class Customer
    {
        public long CustomerId { get; set; }
        public Product[] Products { get; set; }
    }
}