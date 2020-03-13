using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Challenge.Models;
using Challenge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChallengeFunctions
{
    public class AnswersApi
    {
        private readonly IProductService _productService;
        private readonly ITrolleyService _trolleyService;

        public AnswersApi(IProductService productService, ITrolleyService trolleyService)
        {
            _productService = productService;
            _trolleyService = trolleyService;
        }
        
        [FunctionName("GetUser")]
        public ActionResult GetUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "answers/user")]
            HttpRequest req, ILogger log)
        {
            var user = new User
            {
                Name = "Daniel Bittencourt",
                Token = "1234-455662-22233333-3333"
            };
            
            return new OkObjectResult(user);
        }

        [FunctionName(("GetSortedProducts"))]
        public async Task<ActionResult<Product[]>> GetSortedProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "answers/sort")] 
            HttpRequest req, ILogger log)
        {
            // parse query parameter
            var sortParam = req.GetQueryParameterDictionary()
                .FirstOrDefault(p => string.Compare(p.Key, "sortOption", true) == 0)
                .Value;
            if (!Enum.TryParse<SortOption>(sortParam, true, out var sortOption))
                return new BadRequestObjectResult("Invalid sort option");
            
            var sortedProducts = await _productService.GetSortedProductsAsync(sortOption);
            
            if (sortedProducts != null && sortedProducts.Any())
                return new OkObjectResult(sortedProducts.ToArray());

            return new NotFoundObjectResult(new Product[] {});
        }
        
        [FunctionName(("GetTrolleyTotal"))]
        public async Task<ActionResult<decimal>> GetTrolleyTotal([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "answers/trolleyTotal")] 
            HttpRequest req, ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var trolleyRequest = JsonConvert.DeserializeObject<TrolleyRequest>(requestBody);

            var trolleyTotal =
                await _trolleyService.GetTrolleyTotalAsync(trolleyRequest.Products, trolleyRequest.Specials, trolleyRequest.Quantities);
            
            return trolleyTotal;
        }
    }
}