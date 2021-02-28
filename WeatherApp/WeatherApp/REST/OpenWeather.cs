using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherApp.Models;

namespace WeatherApp.REST
{
    public class OpenWeather
    {
        public static string API_KEY = "d9f76563ef774b1be4e2622bcad98d58";
        public static async Task<OpenWeatherOneCall> GetWeather(double latitude, double longitude, Units units)
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture;

            var language = culture.Name.Split('-').FirstOrDefault();

            var address =
                $"https://api.openweathermap.org/data/2.5/onecall?lat={latitude}&lon={longitude}&appid={API_KEY}&lang={language}&units={UnitsToString(units)}&exclude=minutely,alerts";
            using (var client = new HttpClient())
            {
                var response =
                    await client.GetAsync(address);
                if (response.IsSuccessStatusCode == false)
                    return null;
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OpenWeatherOneCall>(responseString);
            }
        }

        private static string UnitsToString(Units units)
        {
            switch (units)
            {
                case Units.Metric:
                    return "metric";
                case Units.Imperial:
                    return "imperial";
                default:
                    throw new ArgumentOutOfRangeException(nameof(units), units, null);
            }

        }
        
        public enum Units
        {
            Metric,
            Imperial
        }

        // https://nominatim.openstreetmap.org/search?q=toru%C5%84&format=json&polygon_geojson=1&addressdetails=1
    }
}
