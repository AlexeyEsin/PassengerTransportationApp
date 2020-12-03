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
    /// Interaction logic for AddBusModelWindow.xaml
    /// </summary>
    public partial class AddBusModelWindow : Window
    {
        public string connectionString { get; set; }
        public BusModelsPage owner { get; set; }

        public AddBusModelWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            string modelName = NameTextBox.Text;
            int seatsNumber;

            if (modelName == "" || SeatsNumberTextBox.Text == "")
            {
                ErrorLabel.Content = "Введите все данные";
            }
            else if (!int.TryParse(SeatsNumberTextBox.Text, out seatsNumber) || seatsNumber <= 0)
            {
                ErrorLabel.Content = "Некорректное количество мест";
            }
            else
            {
                try
                {
                    string expression = "AddBusModel";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@name", modelName));
                    command.Parameters.Add(new SqlParameter("@seats_number", seatsNumber));

                    connection.Open();

                    try
                    {
                        command.ExecuteScalar();
                        owner.RefreshGrid();
                    }
                    catch
                    {
                        ErrorLabel.Content = "Такая модель уже есть";
                    }

                    connection.Close();
                    this.Close();
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
            }
        }

        private void SeatsNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
    }
}
