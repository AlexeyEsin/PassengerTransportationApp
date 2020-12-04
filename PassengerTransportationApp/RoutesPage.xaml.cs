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
    /// Interaction logic for RoutesPage.xaml
    /// </summary>
    public partial class RoutesPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable routesTable;

        public RoutesPage()
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
            var expression = "SELECT * FROM RoutesView";
            var connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter(expression, connection);
            routesTable = new DataTable();

            connection.Open();
            adapter.Fill(routesTable);
            RoutesGrid.ItemsSource = routesTable.DefaultView;
            connection.Close();
        }

        private void AddRoute_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            AddRouteWindow addRouteWindow = new AddRouteWindow();
            addRouteWindow.connectionString = connectionString;
            addRouteWindow.owner = this;
            addRouteWindow.ShowDialog();
        }

        private void DeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (RoutesGrid.SelectedItems.Count != 0)
            {
                try
                {
                    var routeId = (int)((DataRowView)RoutesGrid.SelectedItem).Row["id"];
                    var expression = "DeleteRoute";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@route_id", routeId));

                    connection.Open();

                    try
                    {
                        command.ExecuteScalar();
                    }
                    catch
                    {
                        ErrorLabel.Content = "Не удалось удалить рейс, так как на него уже купили билеты";
                    }

                    command.Dispose();
                    connection.Close();
                    RefreshGrid();
                }
                catch
                {
                    ErrorLabel.Content = "Произошла ошибка соединения";
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какой рейс вы хотите удалить из базы";
            }
        }

        private void UpdateRoute_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (RoutesGrid.SelectedItems.Count != 0)
            {
                try
                {
                    var routeId = (int)((DataRowView)RoutesGrid.SelectedItem).Row["id"];
                    var expression = "SELECT COUNT(*) FROM TicketsView WHERE route_id = " + routeId;
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);

                    connection.Open();
                    var result = (int)command.ExecuteScalar();

                    if (result > 0)
                    {
                        ErrorLabel.Content = "Невозможно отредактировать рейс, так как на него уже купили билеты";
                    }
                    else
                    {
                        UpdateRouteWindow updateRouteWindow = new UpdateRouteWindow();
                        updateRouteWindow.connectionString = connectionString;
                        updateRouteWindow.routeId = routeId;
                        updateRouteWindow.oldDeparturePoint = ((DataRowView)RoutesGrid.SelectedItem).Row["departure_point"].ToString();
                        updateRouteWindow.oldArrivalPoint = ((DataRowView)RoutesGrid.SelectedItem).Row["arrival_point"].ToString();
                        updateRouteWindow.oldPrice = Convert.ToInt32(((DataRowView)RoutesGrid.SelectedItem).Row["price"]);
                        updateRouteWindow.owner = this;
                        updateRouteWindow.ShowDialog();
                    }

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
                ErrorLabel.Content = "Выберите, какой рейс вы хотите отредактировать";
            }
        }
    }
}
