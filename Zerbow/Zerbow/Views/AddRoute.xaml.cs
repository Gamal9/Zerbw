﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Zerbow.Models;
using Zerbow.Services;

namespace Zerbow.Views
{
    public partial class AddRoute : ContentPage
    {
       
        private Users currentUser;
        private Routes newRoute;
        private bool carSelected;
        private IDictionary<string, object> properties;
        private List<Cars> carsList;
        private bool savePoints;

        public AddRoute()
        {
            carSelected = false;
            savePoints = false;
           

            properties = Application.Current.Properties;
            currentUser = (Users)Application.Current.Properties["user"];
            this.IsBusy = true;
            InitializeComponent();

            departureDatePicker.MinimumDate = DateTime.Now;
            departureTimePicker.Time = DateTime.Now.TimeOfDay;
        }

        async void OnAdd(object sender, EventArgs e)
        {
            savePoints = true;
            carSelected = false;
            await Navigation.PushAsync(new AddCar());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            savePoints = false;

            if (carSelected == false)
            {
                carsList = await AzureManager.AzureManagerInstance.GetMyCarsAsync(currentUser);

                carPicker.Items.Clear();

                foreach (var car in carsList)
                {
                    carPicker.Items.Add(car.Model + " " + car.Color);
                }

                carPicker.IsEnabled = carPicker.Items.Count > 0 ? true : false;

                this.IsBusy = false;
            }

            if (properties.ContainsKey("route"))
            {
                newRoute = (Routes)Application.Current.Properties["route"];

                if (!string.IsNullOrEmpty(newRoute.From_Latitude))
                    startingPointButton.Text = "Change Starting point";
                if (!string.IsNullOrEmpty(newRoute.To_Latitude))
                    endingPointButton.Text = "Change Ending point";
            }
        }

        public async void OnStartingPoint(object sender, EventArgs e)
        {
            savePoints = true;
            await Navigation.PushAsync(new MapStartingPoint());
        }

        public async void OnEndingPoint(object sender, EventArgs e)
        {
            savePoints = true;
            await Navigation.PushAsync(new MapEndingPoint());
        }

        public void OnCarPicker(object sender, EventArgs e)
        {
            carSelected = true;
            carPicker.BackgroundColor = Color.FromHex("#00897B");
        }

        public async void OnSaveRoute(object sender, EventArgs e)
        {
            bool validation = false;

            if (!properties.ContainsKey("route"))
            {
                await DisplayAlert("Error", "Select points in map", "accept");
            }
            else
            {
                if (string.IsNullOrEmpty((properties["route"] as Routes).From_Latitude) ||
                    string.IsNullOrEmpty((properties["route"] as Routes).To_Latitude))
                {
                    await DisplayAlert("Error", "Select points in map", "accept");
                }
                else
                {
                    if (string.IsNullOrEmpty(startingNameEntry.Text) || string.IsNullOrEmpty(endingNameEntry.Text) ||
                    string.IsNullOrEmpty(commentsEditor.Text))
                    {
                        await DisplayAlert("Error", "fill blank fields", "accept");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(seatsEntry.Text))
                        {
                            await DisplayAlert("Error", "Seats can't be null", "accept");
                        }
                        else
                        {
                            if (carPicker.SelectedIndex == -1)
                            {
                                await DisplayAlert("error", "select a car", "accept");
                            }
                            else
                            {
                                validation = true;
                            }
                        }
                    }
                }

            }

            if (validation == true)
            {
                newRoute = (Routes)properties["route"];
                newRoute.From = startingNameEntry.Text;
                newRoute.To = endingNameEntry.Text;
                newRoute.Capacity = int.Parse("" + seatsEntry.Text);
                newRoute.Comments = commentsEditor.Text;
                newRoute.Depart_Time = departureTimePicker.Time.ToString();
                newRoute.Id_User = currentUser.ID;

                DateTimeOffset dateRoute = new DateTimeOffset(departureDatePicker.Date.Add(departureTimePicker.Time));

                newRoute.Depart_Date = dateRoute.DateTime;
                newRoute.Depart_Time = departureTimePicker.Time.ToString();
                string carSelected = carPicker.Items.ElementAt(carPicker.SelectedIndex);
                Cars car = carsList.Where(cars => cars.Model + " " + cars.Color == carSelected).First();

                newRoute.Id_Car = car.ID;

                activityIndicator.IsRunning = true;
                await AzureManager.AzureManagerInstance.SaveRouteAsync(newRoute);
                activityIndicator.IsRunning = false;

                await DisplayAlert("Success", "Route added successfully", "Accept");
                properties.Remove("route");
                await Navigation.PopAsync(true);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (savePoints == false)
            {
                Application.Current.Properties.Remove("route");
            }
        }
    }
}