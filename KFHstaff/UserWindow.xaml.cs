using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace KFHstaff
{
    public partial class UserWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        private string _userName;
        private string _userRole;
        private Employee _currentUser;

        public UserWindow(string userName, string role)
        {
            InitializeComponent();
            _userName = userName;
            _userRole = role;
            this.Title = $"Мой профиль, {userName} ({role})";
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT s.*, g.Название AS GenderName " +
                                   "FROM dbo.Staff s " +
                                   "LEFT JOIN dbo.Genders g ON s.Пол = g.ID_Пола " +
                                   "WHERE s.Фамилия + ' ' + s.Имя = @FullName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", _userName);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _currentUser = new Employee
                                {
                                    ID_Сотрудника = Convert.ToInt32(reader["ID_Сотрудника"]),
                                    Фамилия = reader["Фамилия"].ToString(),
                                    Имя = reader["Имя"].ToString(),
                                    Отчество = reader["Отчество"].ToString(),
                                    Дата_Рождения = reader["Дата_Рождения"] != DBNull.Value ? Convert.ToDateTime(reader["Дата_Рождения"]) : DateTime.MinValue,
                                    Пол = reader["GenderName"] != DBNull.Value ? reader["GenderName"].ToString() : "Не указан",
                                    GenderID = reader["Пол"] != DBNull.Value ? Convert.ToInt32(reader["Пол"]) : 0,
                                    Телефон = reader["Телефон"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    Password = reader["Password"].ToString()
                                };
                            }
                        }
                    }

                    // Заполняем поля
                    TxtLastName.Text = _currentUser.Фамилия;
                    TxtFirstName.Text = _currentUser.Имя;
                    TxtPatronymic.Text = _currentUser.Отчество;
                    DpBirthDate.SelectedDate = _currentUser.Дата_Рождения;
                    CmbGender.SelectedItem = CmbGender.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == (_currentUser.GenderID == 1 ? "Мужской" : "Женский"));
                    TxtPhone.Text = _currentUser.Телефон;
                    TxtEmail.Text = _currentUser.Email;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateFields()
        {
            // Проверка заполненности обязательных полей
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtPatronymic.Text) ||
                !DpBirthDate.SelectedDate.HasValue ||
                CmbGender.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtPhone.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Валидация email
            if (!TxtEmail.Text.Contains("@") || !TxtEmail.Text.Contains("."))
            {
                MessageBox.Show("Введите корректный email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Валидация телефона (например, должен начинаться с + и содержать 10-15 цифр)
            if (!System.Text.RegularExpressions.Regex.IsMatch(TxtPhone.Text, @"^\+\d{10,15}$"))
            {
                MessageBox.Show("Введите корректный телефон в формате +1234567890!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация полей
            if (!ValidateFields())
                return;

            // Проверка смены пароля
            bool isPasswordChanged = !string.IsNullOrWhiteSpace(TxtCurrentPassword.Password) ||
                                     !string.IsNullOrWhiteSpace(TxtNewPassword.Password) ||
                                     !string.IsNullOrWhiteSpace(TxtConfirmPassword.Password);

            if (isPasswordChanged)
            {
                // Проверяем текущий пароль
                if (TxtCurrentPassword.Password != _currentUser.Password)
                {
                    MessageBox.Show("Текущий пароль неверный!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем совпадение нового пароля и подтверждения
                if (TxtNewPassword.Password != TxtConfirmPassword.Password)
                {
                    MessageBox.Show("Новый пароль и подтверждение не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем длину нового пароля
                if (string.IsNullOrWhiteSpace(TxtNewPassword.Password) || TxtNewPassword.Password.Length < 6)
                {
                    MessageBox.Show("Новый пароль должен содержать минимум 6 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE dbo.Staff SET Фамилия = @Фамилия, Имя = @Имя, Отчество = @Отчество, " +
                                   "Дата_Рождения = @Дата_Рождения, Пол = @Пол, Телефон = @Телефон, Email = @Email" +
                                   (isPasswordChanged ? ", Password = @Password" : "") +
                                   " WHERE ID_Сотрудника = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Фамилия", TxtLastName.Text);
                        command.Parameters.AddWithValue("@Имя", TxtFirstName.Text);
                        command.Parameters.AddWithValue("@Отчество", TxtPatronymic.Text);
                        command.Parameters.AddWithValue("@Дата_Рождения", DpBirthDate.SelectedDate.Value);
                        string selectedGender = (CmbGender.SelectedItem as ComboBoxItem).Content.ToString();
                        int genderId = selectedGender == "Мужской" ? 1 : 2;
                        command.Parameters.AddWithValue("@Пол", genderId);
                        command.Parameters.AddWithValue("@Телефон", TxtPhone.Text);
                        command.Parameters.AddWithValue("@Email", TxtEmail.Text);
                        if (isPasswordChanged)
                        {
                            command.Parameters.AddWithValue("@Password", TxtNewPassword.Password);
                        }
                        command.Parameters.AddWithValue("@ID", _currentUser.ID_Сотрудника);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Данные успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    GeneralWindow generalWindow = new GeneralWindow($"{TxtLastName.Text} {TxtFirstName.Text}", _userRole);
                    generalWindow.Show();
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindow generalWindow = new GeneralWindow(_userName, _userRole);
            generalWindow.Show();
            this.Close();
        }
    }
}