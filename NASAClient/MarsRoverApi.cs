using System;
using System.Collections.Generic;
using Microsoft.Extensions.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using NASAClient.Models;

namespace NASAClient
{
    public class MarsRoverApi
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MarsRoverApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<EarthDateResponse> GetPicturesInformationByEarthDay(DateTime earthDay)
        {
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetStringAsync("https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?earth_date=2015-6-3&api_key=DEMO_KEY");
            var earthDateResponse = JsonSerializer.Deserialize<EarthDateResponse>(result);
            return earthDateResponse;
        }

        public async Task<byte[]> GetPicture(string url)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetByteArrayAsync(url);
        }
    }
}
