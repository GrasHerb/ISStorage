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
        public int empid { get; set; }
        asonov_KPEntities _context = asonov_KPEntities.GetStockEntity();
        Employee AdmL = new Employee();
        int type = 0;
        int roleID = 0;
        Employee registrEmp = new Employee();
        public registrRequestWindow(Employee loginAdm, int T)
        {            
            InitializeComponent();
            AdmL = loginAdm;
            type = T;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            regLog.IsReadOnly = false;
            switch (type)
            {
                default: Close(); break;
                case 1:
                    regLog.IsReadOnly = false;
                    roleID = 0;
                    switch (cmbRole.SelectedIndex)
                    {
                        case 0: roleID = 3; break;
                        case 1: roleID = 2; break;
                        case 2: roleID = 1; break;
                    }
                    try
                    {
                        if (asonov_KPEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == regLog.Text).Count() > 0) { MessageBox.Show("Логин занят"); return; }
                        if (regLog.Text.Contains("___")) { MessageBox.Show("Логин не может содержать '___'"); return; }
                    }
                    catch { }
                    if (regLog.Text != "")
                        if (regSecName.Text != "")
                            if (regFstName.Text != "")
                                if (regThrName.Text != "")
                                {
                                    registrEmp = new Employee()
                                    {
                                        Emp_Login = regLog.Text,
                                        Emp_Pass = "-",
                                        Full_Name = regSecName.Text + " " + regFstName.Text + " " + regThrName.Text,
                                        EEmail = "-",
                                        ID_Role = roleID,
                                        sysInfo = " "
                                    };
                                    if (MessageBox.Show("Создать пользователя?","Подтверждение",MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                                    _context.Employee.Add(registrEmp);
                                    _context.SaveChanges();
                                    _context.userRequest.Add(new userRequest() { requestTypeID = 2, FullName = AdmL.Full_Name + " создал учётную запись: " + registrEmp.Emp_Login, requestState = 1, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = AdmL.IDEmp });
                                    _context.SaveChanges();
                                    MessageBox.Show("Пользователь создан!");
                                }

                                else MessageBox.Show("Введите Отчество");
                            else MessageBox.Show("Введите Имя");
                        else MessageBox.Show("Введите Фамилия");
                    else MessageBox.Show("Введите Логин");
                    break;
                case 2:
                    if (empid != 0)
                    {
                        regLog.IsReadOnly = true;
                        Employee tempEmp = _context.Employee.Where(p=>p.IDEmp == empid).First();
                        switch (cmbRole.SelectedIndex)
                        {
                            case 0: roleID = 3; break;
                            case 1: roleID = 2; break;
                            case 2: roleID = 1; break;
                        }
                        try
                        {
                            if (asonov_KPEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == regLog.Text).Count() > 0) { MessageBox.Show("Логин занят"); return; }
                            if (regLog.Text.Contains("___")) { MessageBox.Show("Логин не может содержать '___'"); return; }
                        }
                        catch { }
                        if (regLog.Text != "")
                            if (regSecName.Text != "")
                                if (regFstName.Text != "")
                                    if (regThrName.Text != "")
                                    {
                                        string reqText = "Изменения";                                        
                                        tempEmp.Emp_Login = regLog.Text;
                                        if (regSecName.Text + " " + regFstName.Text + " " + regThrName.Text != tempEmp.Full_Name)
                                        {
                                            reqText += "\nФИО: " + tempEmp.Full_Name + "=>" + regSecName.Text + " " + regFstName.Text + " " + regThrName.Text;
                                            tempEmp.Full_Name = regSecName.Text + " " + regFstName.Text + " " + regThrName.Text;
                                        }
                                        if (roleID != tempEmp.ID_Role)
                                        {
                                            reqText += "\nРоль: " + asonov_KPEntities.GetStockEntity().UserRole.Where(p=>p.IDRole == tempEmp.ID_Role).FirstOrDefault().Title + "=>" + asonov_KPEntities.GetStockEntity().UserRole.Where(p => p.IDRole == roleID).FirstOrDefault().Title;
                                            tempEmp.ID_Role = roleID;
                                        }
                                        if (MessageBox.Show("Применить изменения?\n" + reqText, "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) { return; }
                                        _context.SaveChanges();
                                        _context.userRequest.Add(new userRequest() { requestTypeID = 4, FullName = AdmL.Full_Name+" ("+AdmL.Emp_Login+")"+" изменил учётную запись: " + tempEmp.Full_Name +" ("+tempEmp.Emp_Login+")\n"+reqText, requestState = 0, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = AdmL.IDEmp });
                                        _context.SaveChanges();
                                        MessageBox.Show("Пользователь создан!");
                                    }

                                    else MessageBox.Show("Введите Отчество");
                                else MessageBox.Show("Введите Имя");
                            else MessageBox.Show("Введите Фамилия");
                        else MessageBox.Show("Введите Логин");
                    }
                    else Close();
                break;
            }
            
        }

        private void reg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (empid != 0)
                {
                    Employee tempEmp = _context.Employee.Where(p => p.IDEmp == empid).First();
                    regLog.Text = tempEmp.Emp_Login;
                    regFstName.Text = tempEmp.Full_Name.Split(' ')[1];
                    regSecName.Text = tempEmp.Full_Name.Split(' ')[0];
                    regThrName.Text = tempEmp.Full_Name.Split(' ')[2];
                    regLog.IsReadOnly = true;
                    switch (tempEmp.ID_Role)
                    {
                        case 1: cmbRole.SelectedIndex = 2; break;
                        case 2: cmbRole.SelectedIndex = 1; break;
                        case 3: cmbRole.SelectedIndex = 0; break;
                    }
                }
            }
            catch { MessageBox.Show("Ошибка загрузки записи."); }
            
        }
    }
}
