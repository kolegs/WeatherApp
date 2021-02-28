using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using WeatherApp.Annotations;

namespace WeatherApp.Models
{
    public class Address : INotifyPropertyChanged
    {
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName == value)
                    return;
                _displayName = value;
                OnPropertyChanged();
            }
        }
        private string _placeName;
        public string PlaceName
        {
            get => _placeName;
            set
            {
                if (_placeName == value)
                    return;
                _placeName = value;
                OnPropertyChanged();
            }
        }
        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set
            {
                if (_longitude == value)
                    return;
                _longitude = value;
                OnPropertyChanged();
            }
        }
        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set
            {
                if (_latitude == value)
                    return;
                _latitude = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
