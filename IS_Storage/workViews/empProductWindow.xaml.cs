using IS_Storage.classes;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для empProductWindow.xaml
    /// </summary>
    public partial class empProductWindow : Window
    {
        Client cl = new Client();
        public Transaction controll {get;set;}
        List<Product> products = new List<Product>();
        List<Condition> conditions = new List<Condition>();
        List<Place> places = new List<Place>();
        Employee cEmp = null;
        public empProductWindow(Client c, Employee employee, Transaction transaction = null)
        {
            InitializeComponent();
            cl = c;
            controll = transaction;
            cEmp = employee;
            
            products = pControl.ProductsSearch(null,c,null,1);
            placeGrid.ItemsSource = stockEntities.GetStockEntityD().Place.Where(p => !p.SpecialCode.Contains("___")).ToList();
            try
            {
                if (transaction != null)
                {
                    amountTxt.Text = (controll.Product.Amount != 0 ? controll.Amount : 0).ToString();
                    if (transaction.ID_TrTType == 1)
                    {
                        typeTr.SelectedIndex = 0;
                        productsClientGrid.ItemsSource = null;
                        products = pControl.ProductsSearch(null, c, null);
                        productsClientGrid.ItemsSource = products;
                    }
                    if (transaction.ID_TrTType == 2)
                    {
                        typeTr.SelectedIndex = 1;
                        productsClientGrid.ItemsSource = null;
                        products = pControl.ProductsSearch(null, c, null, 1);
                        productsClientGrid.ItemsSource = products;
                    }
                    productsClientGrid.SelectedIndex = controll != null ? pControl.ProductsSearch(null, cl, null, 1).FindIndex(p => p.IDProduct == controll.Product.IDProduct) : 0;
                    placeGrid.SelectedIndex = controll != null ? stockEntities.GetStockEntityD().Place.Where(p => !p.SpecialCode.Contains("___")).ToList().FindIndex(p => p.SpecialCode == controll.Place.SpecialCode) : 0;

                }
                else controll = new Transaction();
            }
            catch { }
            typeTr.SelectedIndex = 0;
            this.amountTxt.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
        }

        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)&&Char.ConvertToUtf32(e.Text,0) != Char.ConvertToUtf32(".",0)&&!Char.IsControl(e.Text,0)) e.Handled = true;
        }
        private void typeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (typeTr.SelectedIndex == 0)
            {
                productsClientGrid.ItemsSource = null;
                products = pControl.ProductsSearch(null, cl, null, 1);
                productsClientGrid.ItemsSource = products;

            }
            else
            {
                productsClientGrid.ItemsSource = null;
                products = pControl.ProductsSearch(null, cl, null);
                productsClientGrid.ItemsSource = products;

            }            
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsClientGrid.SelectedItem!=null)
            {
                if (placeGrid.SelectedItem != null)
                {
                    Place cp = (Place)placeGrid.SelectedItem;
                    Product p = (Product)productsClientGrid.SelectedItem;
                    if (amountTxt.Text != "0" || amountTxt.Text != "")
                    {
                        if (p.Amount<Convert.ToDouble(amountTxt.Text)&&typeTr.SelectedIndex == 1)
                        {
                            if (MessageBox.Show("У клиента не хватает выбранной продукции, добавить максимальное количество?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                amountTxt.Text = p.Amount.ToString();
                        }
                        if (MessageBox.Show("Сохранить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            controll = new Transaction()
                            {
                                ID_Client = cl.IDClient,
                                Amount = Convert.ToDouble(amountTxt.Text),
                                Date = controll.Date != null ? controll.Date : DateTime.Now.ToString("G"),
                                IDTransaction = controll.IDTransaction != 0 ? controll.IDTransaction : 0,
                                ID_Emp = cEmp.IDEmp,
                                ID_Place = cp.IDPlace,
                                ID_Product = p.IDProduct,
                                ID_TrTType = typeTr.SelectedIndex == 0 ? 1 : 2,
                                Product = p,
                                Place = cp
                            };
                            DialogResult = true;
                        }
                    }
                    else MessageBox.Show("Введите корректное количество!");
                }
                else MessageBox.Show("Выберите место хранения!");
            }
            else MessageBox.Show("Выберите продукцию!");
        }

        private void productsClientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsClientGrid.Items.Count == 0) return;
            int found = 0;
            try
            {
                conditions.Clear();
                var plw = (Product)productsClientGrid.SelectedItem;
                foreach (ProdCond plwC in plw.ProdCond)
                {
                    conditions.Add(plwC.Condition);
                }
                placeGrid.ItemsSource = null;
                places = new List<Place>();
                foreach (Place pl in stockEntities.GetStockEntityD().Place.Where(p => !p.SpecialCode.Contains("___")).ToList())
                {
                    found = 0;
                    foreach (PlaceCond pc in pl.PlaceCond)
                    {
                        foreach (Condition c in conditions)
                        {
                            if (pc.Condition.Title == c.Title) { found++; break; }
                        }
                    }
                    if (found == conditions.Count) places.Add(pl);
                }
                placeGrid.ItemsSource = places;
            }
            catch
            {
                MessageBox.Show("Не удалось найти места удовлетворяющего требованиям.");
            }
        }
    }
}
