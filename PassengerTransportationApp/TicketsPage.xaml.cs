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
    /// Interaction logic for TicketsPage.xaml
    /// </summary>
    public partial class TicketsPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable ticketsTable;

        public TicketsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var expression = "SELECT * FROM TicketsView";
            var connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter(expression, connection);
            ticketsTable = new DataTable();

            connection.Open();
            adapter.Fill(ticketsTable);
            TicketsGrid.ItemsSource = ticketsTable.DefaultView;
            connection.Close();
        }
    }
}
