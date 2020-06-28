using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
namespace TangentWatson.WatsonClient.Tests
{
    public class WatsonClientTests
    {
       
        private class MockHttpHandler : HttpMessageHandler
        {
            public HttpResponseMessage MessageToResponsdWith { get; set;}

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(MessageToResponsdWith);
            }
        }

        private  ILogger<T> CreateLogger<T>()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            return factory.CreateLogger<T>();
        }


        private WatsonClient GetClient(HttpResponseMessage requiredRespone)
        {
            var client = new HttpClient(new MockHttpHandler()
            {
                MessageToResponsdWith = requiredRespone
            })
            {
                BaseAddress = new Uri("https://www.google.com")
            };
           return new WatsonClient(client, CreateLogger<WatsonClient>());
        }

        [Fact]
        public async Task WatsonErrorThrows()
        {
            var errorClient = GetClient(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            });
            await Assert.ThrowsAsync<Exception>(()=> errorClient.GetResponse("test message"));
        }

        [Fact]
        public async Task WatsonInvalidJson()
        {
            var errorClient = GetClient(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("just some random stuff goes here")
            });
            await Assert.ThrowsAsync<JsonException>(() => errorClient.GetResponse("test message"));
        }

        [Fact]
        public async Task SuccessCall()
        {
            var successClient = GetClient(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                   "{\r\n  \"document_tone\": {\r\n    \"tones\": [\r\n      {\r\n        \"score\": 0.6165,\r\n        \"tone_id\": \"sadness\",\r\n        \"tone_name\": \"Sadness\"\r\n      },\r\n      {\r\n        \"score\": 0.829888,\r\n        \"tone_id\": \"analytical\",\r\n        \"tone_name\": \"Analytical\"\r\n      }\r\n    ]\r\n  },\r\n  \"sentences_tone\": [\r\n    {\r\n      \"sentence_id\": 0,\r\n      \"text\": \"Team, I know that times are tough!\",\r\n      \"tones\": [\r\n        {\r\n          \"score\": 0.801827,\r\n          \"tone_id\": \"analytical\",\r\n          \"tone_name\": \"Analytical\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"sentence_id\": 1,\r\n      \"text\": \"Product sales have been disappointing for the past three quarters.\",\r\n      \"tones\": [\r\n        {\r\n          \"score\": 0.771241,\r\n          \"tone_id\": \"sadness\",\r\n          \"tone_name\": \"Sadness\"\r\n        },\r\n        {\r\n          \"score\": 0.687768,\r\n          \"tone_id\": \"analytical\",\r\n          \"tone_name\": \"Analytical\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"sentence_id\": 2,\r\n      \"text\": \"We have a competitive product, but we need to do a better job of selling it!\",\r\n      \"tones\": [\r\n        {\r\n          \"score\": 0.506763,\r\n          \"tone_id\": \"analytical\",\r\n          \"tone_name\": \"Analytical\"\r\n        }\r\n      ]\r\n    }\r\n  ]\r\n}")
            });
            var resp = await successClient.GetResponse("test message");
            Assert.Equal("Sadness", resp.DocumentTone.Tones[0].Name);
        }
    }
}
