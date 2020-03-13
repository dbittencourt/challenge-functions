using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Challenge.Models;

namespace Challenge.Services
{
    public class TrolleyService : ITrolleyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _wooliesApiEndpoint;
        private readonly string _appToken;
        
        public TrolleyService(HttpClient httpClient)
        {
            _wooliesApiEndpoint = Environment.GetEnvironmentVariable("WooliesApiEndpoint");
            _appToken = Environment.GetEnvironmentVariable("AppToken");
            _httpClient = httpClient;
        }
        
        public async Task<decimal> GetTrolleyTotalAsync(IEnumerable<TrolleyProduct> products, 
            IEnumerable<Special> specials, IEnumerable<ProductQuantity> quantities)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{_wooliesApiEndpoint}trolleyCalculator?token={_appToken}")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new TrolleyRequest
                    {
                        Products = products.ToArray(),
                        Quantities = quantities.ToArray(),
                        Specials = specials.ToArray()
                    }), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            
            // maybe an exception should be thrown? but thats overkill for the use case
            if (!response.IsSuccessStatusCode) 
                return 0;
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            
            return await JsonSerializer.DeserializeAsync<decimal>(responseStream);
        }
    }
}