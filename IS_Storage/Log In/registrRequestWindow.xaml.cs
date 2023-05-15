using IS_Storage.classes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IS_Storage.Log_In
{
    /// <summary>
    /// Логика взаимодействия для registrRequestWindow.xaml
    /// </summary>
    public partial class registrRequestWindow : Window
    {
        stockEntities _context = stockEntities.GetStockEntity();
        Employee AdmL = new Employee();
        public registrRequestWindow(Employee loginAdm)
        {
            InitializeComponent();
            AdmL = loginAdm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int roleID = 0;
            var registrEmp = new Employee();
            switch (cmbRole.SelectedIndex)
            {
                case 0: roleID = 3; break;
                case 1: roleID = 2; break;
                case 2: roleID = 1; break;
            }
            if (regLog.Text != "")
                if (regSecName.Text != "")
                    if (regFstName.Text != "")
                        if (regThrName.Text != "")
                        { registrEmp = new Employee()
                        {
                            Emp_Login = regLog.Text,
                            Emp_Pass = "-",
                            Full_Name = regSecName.Text + " " + regFstName.Text + " " + regThrName.Text,
                            ID_Role = roleID,
                            sysInfo = " "
                        };
                            _context.Employee.Add(registrEmp);
                            _context.SaveChanges();
                            _context.userRequest.Add(new userRequest() { requestTypeID = 2, FullName = AdmL.Full_Name + " создал учётную запись: " + registrEmp.Emp_Login, requestState = 0, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = AdmL.IDEmp });
                            _context.SaveChanges();
                            MessageBox.Show("Пользователь создан!");
                        }
                                        
                            else MessageBox.Show("Введите Отчество");
                        else MessageBox.Show("Введите Имя");
                    else MessageBox.Show("Введите Фамилия");
            else MessageBox.Show("Введите Логин");
            
        }
    }
}
