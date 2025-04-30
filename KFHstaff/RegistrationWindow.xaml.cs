using System;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;

namespace KFHstaff
{
    public partial class RegistrationWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        // Обработчик кнопки "Зарегистрироваться"
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполненности полей
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtPatronymic.Text) ||
                string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Password) ||
                !DpBirthDate.SelectedDate.HasValue ||
                CmbGender.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtPhone.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка уникальности логина
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM dbo.Staff WHERE Login = @Login";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Login", TxtLogin.Text);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Регистрация нового пользователя
                    string insertQuery = "INSERT INTO dbo.Staff (Фамилия, Имя, Отчество, Login, Password, Дата_Рождения, Пол, Телефон, Email, Role) " +
                                        "VALUES (@Фамилия, @Имя, @Отчество, @Login, @Password, @Дата_Рождения, @Пол, @Телефон, @Email, @Role)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Фамилия", TxtLastName.Text);
                        insertCommand.Parameters.AddWithValue("@Имя", TxtFirstName.Text);
                        insertCommand.Parameters.AddWithValue("@Отчество", TxtPatronymic.Text);
                        insertCommand.Parameters.AddWithValue("@Login", TxtLogin.Text);
                        insertCommand.Parameters.AddWithValue("@Password", TxtPassword.Password);
                        insertCommand.Parameters.AddWithValue("@Дата_Рождения", DpBirthDate.SelectedDate.Value);
                        string selectedGender = (CmbGender.SelectedItem as ComboBoxItem).Content.ToString();
                        int genderId = selectedGender == "Мужской" ? 1 : 2; // Предполагаем, что 1 — мужской, 2 — женский
                        insertCommand.Parameters.AddWithValue("@Пол", genderId);
                        insertCommand.Parameters.AddWithValue("@Телефон", TxtPhone.Text);
                        insertCommand.Parameters.AddWithValue("@Email", TxtEmail.Text);
                        insertCommand.Parameters.AddWithValue("@Role", "User");
                        insertCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик кнопки "Отмена"
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}