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

        public decimal CalculateTrolleyTotal(IEnumerable<TrolleyProduct> products, IEnumerable<Special> specials, 
            IEnumerable<ProductQuantity> quantities)
        {
            var quantitiesDictionary = new Dictionary<string, int>();
            foreach (var product in quantities)
            {
                quantitiesDictionary[product.Name] = (int)product.Quantity;
            }
            
            var offers = specials.ToList();
            offers.AddRange(products.Select(product => new Special
            {
                Total = product.Price, 
                Quantities = new[]
                {
                    new ProductQuantity
                    {
                        Name = product.Name, Quantity = 1
                    }
                }
            }));

            var results = new Dictionary<string, decimal>();
            var minimumTotal = Minimum(offers, quantitiesDictionary, results);
            
            return minimumTotal;
        }

        private static decimal Minimum(IEnumerable<Special> offers, IDictionary<string, int> quantities, 
            IDictionary<string, decimal> results)
        {
            var key = GetQuantityKey(quantities);
            if (results.ContainsKey(key))
                return results[key];
            
            var minimumTotal = decimal.MaxValue;
            
            foreach (var offer in offers)
            {
                if (!OfferFits(offer, quantities)) 
                    continue;
                var cost = offer.Total;
                var quantitiesLeft = UpdateQuantities(quantities, offer);

                if (HasQuantitiesLeft(quantitiesLeft))
                    cost += Minimum(offers, quantitiesLeft, results);

                if (cost < minimumTotal)
                    minimumTotal = cost;
            }

            results[key] = minimumTotal;
            return minimumTotal;
        }

        private static bool OfferFits(Special offer, IDictionary<string, int> quantities)
        {
            foreach (var product in offer.Quantities)
            {
                if (!quantities.TryGetValue(product.Name, out var quantity) || quantity - (int) product.Quantity < 0)
                    return false;
            }

            return true;
        }

        private static IDictionary<string, int> UpdateQuantities(IDictionary<string, int> quantities, Special offer)
        {
            var quantitiesLeft = new Dictionary<string, int>(quantities);
            foreach (var product in offer.Quantities)
            {
                quantitiesLeft[product.Name] = quantities[product.Name] - (int)product.Quantity;
            }

            return quantitiesLeft;
        }

        private static bool HasQuantitiesLeft(IDictionary<string, int> quantities)
        {
            return quantities.Values.Any(v => v > 0);
        }

        private static string GetQuantityKey(IDictionary<string, int> quantities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var product in quantities.Values)
            {
                stringBuilder.Append(product + ":");
            }

            return stringBuilder.ToString();
        }
    }
}