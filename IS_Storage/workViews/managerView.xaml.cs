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
using Microsoft.Office.Interop.Word;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using System.Globalization;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для managerView.xaml
    /// </summary>
    public partial class managerView : System.Windows.Controls.Page
    {
        stockEntities localCont = stockEntities.GetStockEntity();
        Employee cEmp = new Employee();
        List<cControl> listClient;
        List<transactionControll> trInList;
        public managerView(Employee curEmployee)
        {
            InitializeComponent();
            cEmp = curEmployee;
            gridUpdate();
        }
        public void gridUpdate()
        {
            placeGrid.ItemsSource = localCont.Place.ToList();
            if (txtPl.Text != "Поиск" && txtPl.Text != "") placeGrid.ItemsSource = localCont.Place.Where(p => p.SpecialCode == txtPl.Text);
            listClient = cControl.listConvert(localCont.Client.Where(p=>!p.Name.Contains("___")).ToList());
            if (txtSearch.Text != "Поиск" && txtSearch.Text != "")
            {
                try
                {
                    listClient = listClient.Where(p => p.Name == txtSearch.Text || p.PNumber == txtSearch.Text || p.Email == txtSearch.Text).ToList();
                    listClient.OrderBy(p => p.numInGrid);
                }
                catch
                {
                    
                }
            }
            clientGrid.ItemsSource = listClient;

            trInList = transactionControll.listConvert(localCont.Transaction.ToList());
            if (txtClient.Text != "Поиск" && txtClient.Text != "")
            {
                trInList = trInList.Where(p => p.Client == txtClient.Text).ToList();
                listClient.OrderBy(p => p.numInGrid);
            }
            if (datesSearch.IsEnabled && (dateP1.SelectedDate != null && dateP2.SelectedDate != null))
            {
                trInList = trInList
                    .Where
                    (p => p.actualDate>=dateP1.SelectedDate && p.actualDate <= dateP2.SelectedDate)
                    .ToList();
            }
            transGrid.ItemsSource = trInList;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

        private void txtSearch_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
                if (txtSearch.IsKeyboardFocused)
                {
                    if (txtSearch.Text == "Поиск") txtSearch.Text = "";
                }
                else if (txtSearch.Text == "") txtSearch.Text = "Поиск";
        }
        private void txtClient_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (txtClient.IsKeyboardFocused)
            {
                if (txtClient.Text == "Поиск") txtSearch.Text = "";
            }
            else if (txtClient.Text == "") txtSearch.Text = "Поиск";

        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "Поиск"; gridUpdate(); 
        }

        private void dlClientBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clientGrid.SelectedItem == null || clientGrid.SelectedItems.Count > 1) { MessageBox.Show("Выберите одного клиента."); return; }
                var c = (cControl)clientGrid.SelectedItem;
                Client cl = localCont.Client.Where(p=>p.IDClient == c.numActual).First();
                var a = pControl.ProductsSearch(null,cl);
                if (a.Count != 0) MessageBox.Show("Клиент не может быть удалён.\n Клиент имеет продукцию на складе.");
                else
                {
                    var req = cControl.delClient(localCont.Client.Where(p=>p.IDClient==cl.IDClient).First(),cEmp);
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

        private void chClientBtn_Click(object sender, RoutedEventArgs e)
        {            
            if (clientGrid.SelectedItem == null || clientGrid.SelectedItems.Count > 1) { MessageBox.Show("Выберите одного клиента."); return; }
            var a = (cControl)clientGrid.SelectedItem;
            var cClient = new Client()
            {
                IDClient = a.numActual,
                Name = a.Name,
                Email = a.Email,
                PNumber = a.PNumber,
            };
            var changing = new newClientWindow(1, cEmp, cClient);
            changing.ShowDialog();
            if (changing.DialogResult == true) gridUpdate();
        }

        private void exportBtn(object sender, RoutedEventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog();
            switch (expFormatCmb.SelectedIndex)
            {
                case 0:
                    a.Filter = "Таблицы Excel | *.xlsb";
                    break;
                case 1: a.Filter = "Файлы json | *.json"; break;
            }
            if (a.ShowDialog() == true)
            {
                switch (expFormatCmb.SelectedIndex)
                {
                    case 0:                    
                            cControl.excelExport(localCont.Client.Where(p => !p.Name.Contains("___")).ToList(), a.FileName);
                        MessageBox.Show("Экспорт завершён!");
                        break;
                    case 1: cControl.jsonExport(localCont.Client.Where(p => !p.Name.Contains("___")).ToList(), a.FileName); MessageBox.Show("Экспорт завершён!"); break;
                }
            }
        }//экспорт

        private void importBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog();
            switch (impFormatCmb.SelectedIndex)
            {
                case 0:
                    a.Filter = "Таблицы Excel | *.xlsb";
                    break;
                case 1: a.Filter = "Файлы json | *.json"; break;
            }
            if (a.ShowDialog() == true)
            {
                switch (impFormatCmb.SelectedIndex)
                {
                    case 0:
                        try
                        {
                            var list = cControl.excelImport(localCont.Client.Where(p => !p.Name.Contains("___")).ToList(), a.FileName);
                            if (list != new List<Client>())
                            {
                                string clientsToAdd = "";
                                foreach (Client client in list)
                                {
                                    if (client.PNumber != "" && client.Email != "" && client.Name != "")
                                    {
                                        clientsToAdd += "\n" + client.Name + " " + client.Email + " " + client.PNumber;
                                        localCont.Client.Add(new Client { Name = client.Name, Email = client.Email, PNumber = client.PNumber });
                                        localCont.userRequest.Add(new userRequest { requestTypeID = 2, FullName = cEmp.Full_Name + " создал клиента: " + client.Name, requestState = 1, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = cEmp.IDEmp });
                                    }                                    
                                }
                                if (MessageBox.Show("Добавить записи: " + clientsToAdd, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    localCont.SaveChanges();
                                    gridUpdate();
                                    MessageBox.Show("Импорт завершён!");
                                }
                            }
                        }
                        catch { }
                        break;
                    case 1:
                        try
                        {
                            var list = cControl.jsonImport(localCont.Client.Where(p => !p.Name.Contains("___")).ToList(), a.FileName);
                            if (list != new List<Client>())
                            {
                                string clientsToAdd = "";
                                foreach (Client client in list)
                                {
                                    if (client.Name != "" && client.Email != "" && client.PNumber != "")
                                    {
                                        clientsToAdd += "\n" + client.Name + " " + client.Email + " " + client.PNumber;
                                        localCont.Client.Add(new Client { Name = client.Name, Email = client.Email, PNumber = client.PNumber });
                                        localCont.userRequest.Add(new userRequest { requestTypeID = 2, FullName = cEmp.Full_Name + " создал клиента: " + client.Name, requestState = 1, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = cEmp.IDEmp });
                                    }
                                }
                                if (MessageBox.Show("Добавить записи: " + clientsToAdd, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    localCont.SaveChanges();
                                    gridUpdate();
                                    MessageBox.Show("Импорт завершён!");
                                }
                            }
                        }
                        catch { }                        
                        break;
                }
            }
        }//импорт


        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

        private void btnSearchTrClient_Click(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

        private void btnReturnTrClient_Click(object sender, RoutedEventArgs e)
        {
            dateP1.SelectedDate = null;
            dateP1.DisplayDate = DateTime.Today;
            dateP2.SelectedDate = null;
            dateP2.DisplayDate = DateTime.Today;
            txtSearch.Text = "Поиск"; gridUpdate();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            datesSearch.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            datesSearch.IsEnabled = false;
            dateP1.SelectedDate = null;
            dateP1.DisplayDate = DateTime.Today;
            dateP2.SelectedDate = null;
            dateP2.DisplayDate = DateTime.Today;
        }

        private void clientProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = (cControl)clientGrid.SelectedItem;
                transactionWindow window = new transactionWindow(localCont.Client.Where(p=>p.IDClient == a.numActual).First());
                window.Show();
            }
            catch { }
        }

        private void placeInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var place = (Place)placeGrid.SelectedItem;
                transactionWindow window = new transactionWindow(null, null, place);
                window.Show();
            }
            catch { }
        }
        private void trInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaction = (transactionControll)transGrid.SelectedItem;
                transactionWindow window = new transactionWindow(null, transaction);
                window.Show();
            }
            catch { }
        }
        private void btnSearchPl_Click(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

        private void btnReturnPl_Click(object sender, RoutedEventArgs e)
        {
            txtPl.Text = "Поиск"; gridUpdate();
        }

        private void txtPl_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (txtPl.IsKeyboardFocused)
            {
                if (txtPl.Text == "Поиск") txtPl.Text = "";
            }
            else if (txtSearch.Text == "") txtPl.Text = "Поиск";
        }

        private void expTrans_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog();
            a.Filter = "Файлы Word | *.docx|Таблицы Excel | *.xlsb";
            var client = txtClient.Text!="Поиск"?txtClient.Text:null;
            string date = dateP1.SelectedDate.ToString().Split(' ')[0]+" "+ dateP2.SelectedDate.ToString().Split(' ')[0];
            if (a.ShowDialog() == true)
            {
                switch (a.FileName.Split('.')[1])
                {
                    case "docx":
                        transactionControll.wordExport(trInList, a.FileName);
                        MessageBox.Show("Экспорт завершён!");
                        break;
                    case "xlsb": transactionControll.excelExport(trInList, a.FileName, date!=""?date:null, client!=null?client:null); MessageBox.Show("Экспорт завершён!"); break;
                }
            }
        }

    }
}
