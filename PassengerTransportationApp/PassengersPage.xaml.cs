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
    /// Interaction logic for PassengersPage.xaml
    /// </summary>
    public partial class PassengersPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable passengersTable;

        public PassengersPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            passengersTable.Clear();
            adapter.Fill(passengersTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var expression = "SELECT * FROM PassengersView";
            var connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter(expression, connection);
            passengersTable = new DataTable();

            connection.Open();
            adapter.Fill(passengersTable);
            PassengersGrid.ItemsSource = passengersTable.DefaultView;
            connection.Close();
        }

        private void DeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (PassengersGrid.SelectedItems.Count != 0)
            {
                var passengerId = (int)((DataRowView)PassengersGrid.SelectedItem).Row["id"];
                var ticketsQuantity = (int)((DataRowView)PassengersGrid.SelectedItem).Row["tickets_quantity"];

                if (ticketsQuantity == 0)
                {
                    DataRow dataRow = ((DataRowView)PassengersGrid.SelectedItem).Row;
                    dataRow.Delete();
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
                    adapter.Update(passengersTable);
                    RefreshGrid();
                }
                else
                {
                    ErrorLabel.Content = "Нельзя удалить пассажира, имеющего билеты";
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какого пассажира вы хотите удалить";
            }
        }
    }
}
