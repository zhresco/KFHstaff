using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace KFHstaff
{
    public partial class EmployeeManagementWindow : Window
    {
        // Строка подключения к базе данных
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        // Поле для хранения роли пользователя
        private string _userRole;
        // Поле для хранения имени пользователя
        private string _userName;

        // Свойство для привязки роли в XAML
        public string UserRole
        {
            get => _userRole;
            set => _userRole = value;
        }

        public EmployeeManagementWindow(string userName, string role)
        {
            InitializeComponent();
            _userName = userName;
            _userRole = role;
            this.Title = $"Управление сотрудниками, {userName} ({role})";
            LoadEmployees();
        }

        // Метод для загрузки сотрудников из базы данных
        private void LoadEmployees()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT s.*, g.Название AS GenderName " +
                                   "FROM dbo.Staff s " +
                                   "LEFT JOIN dbo.Genders g ON s.Пол = g.ID_Пола";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    ID_Сотрудника = Convert.ToInt32(reader["ID_Сотрудника"]),
                                    Фамилия = reader["Фамилия"].ToString(),
                                    Имя = reader["Имя"].ToString(),
                                    Login = reader["Login"].ToString(),
                                    Дата_Рождения = reader["Дата_Рождения"] != DBNull.Value ? Convert.ToDateTime(reader["Дата_Рождения"]) : DateTime.MinValue,
                                    Пол = reader["GenderName"] != DBNull.Value ? reader["GenderName"].ToString() : "Не указан",
                                    GenderID = reader["Пол"] != DBNull.Value ? Convert.ToInt32(reader["Пол"]) : 0,
                                    Телефон = reader["Телефон"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["Role"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            EmployeesDataGrid.ItemsSource = employees;
        }

        // Обработчик кнопки "Назад"
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindow generalWindow = new GeneralWindow(_userName, _userRole);
            generalWindow.Show();
            this.Close();
        }

        // Обработчик кнопки "Создать"
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            EmployeeEditWindow editWindow = new EmployeeEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        // Обработчик кнопки "Редактировать"
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = EmployeesDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            EmployeeEditWindow editWindow = new EmployeeEditWindow(selectedEmployee);
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        // Обработчик кнопки "Удалить"
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee = EmployeesDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {selectedEmployee.Фамилия} {selectedEmployee.Имя}?",
                                                      "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM dbo.Staff WHERE ID_Сотрудника = @ID";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ID", selectedEmployee.ID_Сотрудника);
                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("Сотрудник успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Ошибка удаления сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}