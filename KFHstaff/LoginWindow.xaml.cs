using System.Data.SqlClient;
using System.Windows;
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
            // получение логина и пароля из полей 
            string login = txtLogin.Text?.ToLower() ?? string.Empty;
            string password = txtPassword.Password ?? string.Empty;

            // проверка на пустоту
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Введите логин и пароль!";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            string fullName = null;
            string role = null;
            // подруб к базе данных и выполнение запроса
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

        // Обработчик кнопки "Регистрация"
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            if (registrationWindow.ShowDialog() == true)
            {
                txtLogin.Text = "";
                txtPassword.Password = "";
                lblError.Visibility = Visibility.Collapsed;
            }
        }
    }
}