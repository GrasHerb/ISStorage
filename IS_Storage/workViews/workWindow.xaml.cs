using IS_Storage.classes;
using IS_Storage.Log_In;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для workWindow.xaml
    /// </summary>
    public partial class workWindow : Window
    {
        Employee currentUser = new Employee();
        stockEntities localCont = stockEntities.GetStockEntity();
        statusWindow a;
        userInfo uInfo;
        public workWindow(Employee employee)
        {
            InitializeComponent();
            currentUser = localCont.Employee.Where(p => p.IDEmp == employee.IDEmp).FirstOrDefault();
            a = new statusWindow("Прекращение работы", "Пожалуйста подождите");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            a.Show();
            var bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerAsync();
            
        }
        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new
                        Action(() =>
                        {
                            uControll.statusChange(currentUser.Emp_Login, 2);
                            Environment.Exit(0);
                        }));
        }
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            switch (currentUser.ID_Role)
            {
                case 1: adminView admin = new adminView(currentUser); mainFrame.Content = admin; break;
                case 2: managerView manager = new managerView(currentUser); mainFrame.Content = manager; break;
                case 3: empView emp = new empView(currentUser); mainFrame.Content = emp; break;
            }            
        }

        private void userInfoCheck(object sender, RoutedEventArgs e)
        {
            uInfo = new userInfo(currentUser);
            uInfo.ShowDialog();
            if (uInfo.DialogResult == true) 
            { 

                currentUser = uInfo.empChanged; 
            }
            localCont.SaveChanges();
        }

        private void infoBtn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Название: IS Storange\nВерсия: 1.0\nРазработчик: Асонов Г.С.\nПриложение предназнчено для учёта продукции на складских помещениях и подбор подходящих мест хранения с учётом специфики продукции.","О приложении");
        }
    }
}
