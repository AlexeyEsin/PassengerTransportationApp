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
    /// Interaction logic for AddBusWindow.xaml
    /// </summary>
    public partial class AddBusWindow : Window
    {
        public string connectionString { get; set; }
        public BusesPage owner { get; set; }

        public AddBusWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var expression = "SELECT * FROM BusModelsView";
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(expression, connection);

            connection.Open();
            var adapter = new SqlDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);
            ModelComboBox.ItemsSource = dt.DefaultView;
            ModelComboBox.DisplayMemberPath = "name";
            ModelComboBox.SelectedValuePath = "id";

            command.Dispose();
            connection.Close();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            int modelId;
            string busNumberStr = NumberTextBox.Text;
            string regNumber = RegNumberTextBox.Text.ToUpper();
            int busNumber;

            var regNumberPattern = @"^[АВЕКМНОРСТУХ]\d{3}(?<!000)[АВЕКМНОРСТУХ]{2}\d{2,3}$";

            if (busNumberStr == "" || regNumber == "" || ModelComboBox.SelectedIndex < 0)
            {
                ErrorLabel.Content = "Введите все данные";
            }
            else if (!int.TryParse(busNumberStr, out busNumber) || busNumber <= 0)
            {
                ErrorLabel.Content = "Неверный формат номера автобуса";
            }
            else if (!Regex.IsMatch(regNumber, regNumberPattern))
            {
                ErrorLabel.Content = "Неверный формат регистрационного номера";
            }
            else
            {
                modelId = (int)ModelComboBox.SelectedValue;

                try
                {
                    var expression = "AddBus";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@model_id", modelId));
                    command.Parameters.Add(new SqlParameter("@bus_number", busNumber));
                    command.Parameters.Add(new SqlParameter("@reg_number", regNumber));

                    connection.Open();

                    try
                    {
                        command.ExecuteScalar();
                        owner.RefreshGrid();
                    }
                    catch
                    {
                        ErrorLabel.Content = "Данный номер автобуса или рег. номер уже занят";
                    }

                    command.Dispose();
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
}
