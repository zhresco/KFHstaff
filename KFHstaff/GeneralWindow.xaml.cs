using System.Windows;

namespace KFHstaff
{
    public partial class GeneralWindow : Window
    {
        // Поле для хранения роли пользователя
        private string _userRole;

        // Свойство для привязки роли в XAML
        public string UserRole
        {
            get => _userRole;
            set => _userRole = value;
        }

        public GeneralWindow()
        {
            InitializeComponent();
        }

        public GeneralWindow(string fullName, string role)
        {
            InitializeComponent();
            _userRole = role;
            this.Title = $"Добро пожаловать, {fullName}! ({role})";
        }

        // Обработчик кнопки выхода
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        // Обработчик кнопки управления сотрудниками
        private void BtnManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManagementWindow empWindow = new EmployeeManagementWindow(this.Title.Split(',')[1].Trim().Split('(')[0].Trim(), _userRole);
            empWindow.Show();
            this.Close();
        }

        // Обработчик кнопки отчётов
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow(this.Title.Split(',')[1].Trim().Split('(')[0].Trim(), _userRole);
            reportsWindow.Show();
            this.Close();
        }

        // Обработчик кнопки настроек
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно настроек пока не реализовано.", "Информация");
        }
    }
}