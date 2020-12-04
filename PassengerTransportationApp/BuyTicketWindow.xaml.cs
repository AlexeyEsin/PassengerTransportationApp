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
using System.Text.RegularExpressions;

namespace PassengerTransportationApp
{
    /// <summary>
    /// Interaction logic for BuyTicketWindow.xaml
    /// </summary>
    public partial class BuyTicketWindow : Window
    {
        public string connectionString { get; set; }
        public UserRoutesPage owner { get; set; }
        public int routeId { get; set; }

        public BuyTicketWindow()
        {
            InitializeComponent();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string middleName = MiddleNameTextBox.Text;
            string pasportStr = PassportTextBox.Text;
            string seatNumberStr = SeatNumberTextBox.Text;
            string arrivalPoint = ArrivalPointTextBox.Text;

            var passportPattern = @"^\d{10}$";

            if (lastName == "" || firstName == "" || pasportStr == "" || seatNumberStr == "" || !BirthDatePicker.SelectedDate.HasValue)
            {
                ErrorLabel.Content = "Введите все обязательные данные";
            }
            else if (!Regex.IsMatch(pasportStr, passportPattern))
            {
                ErrorLabel.Content = "Неверный формат паспорта";
            }
            else
            {
                string birthDate = BirthDatePicker.SelectedDate.Value.ToString("u").Substring(0, 10);
                long passportNumber = long.Parse(pasportStr);
                int seatNumber = int.Parse(seatNumberStr);

                try
                {
                    var checkAgeExpression = "SELECT dbo.CheckAge('" + birthDate + "')";
                    var checkSeatNumberExpression = "SELECT dbo.CheckSeatNumber(" + seatNumber + ", " + routeId + ")";
                    var connection = new SqlConnection(connectionString);
                    var checkAgeCommand = new SqlCommand(checkAgeExpression, connection);
                    var checkSeatNumberCommand = new SqlCommand(checkSeatNumberExpression, connection);

                    connection.Open();
                    bool isMoreThan14 = (bool)checkAgeCommand.ExecuteScalar();
                    bool isCorrectSeatNumber = (bool)checkSeatNumberCommand.ExecuteScalar();

                    if (!isMoreThan14)
                    {

                        ErrorLabel.Content = "Пассажиру должно быть 14 или более лет";
                    }
                    else if (!isCorrectSeatNumber)
                    {
                        ErrorLabel.Content = "Это место в автобусе уже занято или его нет";
                    }
                    else
                    {
                        string addPassengerExpression = $"EXECUTE AddPassenger @first_name = '{firstName}'," +
                        $"@middle_name = '{middleName}',  @last_name = '{lastName}', @birth_date = '{birthDate}'," +
                        $"@passport_number = {passportNumber}; SELECT @@IDENTITY;";
                        var addPassengerCommand = new SqlCommand(addPassengerExpression, connection);

                        int passengerId = Convert.ToInt32(addPassengerCommand.ExecuteScalar());

                        string maxTicketNumberExpression = "SELECT MAX(ticket_number) FROM TicketsView";
                        var maxTicketNumberCommand = new SqlCommand(maxTicketNumberExpression, connection);
                        int newTicketNumber = (int)maxTicketNumberCommand.ExecuteScalar() + 1;

                        string addTicketExpression = "AddTicket";
                        var addTicketCommand = new SqlCommand(addTicketExpression, connection);
                        addTicketCommand.CommandType = CommandType.StoredProcedure;
                        addTicketCommand.Parameters.Add(new SqlParameter("@passenger_id", passengerId));
                        addTicketCommand.Parameters.Add(new SqlParameter("@route_id", routeId));
                        addTicketCommand.Parameters.Add(new SqlParameter("@ticket_number", newTicketNumber));
                        addTicketCommand.Parameters.Add(new SqlParameter("@place_number", seatNumber));
                        addTicketCommand.Parameters.Add(new SqlParameter("@final_arrival_point", arrivalPoint));

                        addTicketCommand.ExecuteScalar();
                        owner.RefreshGrid();

                        connection.Close();
                        this.Close();
                    }
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
            }
        }

        private void SeatNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void Passport_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
    }
}
