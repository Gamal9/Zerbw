﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zerbow.Views;
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Zerbow
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Login())
            {
                BarBackgroundColor = Color.FromHex("#7A3B75"),

            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
