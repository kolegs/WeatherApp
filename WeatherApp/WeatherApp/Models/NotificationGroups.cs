using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Models
{
    public class NotificationGroups : List<NotificationItem>
    { 
        public string PlaceName { get; set; }
    }
}
