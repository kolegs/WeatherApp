using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Models
{
    public class NotificationPlace
    {
        public bool Storm { get; set; } = false;
        public bool Rain { get; set; } = false;
        public bool Snow { get; set; } = false;
        public bool Frost { get; set; } = false;

        public enum NotificationType
        {
            Storm = 0,
            Rain = 1,
            Snow = 2,
            Frost = 3,
        }
    }
}
