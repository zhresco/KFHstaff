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
        private string employeeName;

        public GeneralWindow(string employeeName)
        {
            InitializeComponent();
            this.employeeName = employeeName;
            this.Title = $"Добро пожаловать, {employeeName}!";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Authorization authWindow = new Authorization();
            authWindow.Show();
            this.Close();
        }
    }
}
