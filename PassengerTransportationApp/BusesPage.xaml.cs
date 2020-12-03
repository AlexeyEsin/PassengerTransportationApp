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
    /// Interaction logic for BusesPage.xaml
    /// </summary>
    public partial class BusesPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable busesTable;

        public BusesPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            busesTable.Clear();
            adapter.Fill(busesTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = "SELECT * FROM BusesView";
                var connection = new SqlConnection(connectionString);
                adapter = new SqlDataAdapter(expression, connection);
                busesTable = new DataTable();

                connection.Open();
                adapter.Fill(busesTable);
                BusesGrid.ItemsSource = busesTable.DefaultView;
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }
        }

        private void AddBus_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            var addBusWindow = new AddBusWindow();
            addBusWindow.connectionString = connectionString;
            addBusWindow.owner = this;
            addBusWindow.ShowDialog();
        }

        private void ExploitateBus_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (BusesGrid.SelectedItems.Count != 0)
            {
                var busId = (int)((DataRowView)BusesGrid.SelectedItem).Row["id"];
                var isInExploitation = (bool)((DataRowView)BusesGrid.SelectedItem).Row["is_in_exploitation"];

                string expression;
                if (isInExploitation == false)
                {
                    expression = "ExploitateBus";
                }
                else
                {
                    expression = "UnploitateBus";
                }

                try
                {
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@bus_id", busId));

                    connection.Open();

                    command.ExecuteScalar();
                    RefreshGrid();

                    command.Dispose();
                    connection.Close();
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какой автобус вы хотите ввести или вывести из эксплуатации";
            }
        }

        private void DeleteBus_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (BusesGrid.SelectedItems.Count != 0)
            {
                try
                {
                    var id = (int)((DataRowView)BusesGrid.SelectedItem).Row["id"];
                    var expression = "DeleteBus";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@bus_id", id));

                    connection.Open();

                    try
                    {
                        command.ExecuteScalar();
                    }
                    catch
                    {
                        ErrorLabel.Content = "Не удалось удалить автобус, так как на него есть ссылки в рейсах";
                    }

                    command.Dispose();
                    connection.Close();
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
                finally
                {
                    RefreshGrid();
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какой автобус вы хотите удалить из базы";
            }
        }
    }
}
