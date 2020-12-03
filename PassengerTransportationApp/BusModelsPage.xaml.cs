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
    /// Interaction logic for BusModelsPage.xaml
    /// </summary>
    public partial class BusModelsPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable busModelsTable;

        public BusModelsPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            busModelsTable.Clear();
            adapter.Fill(busModelsTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var expression = "SELECT * FROM BusModelsView";
            var connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter(expression, connection);
            busModelsTable = new DataTable();

            connection.Open();
            adapter.Fill(busModelsTable);
            BusModelsGrid.ItemsSource = busModelsTable.DefaultView;
            connection.Close();
        }

        private void AddModel_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";
            AddBusModelWindow addBusModelWindow = new AddBusModelWindow();
            addBusModelWindow.connectionString = connectionString;
            addBusModelWindow.owner = this;
            addBusModelWindow.ShowDialog();
        }

        private void DeleteModel_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (BusModelsGrid.SelectedItems.Count != 0)
            {
                try
                {
                    DataRow dataRow = ((DataRowView)BusModelsGrid.SelectedItem).Row;
                    dataRow.Delete();
                    SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
                    adapter.Update(busModelsTable);
                }
                catch
                {
                    ErrorLabel.Content = "Не удалось удалить модель автобуса, так как на неё есть ссылки";
                }
                finally
                {
                    RefreshGrid();
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите, какую модель вы хотите удалить";
            }
        }

        private void UpdateModel_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (BusModelsGrid.SelectedItems.Count != 0)
            {
                var modelId = (int)((DataRowView)BusModelsGrid.SelectedItem).Row["id"];
                var name = (string)((DataRowView)BusModelsGrid.SelectedItem).Row["name"];
                var seats_number = (int)((DataRowView)BusModelsGrid.SelectedItem).Row["seats_number"];

                var updateBusModelWindow = new UpdateBusModelWindow();
                updateBusModelWindow.connectionString = connectionString;
                updateBusModelWindow.modelId = modelId;
                updateBusModelWindow.oldName = name;
                updateBusModelWindow.oldSeatsNumber = seats_number;
                updateBusModelWindow.owner = this;
                updateBusModelWindow.ShowDialog();
            }
            else
            {
                ErrorLabel.Content = "Выберите, какую модель вы хотите редактировать";
            }
        }
    }
}
