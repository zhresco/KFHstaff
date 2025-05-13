using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace KFHstaff
{
    public partial class VacationWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        public VacationWindow()
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
            if (CmbEmployee.SelectedItem == null || !DpStartDate.SelectedDate.HasValue || !DpEndDate.SelectedDate.HasValue || CmbType.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DpEndDate.SelectedDate < DpStartDate.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Сохраняем отпуск (пример: в таблицу Vacations)
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO dbo.Vacations (EmployeeID, StartDate, EndDate, Type) VALUES (@EmployeeID, @StartDate, @EndDate, @Type)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", ((Employee)CmbEmployee.SelectedItem).ID_Сотрудника);
                        command.Parameters.AddWithValue("@StartDate", DpStartDate.SelectedDate.Value);
                        command.Parameters.AddWithValue("@EndDate", DpEndDate.SelectedDate.Value);
                        command.Parameters.AddWithValue("@Type", ((System.Windows.Controls.ComboBoxItem)CmbType.SelectedItem).Content.ToString());
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Отпуск успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка оформления отпуска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 