using IS_Storage.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для adminView.xaml
    /// </summary>
    public partial class adminView : Page
    {
        ObservableCollection<userInList> userCollection = new ObservableCollection<userInList>();
        stockEntities localCont = stockEntities.GetStockEntity();
        Employee cEmp = new Employee();
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
                userCollection = userInList.listConvert(localCont.Employee.Where(p=>p.OStatus).ToList());
            }
            else userCollection = userInList.listConvert(localCont.Employee.ToList());
            uListGrid.ItemsSource = userCollection;
        }

        private void uListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void crUserBtn_Click(object sender, RoutedEventArgs e)
        {
            registrRequestWindow registrWindow = new registrRequestWindow(cEmp);
            if (registrRequestWindow.ShowDialog() == false) MessageBox.Show("Заявка отменена.");
        }
    }
}
