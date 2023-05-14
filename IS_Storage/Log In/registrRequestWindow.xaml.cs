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
        List<Run> qrun = new List<Run>();
        bool requestp = false;
        public registrRequestWindow()
        {
            InitializeComponent();
            qrun.AddRange(new List<Run> { req1, req2, req3 });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var registrEmp = new Employee();
            if (regLog.Text != "")
                if (regPass.Text != "")
                    if (regSecName.Text != "")
                        if (regFstName.Text != "")
                            if (regThrName.Text != "")
                                if (regPass.Text == regPass.Text)
                                    if (requestp)
                                    {
                                        registrEmp = new Employee()
                                        {
                                            Emp_Login = regLog.Text,
                                            Emp_Pass = uControll.Sha256password(regPass.Text),
                                            Full_Name = regSecName.Text + " " + regSecName.Text + " " + regThrName.Text,
                                            ID_Role = 3,
                                        };
                                        _context.Employee.Add(registrEmp);
                                        _context.SaveChanges();
                                        _context.userRequest.Add(new userRequest() { requestTypeID = 1, FullName = registrEmp.Full_Name + ": запрос на регистрацию", requestState = 0, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = registrEmp.IDEmp });
                                        _context.SaveChanges();

                                        MessageBox.Show("Заявка отправлена!\nСвяжитесь с администратором.");
                                    }
                                    else MessageBox.Show("Пароль не отвечает требованиям");
                                else MessageBox.Show("Пароли не совпадают");
                            else MessageBox.Show("Введите Отчество");
                        else MessageBox.Show("Введите Имя");
                    else MessageBox.Show("Введите Фамилия");
                else MessageBox.Show("Введите Пароль");
            else MessageBox.Show("Введите Логин");
            
        }

        private void regPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool[] passreqs = uControll.passwordReq(regPass.Text);
            for (int i = 0; i < passreqs.Length; i++)
            {
                if (passreqs[i]) qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"));
                else qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5A"));
            }
            foreach (Run run in qrun) if (run.Foreground != new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"))) return;
        requestp = true;
            reqBlock.Visibility = Visibility.Collapsed;
        }

        private void regPass_GotFocus(object sender, RoutedEventArgs e)
        {
            reqBlock.Visibility = Visibility.Visible;
        }
    }
}
