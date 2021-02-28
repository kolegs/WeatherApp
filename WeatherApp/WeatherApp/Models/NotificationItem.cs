using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using WeatherApp.Annotations;

namespace WeatherApp.Models
{
    public class NotificationItem : INotifyPropertyChanged
    {
        private string _notificationName;
        public string NotificationName
        {
            get => _notificationName;
            set
            {
                if (_notificationName == value)
                    return;
                _notificationName = value;
                OnPropertyChanged();
            }
        }
        
        public NotificationPlace.NotificationType NotificationType { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                OnPropertyChanged();
                
                Settings.Settings.Instance.UpdateNotification(Latitude, Longitude, IsEnabled, NotificationType);
            }
        }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
