using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PassengerTransportationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectionString;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var logInPage = new LogInPage();
            logInPage.owner = this;
            ContentFrame.Navigate(logInPage);
        }

        private void BusModels_MouseEnter(object sender, MouseEventArgs e)
        {
            BusModelsLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void BusModels_MouseLeave(object sender, MouseEventArgs e)
        {
            BusModelsLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void BusModels_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var busModelsPage = new BusModelsPage();
            busModelsPage.connectionString = connectionString;
            ContentFrame.Navigate(busModelsPage);
        }

        private void Buses_MouseEnter(object sender, MouseEventArgs e)
        {
            BusesLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void Buses_MouseLeave(object sender, MouseEventArgs e)
        {
            BusesLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Buses_MouseDown(object sender, MouseEventArgs e)
        {
            var busesPage = new BusesPage();
            busesPage.connectionString = connectionString;
            ContentFrame.Navigate(busesPage);
        }

        private void Drivers_MouseEnter(object sender, MouseEventArgs e)
        {
            DriversLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void Drivers_MouseLeave(object sender, MouseEventArgs e)
        {
            DriversLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Drivers_MouseDown(object sender, MouseEventArgs e)
        {
            var driversPage = new DriversPage();
            driversPage.connectionString = connectionString;
            ContentFrame.Navigate(driversPage);
        }

        private void Passengers_MouseEnter(object sender, MouseEventArgs e)
        {
            PassengersLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void Passengers_MouseLeave(object sender, MouseEventArgs e)
        {
            PassengersLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Passengers_MouseDown(object sender, MouseEventArgs e)
        {
            var passengersPage = new PassengersPage();
            passengersPage.connectionString = connectionString;
            ContentFrame.Navigate(passengersPage);
        }

        private void Routes_MouseEnter(object sender, MouseEventArgs e)
        {
            RoutesLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void Routes_MouseLeave(object sender, MouseEventArgs e)
        {
            RoutesLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Routes_MouseDown(object sender, MouseEventArgs e)
        {
            var routesPage = new RoutesPage();
            routesPage.connectionString = connectionString;
            ContentFrame.Navigate(routesPage);
        }

        private void Tickets_MouseEnter(object sender, MouseEventArgs e)
        {
            TicketsLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void Tickets_MouseLeave(object sender, MouseEventArgs e)
        {
            TicketsLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void Tickets_MouseDown(object sender, MouseEventArgs e)
        {
            var ticketsPage = new TicketsPage();
            ticketsPage.connectionString = connectionString;
            ContentFrame.Navigate(ticketsPage);
        }

        private void UserRoutes_MouseEnter(object sender, MouseEventArgs e)
        {
            UserRoutesLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void UserRoutes_MouseLeave(object sender, MouseEventArgs e)
        {
            UserRoutesLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void UserRoutes_MouseDown(object sender, MouseEventArgs e)
        {
            var userRoutesPage = new UserRoutesPage();
            userRoutesPage.connectionString = connectionString;
            ContentFrame.Navigate(userRoutesPage);
        }

        private void UserTickets_MouseEnter(object sender, MouseEventArgs e)
        {
            UserTicketsLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void UserTickets_MouseLeave(object sender, MouseEventArgs e)
        {
            UserTicketsLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void UserTickets_MouseDown(object sender, MouseEventArgs e)
        {
            var userTicketsPage = new UserTicketsPage();
            userTicketsPage.connectionString = connectionString;
            ContentFrame.Navigate(userTicketsPage);
        }

        private void LogOut_MouseEnter(object sender, MouseEventArgs e)
        {
            LogOutLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void LogOut_MouseLeave(object sender, MouseEventArgs e)
        {
            LogOutLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void UserLogOut_MouseEnter(object sender, MouseEventArgs e)
        {
            UserLogOutLablel.Background = new SolidColorBrush(Colors.LightSkyBlue);
        }

        private void UserLogOut_MouseLeave(object sender, MouseEventArgs e)
        {
            UserLogOutLablel.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void LogOut_MouseDown(object sender, MouseEventArgs e)
        {
            AdminMenu.Visibility = Visibility.Hidden;
            UserMenu.Visibility = Visibility.Hidden;
            var logInPage = new LogInPage();
            logInPage.owner = this;
            ContentFrame.Navigate(logInPage);
        }
    }
}
