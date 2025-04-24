using System;
using System.Collections.Generic;
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

        // Конструктор по умолчанию
        public GeneralWindow()
        {
            InitializeComponent();
        }

        // Конструктор с параметрами имени и роли
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
            MessageBox.Show("Окно отчётов пока не реализовано.", "Информация");
        }

        // Обработчик кнопки настроек
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно настроек пока не реализовано.", "Информация");
        }
    }
}