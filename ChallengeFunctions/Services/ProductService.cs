using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Challenge.Models;
using Challenge.Sorters;

namespace Challenge.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductSorterFactory _factory;
        private readonly HttpClient _httpClient;
        private readonly string _wooliesApiEndpoint;
        private readonly string _appToken;

        public ProductService(IProductSorterFactory factory, HttpClient client)
        {
            _wooliesApiEndpoint = Environment.GetEnvironmentVariable("WooliesApiEndpoint");
            _appToken = Environment.GetEnvironmentVariable("AppToken");
            _factory = factory;
            _httpClient = client;
        }
        
        public async Task<IEnumerable<Product>> GetSortedProductsAsync(SortOption type)
        {
            var byPopularity = type == SortOption.Recommended;
            
            var products = await GetProductsAsync();
            if (byPopularity)
            {
                var customerProducts = await GetCustomerProductsAsync();
                products = products.Concat(customerProducts);
            }
            
            var sorter = _factory.GetProductSorter(type);
            return sorter.Sort(products);
        }

        private async Task<IEnumerable<Product>> GetProductsAsync()
        {
            // retrieves products using WooliesX api
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_wooliesApiEndpoint}products?token={_appToken}");
            var response = await _httpClient.SendAsync(request);
            
            // maybe an exception should be thrown? but thats overkill for the use case
            if (!response.IsSuccessStatusCode) 
                return new List<Product>();

            return await response.Content.ReadAsAsync<Product[]>();
        }

        private async Task<IEnumerable<Product>> GetCustomerProductsAsync()
        {
            // retrieves customers using WooliesX api
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_wooliesApiEndpoint}shopperHistory?token={_appToken}");
            var response = await _httpClient.SendAsync(request);
            
            // maybe an exception should be thrown? but thats overkill for the use case
            if (!response.IsSuccessStatusCode) 
                return new List<Product>();

            var customers = await response.Content.ReadAsAsync<Customer[]>();
            return customers.SelectMany(c => c.Products);   
        }
    }
}