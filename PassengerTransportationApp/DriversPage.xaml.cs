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
    /// Interaction logic for DriversPage.xaml
    /// </summary>
    public partial class DriversPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable driversTable;

        public DriversPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            driversTable.Clear();
            adapter.Fill(driversTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = "SELECT * FROM DriversView";
                var connection = new SqlConnection(connectionString);
                adapter = new SqlDataAdapter(expression, connection);
                driversTable = new DataTable();

                connection.Open();
                adapter.Fill(driversTable);
                DriversGrid.ItemsSource = driversTable.DefaultView;
                connection.Close();
            }
            catch
            {
                ErrorLabel.Content = "Произошла ошибка соединения";
            }
        }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            AddDriverWindow addDriverWindow = new AddDriverWindow();
            addDriverWindow.connectionString = connectionString;
            addDriverWindow.owner = this;
            addDriverWindow.ShowDialog();
        }

        private void DismissOrHireDriver_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (DriversGrid.SelectedItems.Count != 0)
            {
                var driverId = (int)((DataRowView)DriversGrid.SelectedItem).Row["id"];
                var isHired = (bool)((DataRowView)DriversGrid.SelectedItem).Row["is_hired"];

                string expression;
                if (isHired == false)
                {
                    expression = "HireDriver";
                }
                else
                {
                    expression = "DismissDriver";
                }

                try
                {
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@driver_id", driverId));

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
                ErrorLabel.Content = "Выберите, какого водителя вы хотите нанять или уволить";
            }
        }

        private void DeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (DriversGrid.SelectedItems.Count != 0)
            {
                try
                {
                    DataRow dataRow = ((DataRowView)DriversGrid.SelectedItem).Row;
                    dataRow.Delete();
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
                    adapter.Update(driversTable);
                }
                catch
                {
                    ErrorLabel.Content = "Не удалось удалить водителя, так как на него есть ссылки в рейсах";
                }
                finally
                {
                    RefreshGrid();
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какого водителя вы хотите удалить из базы";
            }
        }
    }
}
