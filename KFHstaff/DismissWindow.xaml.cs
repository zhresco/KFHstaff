using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace KFHstaff
{
    public partial class DismissWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        public DismissWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_Сотрудника, Фамилия, Имя FROM dbo.Staff WHERE Role <> 'Admin'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                ID_Сотрудника = Convert.ToInt32(reader["ID_Сотрудника"]),
                                Фамилия = reader["Фамилия"].ToString(),
                                Имя = reader["Имя"].ToString()
                            });
                        }
                    }
                    CmbEmployee.ItemsSource = employees;
                    CmbEmployee.DisplayMemberPath = "Фамилия";
                    CmbEmployee.SelectedValuePath = "ID_Сотрудника";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (CmbEmployee.SelectedItem == null || !DpDismissDate.SelectedDate.HasValue || string.IsNullOrWhiteSpace(TxtReason.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Пример: удаляем сотрудника из БД (или помечаем как уволенного)
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM dbo.Staff WHERE ID_Сотрудника = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", ((Employee)CmbEmployee.SelectedItem).ID_Сотрудника);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Сотрудник успешно уволен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка увольнения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 