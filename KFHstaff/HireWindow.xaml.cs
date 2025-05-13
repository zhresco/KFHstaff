using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace KFHstaff
{
    public partial class HireWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        public HireWindow()
        {
            InitializeComponent();
            CmbGender.SelectedIndex = 0;
            CmbRole.SelectedIndex = 0;
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtPatronymic.Text) ||
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
                    string query = "INSERT INTO dbo.Staff (Фамилия, Имя, Отчество, Login, Password, Дата_Рождения, Пол, Телефон, Email, Role) " +
                                   "VALUES (@Фамилия, @Имя, @Отчество, @Login, @Password, @Дата_Рождения, @Пол, @Телефон, @Email, @Role)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Фамилия", TxtLastName.Text);
                        command.Parameters.AddWithValue("@Имя", TxtFirstName.Text);
                        command.Parameters.AddWithValue("@Отчество", TxtPatronymic.Text);
                        command.Parameters.AddWithValue("@Login", TxtLogin.Text);
                        command.Parameters.AddWithValue("@Password", TxtPassword.Text);
                        command.Parameters.AddWithValue("@Дата_Рождения", DpBirthDate.SelectedDate.Value);
                        string selectedGender = (CmbGender.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();
                        int genderId = selectedGender == "Мужской" ? 1 : 2;
                        command.Parameters.AddWithValue("@Пол", genderId);
                        command.Parameters.AddWithValue("@Телефон", TxtPhone.Text);
                        command.Parameters.AddWithValue("@Email", TxtEmail.Text);
                        command.Parameters.AddWithValue("@Role", (CmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString());
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Сотрудник успешно принят на работу!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка приёма на работу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 