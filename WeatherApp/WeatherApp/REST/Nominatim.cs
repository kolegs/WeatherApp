using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherApp.REST
{
    public class Nominatim
    {
        public static async Task<List<Addresses>> GetAddress(string text)
        {
            var address =
                $"https://nominatim.openstreetmap.org/search?q={text}&format=json&polygon_geojson=1&addressdetails=1";
            using (var client = new HttpClient())
            {
                var response =
                    await client.GetAsync(address);
                if (response.IsSuccessStatusCode == false)
                    return null;
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Addresses>>(responseString);
            }
        }

        public static async Task<Addresses> GetAddress(double latitude, double longitude)
        {
            var lat = latitude.ToString("F6", new CultureInfo("en-US"));
            var lon = longitude.ToString("F6", new CultureInfo("en-US"));
            var address =
                $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                var response =
                    await client.GetAsync(address);
                if (response.IsSuccessStatusCode == false)
                    return null;
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Addresses>(responseString);
            }
        }

        public class Addresses
        {
            [JsonProperty("lon")]
            public double Longitude;
            [JsonProperty("lat")]
            public double Latitude;
            [JsonProperty("display_name")]
            public string DisplayName;

            [JsonProperty("address")]
            public AddressStruct Address = new AddressStruct();
        }
        
        public class AddressStruct
        {
            [JsonProperty("administrative")]
            public string Administrative;
            [JsonProperty("county")]
            public string County;
            [JsonProperty("state")]
            public string State;
            [JsonProperty("country")]
            public string Country;
            [JsonProperty("country_code")]
            public string CountryCode;
            [JsonProperty("city")]
            public string City;
            [JsonProperty("village")]
            public string Village;
            [JsonProperty("municipality")]
            public string Municipality;
            [JsonProperty("hamlet")]
            public string Hamlet;
            [JsonProperty("landuse")]
            public string Landuse;

        }

    }
}
