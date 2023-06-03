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
        Transaction controll = null;
        List<Product> products = new List<Product>();
        List<Condition> conditions = new List<Condition>();
        List<Place> places = new List<Place>();
        public empProductWindow(Client c, Transaction transaction)
        {
            InitializeComponent();
            cl = c;
            controll = transaction;
            productsClientGrid.SelectedItem = controll.Product.Name.Contains("___") ? null:controll.Product;
            products = pControl.ProductsSearch(null,c,null,1);
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
            placeGrid.ItemsSource = stockEntities.GetStockEntityD().Place.Where(p => !p.SpecialCode.Contains("___")).ToList();
            this.amountTxt.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
        }

        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)||!Char.IsControl(e.Text,0)) e.Handled = true;
        }
        private void typeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (typeTr.SelectedIndex == 0)
            {
                productsClientGrid.ItemsSource = null;
                products = pControl.ProductsSearch(null, cl, null);
                productsClientGrid.ItemsSource = products;
            }
            else
            {
                productsClientGrid.ItemsSource = null;
                products = pControl.ProductsSearch(null, cl, null, 1);
                productsClientGrid.ItemsSource = products;
            }            
        }

        private void productsClientGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            bool found = false;
            try
            {
                placeGrid.ItemsSource = null;
                places = new List<Place>();
                foreach (Place pl in stockEntities.GetStockEntityD().Place.Where(p => !p.SpecialCode.Contains("___")).ToList())
                {
                    found = false;
                    foreach(PlaceCond pc in pl.PlaceCond)
                    {
                        foreach (Condition c in conditions)
                        {
                            if (pc.Condition == c) { found = true; break; }
                            else found = false;
                        }
                    }
                    if (found) places.Add(pl);
                }
                placeGrid.ItemsSource = places;
            }
            catch
            {
                MessageBox.Show("Не удалось найти места удовлетворяющего требованиям.");
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsClientGrid.SelectedItem!=null)
            {
                if (placeGrid.SelectedItem != null)
                {

                }
            }
            else MessageBox.Show("");
        }

    }
}
