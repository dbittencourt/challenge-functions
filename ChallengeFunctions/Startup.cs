using System;
using Challenge.Services;
using Challenge.Sorters;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ChallengeFunctions.Startup))]
namespace ChallengeFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IProductSorterFactory, ProductSorterFactory>();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ITrolleyService, TrolleyService>();
        }
    }
}