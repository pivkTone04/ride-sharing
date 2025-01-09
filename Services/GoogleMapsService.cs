using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RideSharing.ViewModels;

namespace RideSharing.Services
{
    public class GoogleMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _directionsApiKey;

        public GoogleMapsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _directionsApiKey = configuration["GoogleMaps:DirectionsApiKey"];
        }

        public async Task<JObject> GetDirectionsAsync(string origin, string destination)
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={Uri.EscapeDataString(origin)}&destination={Uri.EscapeDataString(destination)}&key={_directionsApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google Maps API request failed with status code {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(jsonString);
        }

    
        public RideCreateViewModel ExtractRideData(JObject directions)
        {
            var viewModel = new RideCreateViewModel
            {
                TotalDistance = "N/A",
                TotalDuration = "N/A"
            };

            if (directions["status"]?.ToString() != "OK")
            {
                return viewModel;
            }

            var routes = directions["routes"] as JArray;
            if (routes == null || routes.Count == 0)
            {
                return viewModel;
            }

            var firstRoute = routes[0];
            var legs = firstRoute["legs"] as JArray;

            if (legs == null || legs.Count == 0)
            {
                return viewModel;
            }

            var firstLeg = legs[0];

            viewModel.TotalDistance = firstLeg["distance"]?["text"]?.ToString() ?? "N/A";
            viewModel.TotalDuration = firstLeg["duration"]?["text"]?.ToString() ?? "N/A";

            return viewModel;
        }
    }
}
