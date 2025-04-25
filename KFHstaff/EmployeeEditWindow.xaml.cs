using System;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;

namespace KFHstaff
{
    public partial class EmployeeEditWindow : Window
    {
        // Строка подключения к базе данных
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        // Сотрудник, который редактируется (null для создания нового)
        private Employee _employee;

        public EmployeeEditWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            // Если сотрудник передан, заполняем поля для редактирования
            if (_employee != null)
            {
                TitleTextBlock.Text = "Редактирование сотрудника";
                TxtLastName.Text = _employee.Фамилия;
                TxtFirstName.Text = _employee.Имя;
                TxtLogin.Text = _employee.Login;
                TxtPassword.Text = _employee.Password;
                DpBirthDate.SelectedDate = _employee.Дата_Рождения;
                CmbGender.SelectedItem = CmbGender.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == (_employee.GenderID == 1 ? "Мужской" : "Женский"));
                TxtPhone.Text = _employee.Телефон;
                TxtEmail.Text = _employee.Email;
                CmbRole.SelectedItem = CmbRole.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _employee.Role);
            }
            else
            {
                TitleTextBlock.Text = "Создание сотрудника";
                CmbGender.SelectedIndex = 0; // По умолчанию "Мужской"
                CmbRole.SelectedIndex = 1;   // По умолчанию "User"
            }
        }

        // Обработчик кнопки "Сохранить"
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполненности обязательных полей
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Text) ||
                !DpBirthDate.SelectedDate.HasValue ||
                CmbGender.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtPhone.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                CmbRole.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query;
                    if (_employee == null) // Создание нового сотрудника
                    {
                        query = "INSERT INTO dbo.Staff (Фамилия, Имя, Login, Password, Дата_Рождения, Пол, Телефон, Email, Role) " +
                                "VALUES (@Фамилия, @Имя, @Login, @Password, @Дата_Рождения, @Пол, @Телефон, @Email, @Role)";
                    }
                    else // Обновление существующего сотрудника
                    {
                        query = "UPDATE dbo.Staff SET Фамилия = @Фамилия, Имя = @Имя, Login = @Login, Password = @Password, " +
                                "Дата_Рождения = @Дата_Рождения, Пол = @Пол, Телефон = @Телефон, Email = @Email, Role = @Role " +
                                "WHERE ID_Сотрудника = @ID";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Фамилия", TxtLastName.Text);
                        command.Parameters.AddWithValue("@Имя", TxtFirstName.Text);
                        command.Parameters.AddWithValue("@Login", TxtLogin.Text);
                        command.Parameters.AddWithValue("@Password", TxtPassword.Text);
                        command.Parameters.AddWithValue("@Дата_Рождения", DpBirthDate.SelectedDate.Value);
                        string selectedGender = (CmbGender.SelectedItem as ComboBoxItem).Content.ToString();
                        int genderId = selectedGender == "Мужской" ? 1 : 2;
                        command.Parameters.AddWithValue("@Пол", genderId);
                        command.Parameters.AddWithValue("@Телефон", TxtPhone.Text);
                        command.Parameters.AddWithValue("@Email", TxtEmail.Text);
                        command.Parameters.AddWithValue("@Role", (CmbRole.SelectedItem as ComboBoxItem).Content.ToString());
                        if (_employee != null)
                        {
                            command.Parameters.AddWithValue("@ID", _employee.ID_Сотрудника);
                        }
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show(_employee == null ? "Сотрудник успешно создан!" : "Сотрудник успешно обновлён!",
                                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка сохранения сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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