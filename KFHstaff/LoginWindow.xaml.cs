using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KFHstaff
{
    public partial class Authorization : Window
    {
        private Dictionary<string, string> users = new Dictionary<string, string>
        {
            { "admin", "admin" },
            { "user", "user" }
        };

        public Authorization()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.ToLower(); 
            string password = txtPassword.Password;

            // Проверка введенных данных
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Введите логин и пароль";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            // Проверка учетных данных
            if (users.TryGetValue(login, out string correctPassword) && password == correctPassword)
            {
                
                MessageBox.Show("Авторизация успешна", "Добро пожаловать", MessageBoxButton.OK, MessageBoxImage.Information);

                
                GeneralWindow dashboard = new GeneralWindow();
                dashboard.Show();
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