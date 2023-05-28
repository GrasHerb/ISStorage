using IS_Storage.classes;
using Microsoft.Win32;
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
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для managerView.xaml
    /// </summary>
    public partial class managerView : System.Windows.Controls.Page
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
            try
            {
                if (clientGrid.SelectedItem == null || clientGrid.SelectedItems.Count > 1) { MessageBox.Show("Выберите одного клиента."); return; }
                Client cl = (Client)clientGrid.SelectedItem;
                var a = transactionControll.ProdofClient(cl);
                if (a.Count != 0) MessageBox.Show("Клиент не может быть удалён.\n Клиент имеет продукцию на складе.");
                else
                {
                    var req = transactionControll.delClient(localCont.Client.Where(p=>p.IDClient==cl.IDClient).First(),cEmp);
                    if (req.ID_Request != -2)
                    {
                        localCont.userRequest.Add(req);
                        localCont.SaveChanges();
                        gridUpdate();
                    }
                    else MessageBox.Show("Ошибка удаления!");
                }
            }
            catch
            {

            }
        }

        private void crClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var creating = new newClientWindow(0, cEmp);
            creating.ShowDialog();
            if (creating.DialogResult == true) gridUpdate();
        }

        private void chUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var changing = new newClientWindow(1, cEmp);
            if (clientGrid.SelectedItem == null || clientGrid.SelectedItems.Count > 1) { MessageBox.Show("Выберите одного клиента."); return; }
            changing.clientChange = (Client)clientGrid.SelectedItem;
            if (changing.DialogResult == true) gridUpdate();
        }

        private void exportBtn(object sender, RoutedEventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog();            
            if (a.ShowDialog() == true)
            {
                switch (expFormatCmb.SelectedIndex)
                {
                    case 0: break;
                    case 1: break;
                    case 2: break;
                }
            }
        }

        private void importBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog();
        }
    }
}
