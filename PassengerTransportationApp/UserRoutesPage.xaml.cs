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
    /// Interaction logic for UserRoutesPage.xaml
    /// </summary>
    public partial class UserRoutesPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable routesTable;

        public UserRoutesPage()
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
            //ErrorLabel.Content = "";
            //AddBusModelWindow addBusModelWindow = new AddBusModelWindow(connectionString);
            //addBusModelWindow.owner = this;
            //addBusModelWindow.ShowDialog();
        }

        private void DeleteRoute_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (RoutesGrid.SelectedItems.Count != 0)
            {
                try
                {
                    DataRow dataRow = ((DataRowView)RoutesGrid.SelectedItem).Row;
                    dataRow.Delete();
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
                    adapter.Update(routesTable);
                }
                catch
                {
                    ErrorLabel.Content = "Не удалось удалить рейс, так как на него уже купили билеты";
                }
                finally
                {
                    RefreshGrid();
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какой рейс вы хотите удалить";
            }
        }
    }
}
