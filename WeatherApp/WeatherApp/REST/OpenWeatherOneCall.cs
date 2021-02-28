using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WeatherApp.REST
{
    public class OpenWeatherOneCall
    {
        [JsonProperty("lon")]
        public double Longitude;
        [JsonProperty("lat")]
        public double Latitude;
        [JsonProperty("timezone")]
        public string Timezone;
        [JsonProperty("timezone_offset")]
        public int TimezoneOffset;
        
        [JsonProperty("current")]
        public CurrentWeather Current = new CurrentWeather();

        [JsonProperty("hourly")]
        public List<HourlyWeather> Hourly = new List<HourlyWeather>();
        [JsonProperty("daily")]
        public List<DailyWeather> Daily = new List<DailyWeather>();

        public class CurrentWeather
        {
            [JsonProperty("dt")]
            public int Datetime;
            [JsonProperty("sunrise")]
            public int Sunrise;
            [JsonProperty("sunset")]
            public int Sunset;
            [JsonProperty("temp")]
            public double Temperature;
            [JsonProperty("feels_like")]
            public double FeelsLike;
            [JsonProperty("pressure")]
            public double Pressure;
            [JsonProperty("humidity")]
            public double Humidity;
            [JsonProperty("dew_point")]
            public double DewPoint;
            [JsonProperty("uvi")]
            public double Uvi;
            [JsonProperty("clouds")]
            public double Clouds;
            [JsonProperty("visibility")]
            public int Visibility;
            [JsonProperty("wind_speed")]
            public double WindSpeed;
            [JsonProperty("wind_deg")]
            public double WindDegree;
            [JsonProperty("wind_gust")]
            public double WindGust;
            [JsonProperty("weather")]
            public List<Weather> WeatherList = new List<Weather>();
            [JsonProperty("snow")]
            public Intensity Snow = new Intensity();
            [JsonProperty("rain")]
            public Intensity Rain = new Intensity();

        }
        public class Weather
        {
            [JsonProperty("id")]
            public int Id;
            [JsonProperty("main")]
            public string Main;
            [JsonProperty("description")]
            public string Description;
            [JsonProperty("icon")]
            public string Icon;
        }

        public class HourlyWeather
        {
            [JsonProperty("dt")]
            public int Datetime;
            [JsonProperty("temp")]
            public double Temperature;
            [JsonProperty("feels_like")]
            public double FeelsLike;
            [JsonProperty("pressure")]
            public double Pressure;
            [JsonProperty("humidity")]
            public double Humidity;
            [JsonProperty("dew_point")]
            public double DewPoint;
            [JsonProperty("uvi")]
            public double Uvi;
            [JsonProperty("clouds")]
            public double Clouds;
            [JsonProperty("visibility")]
            public int Visibility;
            [JsonProperty("wind_speed")]
            public double WindSpeed;
            [JsonProperty("wind_deg")]
            public double WindDegree;
            [JsonProperty("wind_gust")]
            public double WindGust;
            [JsonProperty("weather")]
            public List<Weather> WeatherList = new List<Weather>();
            [JsonProperty("pop")]
            public double Pop;
            [JsonProperty("snow")]
            public Intensity Snow = new Intensity();
            [JsonProperty("rain")]
            public Intensity Rain = new Intensity();

        }

        public class DailyWeather
        {
            [JsonProperty("dt")]
            public int Datetime;
            [JsonProperty("sunrise")]
            public int Sunrise;
            [JsonProperty("sunset")]
            public int Sunset;
            [JsonProperty("temp")]
            public TemperatureDaily Temperature = new TemperatureDaily();
            [JsonProperty("feels_like")]
            public FeelsLikeDaily FeelsLike = new FeelsLikeDaily();
            [JsonProperty("pressure")]
            public double Pressure;
            [JsonProperty("humidity")]
            public double Humidity;
            [JsonProperty("dew_point")]
            public double DewPoint;
            [JsonProperty("wind_speed")]
            public double WindSpeed;
            [JsonProperty("wind_deg")]
            public double WindDegree;
            [JsonProperty("weather")]
            public List<Weather> WeatherList = new List<Weather>();
            [JsonProperty("clouds")]
            public double Clouds;
            [JsonProperty("pop")]
            public double Pop;

            [JsonProperty("snow")] 
            public double Snow;
            [JsonProperty("rain")] 
            public double Rain;
            [JsonProperty("uvi")]
            public double Uvi;


            public class TemperatureDaily
            {
                [JsonProperty("day")]
                public double Day;
                [JsonProperty("min")]
                public double Minimum;
                [JsonProperty("max")]
                public double Maximum;
                [JsonProperty("night")]
                public double Night;
                [JsonProperty("eve")]
                public double Evening;
                [JsonProperty("morn")]
                public double Morning;
            }
            public class FeelsLikeDaily
            {
                [JsonProperty("day")]
                public double Day;
                [JsonProperty("night")]
                public double Night;
                [JsonProperty("eve")]
                public double Evening;
                [JsonProperty("morn")]
                public double Morning;
            }
        }

        public class Intensity
        {
            [JsonProperty("1h")]
            public double OneHour;
            [JsonProperty("3h")]
            public double ThreeHours;
        }


    }
}
