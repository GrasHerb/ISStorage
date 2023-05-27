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
    /// Логика взаимодействия для managerView.xaml
    /// </summary>
    public partial class managerView : Page
    {
        ObservableCollection<deluserInList> managerTable = new ObservableCollection<deluserInList>();
        stockEntities localCont = stockEntities.GetStockEntity();
        Employee cEmp = new Employee();
        public managerView(Employee curEmployee)
        {
            InitializeComponent();
            cEmp = curEmployee;
            gridUpdate();
        }
        public void gridUpdate()
        {
            var listClient = localCont.Client.ToList();
            if (txtSearch.Text != "Поиск")
            {
                listClient = listClient.Where(p => (p.Name == txtSearch.Text|| p.PNumber == txtSearch.Text || p.Email == txtSearch.Text)&&!p.Name.Contains("___")).ToList();
                listClient.OrderBy(p=>p.IDClient);
            }
            clientGrid.ItemsSource = listClient;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != "") { gridUpdate(); }
        }

        private void txtSearch_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (txtSearch.IsKeyboardFocused)
            {
                txtSearch.Text = "";
            }
            else txtSearch.Text = "Поиск";
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || txtSearch.IsKeyboardFocused)
            {
                gridUpdate();
            }
        }

        private void dlUserBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
