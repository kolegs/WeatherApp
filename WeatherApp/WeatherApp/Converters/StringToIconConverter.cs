using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Converters
{
    public static class StringToIconConverter
    {
        public static string Convert(string value)
        {
            switch (value)
            {
                case "01d":
                    return "weather_fc_clear_day.png";
                case "01n":
                    return "weather_fc_clear_night.png";

                case "02d":
                    return "weather_fc_partly_cloudy_day.png";
                case "02n":
                    return "weather_fc_partly_cloudy_night.png";

                case "03d":
                    return "weather_fc_cloudy.png";
                case "03n":
                    return "weather_fc_cloudy.png";

                case "04d":
                    return "weather_fc_cloudy.png";
                case "04n":
                    return "weather_fc_cloudy.png";

                case "09d":
                    return "weather_fc_rain.png";
                case "09n":
                    return "weather_fc_rain.png";

                case "10d":
                    return "weather_fc_10d.png";
                case "10n":
                    return "weather_fc_10n.png";

                case "11d":
                    return "weather_fc_11d.png";
                case "11n":
                    return "weather_fc_11n.png";

                case "13d":
                    return "weather_fc_snow.png";
                case "13n":
                    return "weather_fc_13n.png";

                case "50d":
                    return "weather_fc_fog.png";
                case "50n":
                    return "weather_fc_fog.png";
                
                default:
                    return "weather_fc_clear_day.png";
            }
        }
    }
}
