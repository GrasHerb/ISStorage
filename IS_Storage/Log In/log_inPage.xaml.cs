using IS_Storage.classes;
using IS_Storage.Log_In;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IS_Storage
{
    /// <summary>
    /// Логика взаимодействия для log_inPage.xaml
    /// </summary>
    public partial class log_inPage : Page
    {
        stockEntities localCont = stockEntities.GetStockEntity();
        MainWindow window = (MainWindow)Application.Current.MainWindow;
        public log_inPage()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();
            if (txtLog.Text != "" &&
                txtPass.Visibility == Visibility.Visible
                ?
                txtPass.Password != ""
                :
                txtPassVisible.Text != ""
                )
                switch (uControll.passwCheck(txtPass.Password, txtLog.Text)) 
                { 
                    case 0:
                        uControll.statusChange(txtLog.Text,1);                        
                        window.pageChange(2, txtLog.Text);
                    break;
                    case 1:
                        MessageBox.Show("Вы были зарегистрированы администратором.\n Для продолжения работы установите пароль.");
                        passChange passwindow = new passChange(localCont.Employee.Where(p=>p.Emp_Login == txtLog.Text).FirstOrDefault());
                        if (passwindow.ShowDialog() == true)
                        {                            
                            employee = localCont.Employee.Where(p => p.Emp_Login == txtLog.Text).FirstOrDefault();
                            employee.Emp_Pass = passwindow.newEmp.Emp_Pass;
                            localCont.SaveChanges();
                            uControll.statusChange(txtLog.Text,1);
                            window.pageChange(2, txtLog.Text);
                        }
                        else
                        {
                            MessageBox.Show("Вход отменён.");
                        }
                    break;
                    case 2:
                        MessageBox.Show("Ваша учётная запись ещё не была подтверждена администратором.");
                    break;
                    case 3:
                        MessageBox.Show("Ваша учётная запись была удалена администратором.");
                    break;
                    default:

                        MessageBox.Show("Ошибка проверки пароля.");
                    break;
                }                
                else
                    MessageBox.Show("Введите логин и пароль пользователя.");
        }

        private void lblForgetPass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            window.pageChange(1);
        }

        private void visibilityCheck_Checked(object sender, RoutedEventArgs e)
        {
                if (visibilityCheck.IsChecked == true)
                {
                    txtPass.Visibility = Visibility.Collapsed;
                    txtPassVisible.Visibility = Visibility.Visible;
                    txtPassVisible.Text = txtPass.Password;
                }
                else
                {
                txtPass.Visibility = Visibility.Visible;
                txtPassVisible.Visibility = Visibility.Collapsed;
                txtPass.Password = txtPassVisible.Text;
            }
        }

        private void btnRegist_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
