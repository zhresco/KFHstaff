using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFHstaff
{
    public class Employee
    {
        public int ID_Сотрудника { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime Дата_Рождения { get; set; }
        public string Пол { get; set; }
        public int GenderID { get; set; }
        public string Телефон { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
