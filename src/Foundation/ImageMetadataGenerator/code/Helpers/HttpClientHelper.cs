using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Sitecore.Diagnostics;

namespace Foundation.ImageMetadataGenerator.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient PopulateHttpClient(this HttpClient client, string baseAddress, string apiKey)
        {
            Assert.IsNotNull(client, "client");
            
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            return client;
        }
    }
}