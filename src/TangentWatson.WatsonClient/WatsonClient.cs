using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using TangentWatson.Entities;
using TangentWatson.WatsonClient.Interfaces;

#if DEBUG
[assembly: InternalsVisibleTo("TangentWatson.WatsonClient.Tests")]
#endif
namespace TangentWatson.WatsonClient
{
    internal class WatsonClient : IWatsonClient
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger _log;

        public WatsonClient(HttpClient httpClient, ILogger<WatsonClient> log)
        {
            _httpClient = httpClient;
            _log = log;
        }
        public async Task<WatsonResponse> GetResponse(string message)
        {
            _log.LogInformation($"Getting Watson response for message: {message}");
            try
            {
                var watsonResponse = await _httpClient.PostAsync("", new StringContent(message, Encoding.UTF8));
                if (watsonResponse.IsSuccessStatusCode)
                {
                    var responseBody = await watsonResponse.Content.ReadAsStringAsync();
                    _log.LogDebug($"Response from watson - {responseBody}");
                    return JsonSerializer.Deserialize<WatsonResponse>(responseBody);
                }
                else
                {
                    _log.LogError( $"an error occured in watson API request status code {watsonResponse.StatusCode}");
                    throw new Exception("Watson service call failure");
                }
            }
            catch (JsonException ex)
            {
                _log.LogError(ex, "an error occured deserializing the response from watson");
                throw;
            }
        }
    }
}
