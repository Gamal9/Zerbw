using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Zerbow.Models;
using Zerbow.Services;

namespace Zerbow.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteDetails : ContentPage
    {
        private Users userRoute;
        private Routes route;
        private Users currentUser;
        private ToolbarItem callbtn;
        public RouteDetails(string idRoute)
        {

           
            InitializeComponent();

           
            currentUser = (Users)Application.Current.Properties["user"];

            LoadRouteData(idRoute);

            callbtn = new ToolbarItem
            {
                Text = "Call",
                Command = new Command(Callbt),
                Order = ToolbarItemOrder.Primary,
                Priority = 3,
                Icon = "icons8-phone-50.png"

            };

        }

        private void Callbt(object obj)
        {
            var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (PhoneCallTask.CanMakePhoneCall)
                PhoneCallTask.MakePhoneCall(userRoute.Phone);
        }

        private async void LoadRouteData(string idRoute)
        {
            this.IsBusy = true;

            route = await AzureManager.AzureManagerInstance.GetRouteWhere(route => route.Id == idRoute);
            userRoute = new Users
            {
                ID = route.Id_User
            };

            
            this.LoadReservation();
            this.LoadData();
        }

        private async void LoadData()
        {
            userRoute = await AzureManager.AzureManagerInstance.GetUserWhere(userSelect => userSelect.ID == userRoute.ID);

            nameLabel.Text = userRoute.Name;
            
            descriptionLabel.Text = "Comments:\n" + route.Comments;
            departureLabel.Text = "Departure: \n" + route.Depart_Date.ToString("dd/MMMM H:mm ") + "h";
            profileImage.Source =  userRoute.Photo;


            Reservations reservation = new Reservations
            {
                ID_Route = route.Id
            };

            List<Reservations> reservations = await AzureManager.AzureManagerInstance.GetReservationsWhere(reserv => reserv.ID_Route == reservation.ID_Route);

            //if user pay for use Appear Call Button
            if (reservations.Count > 0)
            {
                ToolbarItems.Add(callbtn);
            }
          
            seatsLabel.Text = "Seats Available: " + (route.Capacity - reservations.Count);
        }

        private async void LoadReservation()
        {
            string id_user = currentUser.ID;
            string id_route = route.Id;

            Reservations reservation = new Reservations
            {
                ID_User = id_user,
                ID_Route = id_route
            };

            List<Reservations> reservationResult = await AzureManager.AzureManagerInstance.GetReservationsWhere(reserv => reserv.ID_Route == reservation.ID_Route && reserv.ID_User == reservation.ID_User);

            if (reservationResult.Count != 0)
            {
                cancelButton.IsVisible = true;
                reserveButton.IsVisible = false;
                
            }
            else
            {
                reserveButton.IsVisible = true;
            }
          
            this.IsBusy = false;
        }


        private async void OnStartingPoint(object sender, EventArgs e)
        {
            var latitude = double.Parse(route.From_Latitude);
            var longitude = double.Parse(route.From_Longitude);

            var position = new Position(latitude, longitude);

            await Navigation.PushAsync(new MapStartingPoint(position));
        }
        private async void OnEndingPoint(object sender, EventArgs e)
        {

            var latitude = double.Parse(route.To_Latitude);
            var longitude = double.Parse(route.To_Longitude);

            var position = new Position(latitude, longitude);

            await Navigation.PushAsync(new MapEndingPoint(position));
        }
        private async void OnReserved(object sender, EventArgs e)
        {
            bool r = await DisplayAlert("Reservation", "Add reservation?", "Accept", "Cancel");
            if (r == true)
            {
                activityIndicator.IsRunning = true;

                var newReservation = new Reservations
                {
                    ID_Route = route.Id,
                    ID_User = currentUser.ID,
                    Id_Owner = route.Id_User
                };
               
                

                await AzureManager.AzureManagerInstance.SaveReservationAsync(newReservation);

                activityIndicator.IsRunning = false;
                cancelButton.IsVisible = true;
                reserveButton.IsVisible = false;
                LoadData();
            }
        }

        private async void OnCancelReservation(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Cancel Reservation", "Are you sure?", "Accept", "Cancel");
            if (answer)
            {
                var reservation = new Reservations { ID_Route = route.Id, ID_User = currentUser.ID, Id_Owner = route.Id_User };
                IsBusy = true;
                await AzureManager.AzureManagerInstance.DeleteReservationAsync(reservation);
                IsBusy = false;
                await DisplayAlert("Success", "Reservation Canceled", "Accept");
                await Navigation.PopAsync(true);
            }
        }

    }
}