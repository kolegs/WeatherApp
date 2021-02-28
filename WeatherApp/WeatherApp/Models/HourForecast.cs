using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using WeatherApp.Annotations;
using Xamarin.Forms.Internals;

namespace WeatherApp.Models
{
    public class HourForecast : INotifyPropertyChanged
    {
        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                if (value == _time)
                    return;
                _time = value;
                OnPropertyChanged();
            }
        }
        private string _iconSource;
        public string IconSource
        {
            get => _iconSource;
            set
            {
                if (value == _iconSource)
                    return;
                _iconSource = value;
                OnPropertyChanged();
            }
        }
        private string _temperature;
        public string Temperature
        {
            get => _temperature;
            set
            {
                if (value == _temperature)
                    return;
                _temperature = value;
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
