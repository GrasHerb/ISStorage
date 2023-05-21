using IS_Storage.classes;
using IS_Storage.Log_In;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
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
using System.Windows.Threading;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для adminView.xaml
    /// </summary>
    public partial class adminView : Page
    {
        ObservableCollection<userInList> userCollection = new ObservableCollection<userInList>();
        ObservableCollection<deluserInList> deluserCollection = new ObservableCollection<deluserInList>();
        stockEntities localCont = stockEntities.GetStockEntity();
        Employee cEmp = new Employee();
        DispatcherTimer t = new DispatcherTimer();
        public adminView(Employee curEmployee)
        {
            InitializeComponent();
            cEmp = curEmployee;            
            gridUpdate();
        }
        private void visibleOnline_Checked(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }
        public void gridUpdate()
        {            
            if (visibleOnline.IsChecked == true)
            {
                userCollection = userInList.listConvert(localCont.Employee.Where(p=>p.OStatus&&!p.Emp_Login.Contains("___")).ToList());
            }
            else userCollection = userInList.listConvert(localCont.Employee.Where(p => !p.Emp_Login.Contains("___")).ToList());
            deluserCollection = deluserInList.listConvert(localCont.Employee.ToList());
            uDelListGrid.ItemsSource = deluserCollection;
            uListGrid.ItemsSource = userCollection;
        }

        private void uListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void crUserBtn_Click(object sender, RoutedEventArgs e)
        {
            registrRequestWindow registrWindow = new registrRequestWindow(cEmp,1);
            if (registrWindow.ShowDialog() == false) MessageBox.Show("Заявка отменена.");
        }

        private void dlUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (uListGrid.SelectedItem == null || uListGrid.SelectedItems.Count>1) {MessageBox.Show("Выберите одного пользователя."); return; }
            try
            {
                userInList userforDel = (userInList)uListGrid.SelectedItem;
                userRequest code = uControll.deleteEmp(localCont.Employee.Where(p => p.IDEmp == userforDel.uNumber).FirstOrDefault(),cEmp);
                if (code.ID_Request > -1) { MessageBox.Show("Пользователь был удалён."); localCont.userRequest.Add(code); localCont.SaveChanges(); }
                switch (code.ID_Request)
                {
                    default: break;
                    case -1: MessageBox.Show("Пользователь онлайн и не может быть удалён."); break;
                    case -2: MessageBox.Show("Невозможно удалить текущую учетную запись."); break;
                    case -3: MessageBox.Show("Невозможно удалить единственную запись с правами администратора."); break;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                {
                    string a = "Object: " + validationError.Entry.Entity.ToString();
                    foreach (DbValidationError err in validationError.ValidationErrors)
                    {
                        a+="\n "+(err.ErrorMessage + "");
                    }
                    MessageBox.Show(a);
                        
                }
            }
        }

        private void chUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (uListGrid.SelectedItem == null || uListGrid.SelectedItems.Count > 1) { MessageBox.Show("Выберите одного пользователя."); return; }
            userInList userforChange = (userInList)uListGrid.SelectedItem;
            registrRequestWindow a = new registrRequestWindow(cEmp, 2) { empid = userforChange.uNumber};
            
            a.ShowDialog();
        }
    }
}
