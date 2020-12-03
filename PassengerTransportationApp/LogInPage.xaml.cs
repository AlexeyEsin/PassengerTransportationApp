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
using System.Data.SqlClient;

namespace PassengerTransportationApp
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public string connectionString { get; set; }
        public MainWindow owner { get; set; }

        public LogInPage()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            if (login != "")
            {
                try
                {
                    connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=PassengerTransportationDB;User Id=" + login + ";Password=" + password + ";";
                    owner.connectionString = connectionString;
                    var connection = new SqlConnection(connectionString);
                    string expression = "IF IS_MEMBER ('db_ddladmin') = 1 SELECT 1 ELSE SELECT 0";

                    connection.Open();
                    var command = new SqlCommand(expression, connection);
                    var isAdmin = command.ExecuteScalar();

                    if ((int)isAdmin == 1)
                    {
                        owner.ContentFrame.Navigate(new WelcomeAdminPage());
                        owner.AdminMenu.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var userRoutesPage = new UserRoutesPage();
                        userRoutesPage.connectionString = connectionString;
                        owner.ContentFrame.Navigate(userRoutesPage);
                        owner.UserMenu.Visibility = Visibility.Visible;
                    }

                    connection.Close();
                }
                catch
                {
                    ErrorLabel.Content = "Неверный логин или пароль";
                }
            }
        }
    }
}
