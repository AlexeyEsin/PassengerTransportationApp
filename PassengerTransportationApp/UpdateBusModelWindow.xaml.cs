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
    /// Interaction logic for UpdateBusModelWindow.xaml
    /// </summary>
    public partial class UpdateBusModelWindow : Window
    {
        public BusModelsPage owner { get; set; }
        public string connectionString { get; set; }
        public int modelId { get; set; }
        public string oldName { get; set; }
        public int oldSeatsNumber { get; set; }

        public UpdateBusModelWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
                    string expression = "UpdateBusModel";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@id", modelId));
                    command.Parameters.Add(new SqlParameter("@new_name", modelName));
                    command.Parameters.Add(new SqlParameter("@new_seats_number", seatsNumber));

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
