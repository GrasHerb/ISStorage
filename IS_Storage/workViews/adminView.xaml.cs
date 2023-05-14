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
        public adminView(Employee curEmployee)
        {
            InitializeComponent();
            uListGrid.ItemsSource = userCollection;
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
                userCollection = userInList.listConvert(stockEntities.GetStockEntity().Employee.Where(p=>p.OStatus).ToList());
            }
            else userCollection = userInList.listConvert(stockEntities.GetStockEntity().Employee.ToList());
        }
    }
}
