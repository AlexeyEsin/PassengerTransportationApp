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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = "SELECT * FROM PassengersView WHERE user_id = USER_ID()";
                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand(expression, connection);

                connection.Open();
                var adapter = new SqlDataAdapter(command);
                var passengersTable = new DataTable();
                adapter.Fill(passengersTable);
                PassengerComboBox.ItemsSource = passengersTable.DefaultView;
                PassengerComboBox.DisplayMemberPath = "full_name";
                PassengerComboBox.SelectedValuePath = "id";

                command.Dispose();
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }
        }

        private bool AddTicket(int pasId, int routeId, int seatNumber, string arrPoint)
        {
            bool added = false;

            try
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                var checkSeatNumberExpression = "SELECT dbo.CheckSeatNumber(" + seatNumber + ", " + routeId + ")";
                var checkSeatNumberCommand = new SqlCommand(checkSeatNumberExpression, connection);
                bool isCorrectSeatNumber = (bool)checkSeatNumberCommand.ExecuteScalar();

                if (!isCorrectSeatNumber)
                {
                    ErrorLabel.Content = "Это место в автобусе уже занято или его нет";
                }
                else
                {
                    int newTicketNumber = 1;

                    string ticketsCountExpression = "SELECT COUNT(*) FROM TicketsView";
                    var ticketsCountCommand = new SqlCommand(ticketsCountExpression, connection);
                    int ticketsCount = (int)ticketsCountCommand.ExecuteScalar();

                    if (ticketsCount != 0)
                    {
                        string maxTicketNumberExpression = "SELECT MAX(ticket_number) FROM TicketsView";
                        var maxTicketNumberCommand = new SqlCommand(maxTicketNumberExpression, connection);
                        var maxTicketNumber = maxTicketNumberCommand.ExecuteScalar();
                        newTicketNumber = (int)maxTicketNumber + 1;
                    }

                    string addTicketExpression = "AddTicket";
                    var addTicketCommand = new SqlCommand(addTicketExpression, connection);
                    addTicketCommand.CommandType = CommandType.StoredProcedure;
                    addTicketCommand.Parameters.Add(new SqlParameter("@passenger_id", pasId));
                    addTicketCommand.Parameters.Add(new SqlParameter("@route_id", routeId));
                    addTicketCommand.Parameters.Add(new SqlParameter("@ticket_number", newTicketNumber));
                    addTicketCommand.Parameters.Add(new SqlParameter("@place_number", seatNumber));
                    addTicketCommand.Parameters.Add(new SqlParameter("@final_arrival_point", arrPoint));

                    addTicketCommand.ExecuteScalar();
                    owner.RefreshGrid();
                    added = true;

                    connection.Close();

                }
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }

            return added;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            string arrivalPoint = ArrivalPointTextBox.Text;
            string seatNumberStr = SeatNumberTextBox.Text;
            int passengerId = 0;

            int seatNumber;

            if (!int.TryParse(seatNumberStr, out seatNumber) || seatNumber <= 0)
            {
                ErrorLabel.Content = "Некорректный номер места";
            }
            else
            {
                try
                {
                    var connection = new SqlConnection(connectionString);
                    connection.Open();
                    var checkSeatNumberExpression = "SELECT dbo.CheckSeatNumber(" + seatNumber + ", " + routeId + ")";
                    var checkSeatNumberCommand = new SqlCommand(checkSeatNumberExpression, connection);
                    bool isCorrectSeatNumber = (bool)checkSeatNumberCommand.ExecuteScalar();

                    if (!isCorrectSeatNumber)
                    {
                        ErrorLabel.Content = "Это место в автобусе уже занято или его нет";
                    }
                    else
                    {
                        if (ExistingPassengerRadio.IsChecked == false && NewPassengerRadio.IsChecked == false)
                        {
                            ErrorLabel.Content = "Выберите, добавить нового пассажира или существующего";
                        }
                        else if (NewPassengerRadio.IsChecked == true)
                        {
                            string lastName = LastNameTextBox.Text;
                            string firstName = FirstNameTextBox.Text;
                            string middleName = MiddleNameTextBox.Text;
                            string pasportStr = PassportTextBox.Text;

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

                                var checkAgeExpression = "SELECT dbo.CheckAge('" + birthDate + "')";
                                var checkAgeCommand = new SqlCommand(checkAgeExpression, connection);

                                bool isMoreThan14 = (bool)checkAgeCommand.ExecuteScalar();

                                if (!isMoreThan14)
                                {
                                    ErrorLabel.Content = "Пассажиру должно быть 14 или более лет";
                                }
                                else
                                {
                                    string addPassengerExpression = $"EXECUTE AddPassenger @first_name = N'{firstName}'," +
                                    $"@middle_name = N'{middleName}',  @last_name = N'{lastName}', @birth_date = '{birthDate}'," +
                                    $"@passport_number = {passportNumber}; SELECT @@IDENTITY;";
                                    var addPassengerCommand = new SqlCommand(addPassengerExpression, connection);

                                    try
                                    {
                                        passengerId = Convert.ToInt32(addPassengerCommand.ExecuteScalar());

                                        if (AddTicket(passengerId, routeId, seatNumber, arrivalPoint))
                                        {
                                            this.Close();
                                        }
                                    }
                                    catch
                                    {
                                        ErrorLabel.Content = "Вы уже добавляли пассажира с таким номером паспорта";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (PassengerComboBox.SelectedIndex < 0)
                            {
                                ErrorLabel.Content = "Выберите пассажира";
                            }
                            else
                            {
                                passengerId = (int)PassengerComboBox.SelectedValue;

                                if (AddTicket(passengerId, routeId, seatNumber, arrivalPoint))
                                {
                                    this.Close();
                                }
                            }
                        }
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
