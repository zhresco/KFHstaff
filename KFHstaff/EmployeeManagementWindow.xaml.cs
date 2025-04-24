using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace KFHstaff
{
    public partial class EmployeeManagementWindow : Window
    {
        private readonly string _fullName;
        private readonly string _role;
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;

        public EmployeeManagementWindow(string fullName, string role)
        {
            InitializeComponent();
            _fullName = fullName;
            _role = role;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_Сотрудника, Фамилия, Имя, Login, Role, Дата_Рождения, Пол, Телефон, Email FROM dbo.Staff";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        EmployeesDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindow generalWindow = new GeneralWindow(_fullName, _role);
            generalWindow.Show();
            this.Close();
        }
    }
}