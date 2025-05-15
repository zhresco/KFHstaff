using System.Windows;

namespace KFHstaff
{
    public partial class GeneralWindow : Window
    {
        private string _userName; // Поле для хранения имени пользователя
        private string _userRole; // Поле для хранения роли пользователя

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
            _userName = fullName;
            _userRole = role;
            this.Title = $"Добро пожаловать, {fullName}! ({role})";
            DataContext = this; // Для привязки UserRole
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
            EmployeeManagementWindow empWindow = new EmployeeManagementWindow(_userName, _userRole);
            empWindow.Show();
            this.Close();
        }

        // Обработчик кнопки отчётов
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow(_userName, _userRole);
            reportsWindow.Show();
            this.Close();
        }

        // Обработчик кнопки настроек
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно настроек пока не реализовано.", "Информация");
        }

        // Обработчик кнопки "Мой профиль"
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow(_userName, _userRole);
            userWindow.Show();
            this.Close();
        }

        // Обработчик кнопки "Оформить отпуск"
        private void BtnVacation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно оформления отпуска пока не реализовано.", "Информация");
        }

        // Обработчик кнопки "Оформить больничный"
        private void BtnSickLeave_Click(object sender, RoutedEventArgs e)
        {
            SicklistWindow sicklistWindow = new SicklistWindow();
            sicklistWindow.ShowDialog();
        }

        // Обработчик кнопки "История"
        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow();
            historyWindow.ShowDialog();
        }

        // Обработчик кнопки "Принять на работу"
        private void BtnHire_Click(object sender, RoutedEventArgs e)
        {
            HireWindow hireWindow = new HireWindow();
            hireWindow.ShowDialog();
        }

        // Обработчик кнопки "Уволить сотрудника"
        private void BtnDismiss_Click(object sender, RoutedEventArgs e)
        {
            DismissWindow dismissWindow = new DismissWindow();
            dismissWindow.ShowDialog();
        }
    }
}