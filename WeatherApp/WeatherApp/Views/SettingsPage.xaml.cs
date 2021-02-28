﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interface;
using WeatherApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = new SettingsViewModel();
        }

        private void SettingsPage_OnAppearing(object sender, EventArgs e)
        {
            (BindingContext as SettingsViewModel)?.Refresh();
        }
        private DateTime LastTimeButtonClicked { get; set; }
        protected override bool OnBackButtonPressed()
        {
            if (DateTime.Now - LastTimeButtonClicked > TimeSpan.FromSeconds(3))
            {
                LastTimeButtonClicked = DateTime.Now;
                DependencyService.Get<IMessage>().ShortAlert("Naciśnij ponownie, aby zamknąć aplikację");
                return true;
            }
            base.OnBackButtonPressed();
            return false;
        }
    }
}