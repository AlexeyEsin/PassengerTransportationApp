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
using System.Data;
using System.Data.SqlClient;

namespace PassengerTransportationApp
{
    /// <summary>
    /// Interaction logic for AddRouteWindow.xaml
    /// </summary>
    public partial class AddRouteWindow : Window
    {
        public string connectionString { get; set; }
        public RoutesPage owner { get; set; }

        public AddRouteWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var busesExpression = "SELECT * FROM BusesView WHERE is_in_exploitation = 1";
                var driversExpression = "SELECT * FROM DriversView WHERE is_hired = 1";
                var connection = new SqlConnection(connectionString);
                var busesCommand = new SqlCommand(busesExpression, connection);
                var driversCommand = new SqlCommand(driversExpression, connection);

                DataTable busesDataTable = new DataTable();
                DataTable driversDataTable = new DataTable();

                connection.Open();

                var adapter = new SqlDataAdapter(busesCommand);
                adapter.Fill(busesDataTable);
                BusComboBox.ItemsSource = busesDataTable.DefaultView;
                BusComboBox.DisplayMemberPath = "bus_number";
                BusComboBox.SelectedValuePath = "id";

                adapter.SelectCommand = driversCommand;
                adapter.Fill(driversDataTable);
                DriverComboBox.ItemsSource = driversDataTable.DefaultView;
                DriverComboBox.DisplayMemberPath = "full_name";
                DriverComboBox.SelectedValuePath = "id";

                busesCommand.Dispose();
                driversCommand.Dispose();
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            string departurePoint = DeparturePointTextBox.Text;
            string arrivalPoint = ArrivalPointTextBox.Text;
            DateTime departureDate;
            DateTime arrivalDate;
            string departureTimeStr = DepartureTimeTextBox.Text;
            string arrivalTimeStr = ArrivalTimeTextBox.Text;
            DateTime departureTime;
            DateTime arrivalTime;
            string priceStr = PriceTextBox.Text;
            int price;
            int busId;
            int driverId;

            if (departurePoint == "" || arrivalPoint == "" || departureTimeStr == "" || arrivalTimeStr == "" || priceStr == ""
                || !DepartureDatePicker.SelectedDate.HasValue || !ArrivalDatePicker.SelectedDate.HasValue
                || BusComboBox.SelectedIndex < 0 || DriverComboBox.SelectedIndex < 0)
            {
                ErrorLabel.Content = "Введите все данные";
            }
            else if (!DateTime.TryParse(departureTimeStr, out departureTime) || !DateTime.TryParse(arrivalTimeStr, out arrivalTime))
            {
                ErrorLabel.Content = "Неверный формат времени";
            }
            else if (!int.TryParse(priceStr, out price) || price <= 0)
            {
                ErrorLabel.Content = "Некорректная стоимость";
            }
            else
            {
                busId = (int)BusComboBox.SelectedValue;
                driverId = (int)DriverComboBox.SelectedValue;
                departureDate = DepartureDatePicker.SelectedDate.Value;
                arrivalDate = ArrivalDatePicker.SelectedDate.Value;

                DateTime fullDepartureDate = departureDate.AddHours(departureTime.Hour).AddMinutes(departureTime.Minute);
                DateTime fullArrivalDate = arrivalDate.AddHours(arrivalTime.Hour).AddMinutes(arrivalTime.Minute);

                if (fullDepartureDate <= DateTime.Now)
                {
                    ErrorLabel.Content = "Дата отправления уже прошла";
                }
                else if (fullArrivalDate <= fullDepartureDate)
                {
                    ErrorLabel.Content = "Дата прибытия должна быть позже даты отправления";
                }
                else
                {
                    string fullDepartureDateStr = fullDepartureDate.ToString("u");
                    fullDepartureDateStr = fullDepartureDateStr.Remove(fullDepartureDateStr.Length - 1);
                    string fullArrivalDateStr = fullArrivalDate.ToString("u");
                    fullArrivalDateStr = fullArrivalDateStr.Remove(fullArrivalDateStr.Length - 1);

                    try
                    {
                        string expression = "AddRoute";
                        var connection = new SqlConnection(connectionString);
                        var command = new SqlCommand(expression, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@bus_id", busId));
                        command.Parameters.Add(new SqlParameter("@driver_id", driverId));
                        command.Parameters.Add(new SqlParameter("@departure_point", departurePoint));
                        command.Parameters.Add(new SqlParameter("@arrival_point", arrivalPoint));
                        command.Parameters.Add(new SqlParameter("@departure_time", fullDepartureDateStr));
                        command.Parameters.Add(new SqlParameter("@arrival_time", fullArrivalDateStr));
                        command.Parameters.Add(new SqlParameter("@price", price));

                        connection.Open();

                        command.ExecuteScalar();
                        owner.RefreshGrid();

                        connection.Close();
                        this.Close();
                    }
                    catch
                    {
                        ErrorLabel.Content = "Произошла ошибка соединения";
                    }
                }
            }
        }

        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
    }
}
