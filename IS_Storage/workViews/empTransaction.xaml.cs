using IS_Storage.classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для empTransaction.xaml
    /// </summary>
    public partial class empTransaction : Window
    {
        public transactionControll transaction { get; set; }
        Employee cEmp = null;
        public string actions { get; set; }
        public empTransaction(Employee employee, transactionControll t = null)
        {
            InitializeComponent();
            if(t!= null)
            {
                transaction = t;
                mainGridExtra.ItemsSource = transaction.actualList;
                clientTxt.Text = t.Client;
                clientTxt.IsEnabled = false;
            }
            else
            {
                transaction = new transactionControll() {actualList=new List<Transaction>() };
            }
            actions = "";
            cEmp = employee;
        }

        private void chTrBtn_Click(object sender, RoutedEventArgs e)
        {
            if (clientTxt.Text != "" && stockEntities.GetStockEntityD().Client.Where(p => p.Name == clientTxt.Text).Count() != 0)
            {
                if (mainGridExtra.SelectedItems.Count != 1) { MessageBox.Show("Выберите одну транзакцию на изменение!"); return; }
                empProductWindow a = new empProductWindow(stockEntities.GetStockEntityD().Client.Where(p => p.Name == clientTxt.Text).AsNoTracking().First(), cEmp, (Transaction)mainGridExtra.SelectedItem);
                a.ShowDialog();
                if (a.DialogResult == true)
                {
                    if (a.controll.IDTransaction != 0)
                    {
                        var prodAction = stockEntities.GetStockEntityD().Product.Single(p => p.IDProduct == a.controll.ID_Product).Name;
                        var placeAction = stockEntities.GetStockEntityD().Place.Single(p => p.IDPlace == a.controll.ID_Place).SpecialCode;
                        actions += "\nИзменение транзакции: id " + a.controll.IDTransaction + ", " + (a.controll.ID_TrTType == 1 ? "привоз" : "вывоз") + ", продукции " + prodAction + ", в количестве " + a.controll.Amount + ", место " + placeAction;
                        var ind = transaction.actualList.FindIndex(p => p.IDTransaction == a.controll.IDTransaction);
                        transaction.actualList[ind] = a.controll;
                    }
                    else
                    {
                        var prodAction = stockEntities.GetStockEntityD().Product.Single(p => p.IDProduct == a.controll.ID_Product).Name;
                        var placeAction = stockEntities.GetStockEntityD().Place.Single(p => p.IDPlace == a.controll.ID_Place).SpecialCode;
                        actions += "\nИзменение транзакции: id " + a.controll.IDTransaction+1 + ", " + (a.controll.ID_TrTType == 1 ? "привоз" : "вывоз") + ", продукции " + prodAction + ", в количестве " + a.controll.Amount + ", место " + placeAction;
                        var ind = transaction.actualList.FindIndex(p => p.IDTransaction == a.controll.IDTransaction);
                        transaction.actualList[ind] = a.controll;
                    }
                }
                mainGridExtra.ItemsSource = null;
                mainGridExtra.ItemsSource = transaction.actualList;
            }
            else MessageBox.Show("Клиент не найден!");
        }

        private void crTrBtn_Click(object sender, RoutedEventArgs e)
        {
            if (clientTxt.Text != "" && stockEntities.GetStockEntityD().Client.Where(p => p.Name == clientTxt.Text).Count() != 0)
            {
                empProductWindow a = new empProductWindow(stockEntities.GetStockEntityD().Client.Where(p => p.Name == clientTxt.Text).AsNoTracking().First(), cEmp);
                a.ShowDialog();
                if (a.DialogResult == true)
                {
                    var prodAction = stockEntities.GetStockEntityD().Product.Single(p => p.IDProduct == a.controll.ID_Product).Name;
                    var placeAction = stockEntities.GetStockEntityD().Place.Single(p => p.IDPlace == a.controll.ID_Place).SpecialCode;
                    transaction.actualList.Add(a.controll);                    
                    actions += "\nДобавление транзакции: " + (a.controll.ID_TrTType==1?"привоз":"вывоз") +" продукции " + prodAction + ", в количестве " + a.controll.Amount + ", место "+ placeAction;
                }

                mainGridExtra.ItemsSource = null;
                mainGridExtra.ItemsSource = transaction.actualList;
            }
            else MessageBox.Show("Клиент не найден!");
        }

        private void dlTrBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mainGridExtra.SelectedItems.Count != 1) { MessageBox.Show("Выберите одну транзакцию на изменение!"); return; }
                if (MessageBox.Show("Удалить данную транзакцию?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var a = (Transaction)mainGridExtra.SelectedItem;
                    actions = "\nУдаление транзакции: " + (a.ID_TrTType == 1 ? "привоз" : "вывоз") + " продукции " + a.ID_Product + " в количестве " + a.Amount + " с места " + a.Place.SpecialCode;
                    a.ID_TrTType = 3;
                    mainGridExtra.ItemsSource = null;
                    mainGridExtra.ItemsSource = transaction.actualList;
                }
            }
            catch
            {

            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (actions != "")
            {
                if (MessageBox.Show("Применить изменения?"+actions, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DialogResult = true;
                }
            }
        }
    }
}
