
using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TangentWatson.WatsonClient.Interfaces;
using TangentWatson.WatsonClient.Startup;
using AppConstants = TangentWatson.Constants.Constants;

[assembly: FunctionsStartup(typeof(WatsonClientStartup))]
namespace TangentWatson.WatsonClient.Startup
{
    public class WatsonClientStartup :FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            var httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(AppConstants.WatsonAPIUserName,
                    Environment.GetEnvironmentVariable(AppConstants.WatsonApiKeyKey)),
            });
            httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable(AppConstants.WatsonUrlKey));


            builder.Services.AddTransient<IWatsonClient, WatsonClient>(x =>
                new WatsonClient(httpClient, x.GetRequiredService<ILogger<WatsonClient>>()));
        }

    }
}