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

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для workWindow.xaml
    /// </summary>
    public partial class workWindow : Window
    {
        Employee currentUser = new Employee();
        public workWindow(Employee employee)
        {
            InitializeComponent();
            List<Page> pages = new List<Page>
            {
                new adminView(employee),
                new managerView(employee),
                new empView(employee)
            };
            currentUser = employee;
            mainFrame.Content = pages[employee.ID_Role-1];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            uControll.statusChange(currentUser.Emp_Login,2);            
            Environment.Exit(0);
        }
    }
}
