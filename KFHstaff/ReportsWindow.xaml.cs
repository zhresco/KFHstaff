using System;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace KFHstaff
{
    public partial class ReportsWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["KFHstaffDBConnection"].ConnectionString;
        private string _userRole;
        private string _userName;
        private int totalEmployees;
        private double averageAge;
        private int maleCount;
        private int femaleCount;
        private int adminCount;
        private int userCount;
        private List<Employee> employees;

        public string UserRole
        {
            get => _userRole;
            set => _userRole = value;
        }

        public ReportsWindow(string userName, string role)
        {
            InitializeComponent();
            _userName = userName;
            _userRole = role;
            this.Title = $"Отчёты, {_userName} ({role})";
            LoadStatistics();
            LoadEmployeesForComboBox();
        }

        private void LoadStatistics()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) AS Total, AVG(DATEDIFF(YEAR, Дата_Рождения, GETDATE())) AS AvgAge, " +
                                   "SUM(CASE WHEN Пол = 1 THEN 1 ELSE 0 END) AS MaleCount, " +
                                   "SUM(CASE WHEN Пол = 2 THEN 1 ELSE 0 END) AS FemaleCount, " +
                                   "SUM(CASE WHEN Role = 'Admin' THEN 1 ELSE 0 END) AS AdminCount, " +
                                   "SUM(CASE WHEN Role = 'User' THEN 1 ELSE 0 END) AS UserCount " +
                                   "FROM dbo.Staff";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalEmployees = reader.GetInt32(0);
                                averageAge = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                maleCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                femaleCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                adminCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                userCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            }
                            else
                            {
                                MessageBox.Show("Нет данных для отображения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            TxtTotalEmployees.Text = $"Общее количество сотрудников: {totalEmployees}";
            TxtAverageAge.Text = $"Средний возраст: {averageAge:F1} лет";
            TxtGenderDistribution.Text = $"Мужчины: {maleCount}\nЖенщины: {femaleCount}";
            TxtRoleDistribution.Text = $"Администраторы: {adminCount}\nПользователи: {userCount}";
        }

        private void LoadEmployeesForComboBox()
        {
            employees = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT s.*, g.Название AS GenderName " +
                                   "FROM dbo.Staff s " +
                                   "LEFT JOIN dbo.Genders g ON s.Пол = g.ID_Пола";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    ID_Сотрудника = Convert.ToInt32(reader["ID_Сотрудника"]),
                                    Фамилия = reader["Фамилия"].ToString(),
                                    Имя = reader["Имя"].ToString(),
                                    Login = reader["Login"].ToString(),
                                    Дата_Рождения = reader["Дата_Рождения"] != DBNull.Value ? Convert.ToDateTime(reader["Дата_Рождения"]) : DateTime.MinValue,
                                    Пол = reader["GenderName"] != DBNull.Value ? reader["GenderName"].ToString() : "Не указан",
                                    GenderID = reader["Пол"] != DBNull.Value ? Convert.ToInt32(reader["Пол"]) : 0,
                                    Телефон = reader["Телефон"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["Role"].ToString()
                                });
                            }
                        }
                    }
                    CmbEmployees.ItemsSource = employees;
                    CmbEmployees.DisplayMemberPath = "Фамилия";
                    CmbEmployees.SelectedValuePath = "ID_Сотрудника";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnGenerateT2_Click(object sender, RoutedEventArgs e)
        {
            if (CmbEmployees.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для формирования карточки Т-2.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Employee selectedEmployee = CmbEmployees.SelectedItem as Employee;
            GenerateT2Card(selectedEmployee);
        }

        private void GenerateT2Card(Employee employee)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF файлы (*.pdf)|*.pdf",
                Title = "Сохранить личную карточку Т-2",
                FileName = $"Личная_карточка_Т2_{employee.Фамилия}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document document = new Document(PageSize.A4, 36, 36, 36, 36);
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font titleFont = new Font(baseFont, 14, Font.BOLD);
                        Font sectionFont = new Font(baseFont, 12, Font.BOLD);
                        Font normalFont = new Font(baseFont, 12);

                        Paragraph title = new Paragraph("Унифицированная форма № Т-2", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 10f
                        };
                        document.Add(title);

                        Paragraph subtitle = new Paragraph("Утверждена Постановлением Госкомстата России от 05.01.2004 № 1", normalFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 20f
                        };
                        document.Add(subtitle);

                        Paragraph cardTitle = new Paragraph("Личная карточка работника", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 20f
                        };
                        document.Add(cardTitle);

                        Paragraph section1 = new Paragraph("I. Общие сведения", sectionFont)
                        {
                            SpacingAfter = 10f
                        };
                        document.Add(section1);

                        document.Add(new Paragraph($"1. Фамилия: {employee.Фамилия}", normalFont));
                        document.Add(new Paragraph($"   Имя: {employee.Имя}", normalFont));
                        document.Add(new Paragraph($"   Отчество: Не указано", normalFont) { SpacingAfter = 10f });

                        document.Add(new Paragraph($"2. Дата рождения: {employee.Дата_Рождения:dd.MM.yyyy}", normalFont) { SpacingAfter = 10f });
                        document.Add(new Paragraph($"3. Место рождения: Не указано", normalFont) { SpacingAfter = 10f });
                        document.Add(new Paragraph($"4. Трудовой договор: Номер 1 от 01.01.2025", normalFont) { SpacingAfter = 10f });
                        document.Add(new Paragraph($"5. Знание иностранного языка: Не указано", normalFont) { SpacingAfter = 10f });
                        document.Add(new Paragraph($"6. Образование: Не указано", normalFont) { SpacingAfter = 10f });

                        document.Add(new Paragraph("Наименование образовательного учреждения: Не указано", normalFont));
                        document.Add(new Paragraph("Документ об образовании: Не указано", normalFont));
                        document.Add(new Paragraph("Квалификация по документу: Не указано", normalFont));
                        document.Add(new Paragraph("Направление или специальность: Не указано", normalFont));
                        document.Add(new Paragraph("Код по ОКСО: Не указано", normalFont) { SpacingAfter = 20f });

                        document.Add(new Paragraph($"Табельный номер: {employee.ID_Сотрудника}", normalFont));
                        document.Add(new Paragraph($"Идентификационный номер налогоплательщика: Не указано", normalFont));
                        document.Add(new Paragraph($"Номер страхового свидетельства: Не указано", normalFont));
                        document.Add(new Paragraph($"Алфавит: {employee.Фамилия[0]}", normalFont));
                        document.Add(new Paragraph($"Характер работы: Постоянная", normalFont));
                        document.Add(new Paragraph($"Вид работы: Основная", normalFont));
                        document.Add(new Paragraph($"Пол: {employee.Пол}", normalFont));
                        document.Add(new Paragraph($"Код по ОКТМО: 0301002", normalFont));
                        document.Add(new Paragraph($"Код по ОКИН: Не указано", normalFont));

                        document.Close();
                    }

                    MessageBox.Show("Личная карточка Т-2 успешно сформирована в формате PDF!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка формирования карточки Т-2: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindow generalWindow = new GeneralWindow(_userName, _userRole);
            generalWindow.Show();
            this.Close();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt",
                Title = "Сохранить отчёт",
                FileName = $"Отчёт_по_сотрудникам_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string reportContent = $"Отчёт по сотрудникам\n" +
                                          $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n\n" +
                                          $"{TxtTotalEmployees.Text}\n" +
                                          $"{TxtAverageAge.Text}\n\n" +
                                          $"{TxtGenderDistribution.Text}\n\n" +
                                          $"{TxtRoleDistribution.Text}";
                    File.WriteAllText(saveFileDialog.FileName, reportContent);
                    MessageBox.Show("Отчёт успешно экспортирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}