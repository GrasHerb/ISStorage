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
using System.Windows.Shapes;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для userInfo.xaml
    /// </summary>
    public partial class userInfo : Window
    {
        asonov_KPEntities localCont = asonov_KPEntities.GetStockEntity();
        Employee empChanges = new Employee();
        public Employee empChanged { get; set; } = new Employee();
        passChange a;
        public userInfo(Employee emp)
        {
            InitializeComponent();
            empChanges = emp;
            a = new passChange(empChanges);
            txtLog.Text = emp.Emp_Login;
            txtFstName.Text = emp.Full_Name.Split(' ')[1];
            txtSecName.Text = emp.Full_Name.Split(' ')[0];
            txtThrName.Text = emp.Full_Name.Split(' ')[2];
            txtMail.Text = emp.EEmail;
        }

        private void passChangeBtn(object sender, RoutedEventArgs e)
        {
            a.ShowDialog();
            if (a.DialogResult == true) { empChanges.Emp_Pass = a.newEmp.Emp_Pass; }            
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void applyBtn(object sender, RoutedEventArgs e)
        {
            string reqText = "Изменения";
            if (txtLog.Text != "" && txtMail.Text != "" && txtFstName.Text != "" && txtSecName.Text != "" && txtThrName.Text != "")
            {
                if (txtLog.Text!=empChanges.Emp_Login)
                {
                    if (asonov_KPEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == txtLog.Text).Count() > 0) { MessageBox.Show("Логин занят"); return; }
                    if (txtLog.Text.Contains("___")) { MessageBox.Show("Логин не может содержать '___'"); return; }
                    reqText += "\nЛогин: " + empChanges.Emp_Login +"=>"+txtLog.Text;
                    empChanges.Emp_Login = txtLog.Text;
                }
                if (a.DialogResult == true)
                {
                    reqText += "\nИзменён пароль";
                }
                if (txtMail.Text != empChanges.EEmail)
                {
                    reqText += "\nЭлектронная почта: " + empChanges.EEmail + "=>" + txtMail.Text;
                    empChanges.EEmail = txtMail.Text;
                }
                if (txtSecName.Text+" "+txtFstName.Text + " "+ txtThrName.Text != empChanges.Full_Name)
                {
                    reqText += "\nФИО: " + empChanges.Full_Name + "=>" + txtSecName.Text + " " + txtFstName.Text + " " + txtThrName.Text;
                    empChanges.Full_Name = txtSecName.Text + " " + txtFstName.Text + " " + txtThrName.Text;
                }
                if (MessageBox.Show("Применить изменения?\n" + reqText,"Изменения учётной записи", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    empChanged = empChanges;
                    DialogResult = true;
                }
            }
            else MessageBox.Show("Заполните все поля!");
        }

        private void txtKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
