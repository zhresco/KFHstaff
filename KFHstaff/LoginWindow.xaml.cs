using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

namespace KFHstaff
{
    public partial class Authorization : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;

        public Authorization()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text?.ToLower() ?? string.Empty;
            string password = txtPassword.Password ?? string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Введите логин и пароль!";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            string fullName = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Фамилия + ' ' + Имя FROM dbo.Staff WHERE Login = @Login AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);
                        fullName = command.ExecuteScalar()?.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                GeneralWindow generalWindow = new GeneralWindow(fullName);
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