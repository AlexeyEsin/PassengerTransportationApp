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
    /// Interaction logic for AddDriverWindow.xaml
    /// </summary>
    public partial class AddDriverWindow : Window
    {
        public string connectionString { get; set; }
        public DriversPage owner { get; set; }

        public AddDriverWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string middleName = MiddleNameTextBox.Text;

            try
            {
                var expression = "AddDriver";
                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand(expression, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@last_name", lastName));
                command.Parameters.Add(new SqlParameter("@first_name", firstName));
                command.Parameters.Add(new SqlParameter("@middle_name", middleName));

                connection.Open();

                command.ExecuteScalar();
                owner.RefreshGrid();

                command.Dispose();
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }

            this.Close();
        }
    }
}
