using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Converters
{
    public static class UnitConverters
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 1.8 + 32.0;
        }
        public static double MetersPerSecondsToKilometersPerHours(double mps)
        {
            return mps * 3.6;
        }
    }
}
