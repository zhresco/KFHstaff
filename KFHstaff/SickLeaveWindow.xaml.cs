using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace KFHstaff
{
    public partial class SickLeaveWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        public SickLeaveWindow()
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
            if (CmbEmployee.SelectedItem == null || !DpStartDate.SelectedDate.HasValue || !DpEndDate.SelectedDate.HasValue || string.IsNullOrWhiteSpace(TxtReason.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DpEndDate.SelectedDate < DpStartDate.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Сохраняем больничный (пример: в таблицу SickLeaves)
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO dbo.SickLeaves (EmployeeID, StartDate, EndDate, Reason) VALUES (@EmployeeID, @StartDate, @EndDate, @Reason)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", ((Employee)CmbEmployee.SelectedItem).ID_Сотрудника);
                        command.Parameters.AddWithValue("@StartDate", DpStartDate.SelectedDate.Value);
                        command.Parameters.AddWithValue("@EndDate", DpEndDate.SelectedDate.Value);
                        command.Parameters.AddWithValue("@Reason", TxtReason.Text);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Больничный успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка оформления больничного: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 