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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace PassengerTransportationApp
{
    /// <summary>
    /// Interaction logic for UserRoutesPage.xaml
    /// </summary>
    public partial class UserRoutesPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter = new SqlDataAdapter();
        private DataTable routesTable = new DataTable();

        public UserRoutesPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            routesTable.Clear();
            adapter.Fill(routesTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var depPointExpression = "SELECT departure_point FROM RoutesView WHERE departure_time > GETDATE() GROUP BY departure_point";
            var arrPointExpression = "SELECT arrival_point FROM RoutesView WHERE departure_time > GETDATE() GROUP BY arrival_point";
            var connection = new SqlConnection(connectionString);
            var arrPointCommand = new SqlCommand(arrPointExpression, connection);
            var depPointCommand = new SqlCommand(depPointExpression, connection);

            try
            {
                connection.Open();

                var newAdapter = new SqlDataAdapter(depPointCommand);
                var depPointTable = new DataTable();
                newAdapter.Fill(depPointTable);

                DeparturePointComboBox.ItemsSource = depPointTable.DefaultView;
                DeparturePointComboBox.DisplayMemberPath = "departure_point";

                newAdapter = new SqlDataAdapter(arrPointCommand);
                var arrPointTable = new DataTable();
                newAdapter.Fill(arrPointTable);

                ArrivalPointComboBox.ItemsSource = arrPointTable.DefaultView;
                ArrivalPointComboBox.DisplayMemberPath = "arrival_point";

                depPointCommand.Dispose();
                arrPointCommand.Dispose();
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }
        }

        private void FindRoutes_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            string departurePoint = DeparturePointComboBox.Text;
            string arrivalPoint = ArrivalPointComboBox.Text;
            DateTime departureDate;

            if (departurePoint == "" || arrivalPoint == "" || !DepartureDatePicker.SelectedDate.HasValue)
            {
                ErrorLabel.Content = "Введите все данные";
            }
            else
            {
                departureDate = DepartureDatePicker.SelectedDate.Value;

                try
                {
                    string departureDateStr = departureDate.ToString("u").Substring(0, 10);
                    var countEpression = "SELECT COUNT(*) FROM FindRoute(N'" + departurePoint + "', N'" + arrivalPoint + "', '"
                                      + departureDateStr + "')";
                    var expression = "SELECT * FROM FindRoute(N'" + departurePoint + "', N'" + arrivalPoint + "', '"
                                      + departureDateStr + "')";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(countEpression, connection);

                    connection.Open();

                    if ((int)command.ExecuteScalar() == 0)
                    {
                        ErrorLabel.Content = "Не нашлось рейсов на эту дату";
                        RoutesGrid.ItemsSource = null;
                    }
                    else
                    {
                        command.CommandText = expression;
                        adapter.SelectCommand = command;
                        RoutesGrid.ItemsSource = routesTable.DefaultView;
                        RefreshGrid();
                    }

                    command.Dispose();
                    connection.Close();
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
            }
        }

        private void BuyTicket_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (RoutesGrid.SelectedIndex < 0)
            {
                ErrorLabel.Content = "Выберите рейс, на который хотите купить билет";
            }
            else
            {
                int freePlaces = (int)((DataRowView)RoutesGrid.SelectedItem).Row["free_places"];

                if (freePlaces == 0)
                {
                    ErrorLabel.Content = "На этот рейс нет мест";
                }
                else
                {
                    int routeId = (int)((DataRowView)RoutesGrid.SelectedItem).Row["id"];

                    BuyTicketWindow buyTicketWindow = new BuyTicketWindow();
                    buyTicketWindow.connectionString = connectionString;
                    buyTicketWindow.owner = this;
                    buyTicketWindow.routeId = routeId;
                    buyTicketWindow.ShowDialog();
                }
            }
        }
    }
}
