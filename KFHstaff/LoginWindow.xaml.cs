using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

namespace KFHstaff
{
    public partial class LoginWindow : Window
    {
        // Строка подключения к базе данных
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Получение логина и пароля из полей ввода
            string login = txtLogin.Text?.ToLower() ?? string.Empty;
            string password = txtPassword.Password ?? string.Empty;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Введите логин и пароль!";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            string fullName = null;
            string role = null;
            // Подключение к базе данных и выполнение запроса
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Фамилия + ' ' + Имя AS FullName, Role FROM dbo.Staff WHERE Login = @Login AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                fullName = reader["FullName"].ToString();
                                role = reader["Role"].ToString();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Если пользователь найден, открываем GeneralWindow
            if (!string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                GeneralWindow generalWindow = new GeneralWindow(fullName, role);
                generalWindow.Show();
                this.Close();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль!";
                lblError.Visibility = Visibility.Visible;
                txtPassword.Password = "";
            }
        }
    }
}