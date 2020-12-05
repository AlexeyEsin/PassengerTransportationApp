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
    /// Interaction logic for UserTicketsPage.xaml
    /// </summary>
    public partial class UserTicketsPage : Page
    {
        public string connectionString { get; set; }
        private SqlDataAdapter adapter;
        private DataTable ticketsTable;

        public UserTicketsPage()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            ticketsTable.Clear();
            adapter.Fill(ticketsTable);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var expression = "SELECT * FROM ThisUserTickets()";
            var connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter(expression, connection);
            ticketsTable = new DataTable();

            connection.Open();
            adapter.Fill(ticketsTable);
            TicketsGrid.ItemsSource = ticketsTable.DefaultView;
            connection.Close();
        }

        private void ReturnTicket_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Content = "";

            if (TicketsGrid.SelectedItems.Count != 0)
            {
                DateTime departureTime = (DateTime)((DataRowView)TicketsGrid.SelectedItem).Row["departure_time"];

                if (departureTime <= DateTime.Now)
                {
                    ErrorLabel.Content = "Невозможно вернуть билет после отправления автобуса";
                }
                else
                {
                    var ticketId = (int)((DataRowView)TicketsGrid.SelectedItem).Row["id"];
                    var expression = "DeleteTicket";
                    var connection = new SqlConnection(connectionString);
                    var command = new SqlCommand(expression, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ticket_id", ticketId));

                    connection.Open();

                    command.ExecuteScalar();
                    RefreshGrid();
                }
            }
            else
            {
                ErrorLabel.Content = "Выберите билет, который хотите вернуть";
            }
        }
    }
}
