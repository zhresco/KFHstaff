using System;
using System.Collections.Generic;
using System.Windows;

namespace KFHstaff
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            // Заглушка: пример данных
            var history = new List<dynamic>
            {
                new { EmployeeName = "Иванов И.И.", EventType = "Отпуск", Date = DateTime.Now.AddDays(-10).ToShortDateString(), Comment = "Ежегодный оплачиваемый" },
                new { EmployeeName = "Петров П.П.", EventType = "Больничный", Date = DateTime.Now.AddDays(-5).ToShortDateString(), Comment = "ОРВИ" },
                new { EmployeeName = "Сидорова А.А.", EventType = "Увольнение", Date = DateTime.Now.AddDays(-2).ToShortDateString(), Comment = "По собственному желанию" }
            };
            HistoryDataGrid.ItemsSource = history;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 