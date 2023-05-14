using IS_Storage.classes;
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

namespace IS_Storage.Log_In
{
    /// <summary>
    /// Логика взаимодействия для passChange.xaml
    /// </summary>   
    public partial class passChange : Window
    {
        List<Run> qrun = new List<Run>();
        bool requestp = false;
        public Employee newEmp { get; set; } = new Employee();
        public passChange(Employee employee)
        {
            InitializeComponent();
            newEmp = employee;
            qrun.AddRange(new List<Run> { req1, req2, req3 });
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (newPassTxt.Password != "" || newPassTxtVis.Text != "")
            {
                if (visiblePass.IsChecked == true ? 
                    newPassTxt.Password == newPassTxtRep.Password 
                    :
                    newPassTxtVis.Text == newPassTxtVis.Text)
                {
                    if (requestp)
                    {
                        switch (visiblePass.IsChecked)
                        {
                            case true:
                                newEmp.Emp_Pass = uControll.Sha256password(newPassTxtVis.Text);
                                break;
                            case false:
                                newEmp.Emp_Pass = uControll.Sha256password(newPassTxt.Password);
                                break;
                        }
                        MessageBox.Show("Пароль изменён.");
                        this.DialogResult = true;
                    }
                    else MessageBox.Show("Выполните требования к паролю.");
                }
                else MessageBox.Show("Пароли должны совпадать");                
            }
            else MessageBox.Show("Введите новый пароль");
        }

        private void visiblePass_Checked(object sender, RoutedEventArgs e)
        {
            if (visiblePass.IsChecked == true)
            {
                newPassTxt.Visibility = Visibility.Collapsed;
                newPassTxtRep.Visibility = Visibility.Collapsed;
                newPassTxtVis.Visibility = Visibility.Visible;
                newPassTxtRepVis.Visibility = Visibility.Visible;
                newPassTxtVis.Text = newPassTxt.Password;
                newPassTxtRepVis.Text = newPassTxtRep.Password;
            }
            else
            {
                newPassTxt.Visibility = Visibility.Visible;
                newPassTxtRep.Visibility = Visibility.Visible;
                newPassTxtVis.Visibility = Visibility.Collapsed;
                newPassTxtRepVis.Visibility = Visibility.Collapsed;
                newPassTxt.Password = newPassTxtVis.Text;
                newPassTxtRep.Password = newPassTxtRepVis.Text;
            }
        }

        private void newPassTxt_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool[] passreqs = uControll.passwordReq(newPassTxt.Password);
            for (int i = 0; i < passreqs.Length; i++)
            {
                if (passreqs[i]) qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"));
                else qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5A"));
            }
            foreach (Run run in qrun) if (run.Foreground != new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"))) return;
            requestp = true;
            reqBlock.Visibility = Visibility.Collapsed;
        }

        private void newPassTxtVis_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool[] passreqs = uControll.passwordReq(newPassTxt.Password);
            for (int i = 0; i < passreqs.Length; i++)
            {
                if (passreqs[i]) qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"));
                else qrun[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5A"));
            }
            foreach (Run run in qrun) if (run.Foreground != new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75FF5A"))) return;
            requestp = true;
            reqBlock.Visibility = Visibility.Collapsed;
        }
    }
}
