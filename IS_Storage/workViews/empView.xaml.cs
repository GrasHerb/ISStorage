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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для empView.xaml
    /// </summary>
    public partial class empView : Page
    {
        Employee employee = null;
        public empView(Employee curEmployee)
        {
            InitializeComponent();
            stockEntities.GetStockEntityD();
            employee = curEmployee;
            gridUpdate();
        }

        void gridUpdate()
        {
            condGrid.ItemsSource = stockEntities.GetStockEntity().Condition.Where(p => p.Title != "Удалено").ToList();
            placeGrid.ItemsSource = stockEntities.GetStockEntity().Place.Where(p => p.SpecialCode != "Удалено").ToList();
        }
        private void crPlaceBtn_Click(object sender, RoutedEventArgs e)
        {
            empPlaceWindow a = new empPlaceWindow(employee, null);
            a.ShowDialog();
            if (a.DialogResult == true) gridUpdate();
        }

        private void chPlaceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (placeGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите одно условие хранения из таблицы!"); return; }
            empPlaceWindow a = new empPlaceWindow(employee, (Place)placeGrid.SelectedItem);
            a.ShowDialog();
            if (a.DialogResult == true) gridUpdate();
        }

        private void dlPlaceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (placeGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите одно условие хранения из таблицы!"); return; }
            Place a = (Place)placeGrid.SelectedItem;
            if (MessageBox.Show("Удалить место хранения " + a.SpecialCode + "?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MessageBox.Show("Данное место будет отмечено как 'Удалено' и не станет отображаться для создания новых продуктов");
                a.SpecialCode = "Удалено";
                stockEntities.GetStockEntity().userRequest.Add(new userRequest
                {
                    requestTypeID = 4,
                    FullName = employee.Full_Name + " удалил место хранения\n" + a.SpecialCode,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G"),
                    computerName = Environment.MachineName + " " + Environment.UserName,
                    userID = employee.IDEmp
                });
                stockEntities.GetStockEntity().SaveChanges();
                gridUpdate();
            }
        }

        private void crCondBtn_Click(object sender, RoutedEventArgs e)
        {
            empCondWindow a = new empCondWindow(employee, null);
            a.ShowDialog();
            if (a.DialogResult == true) gridUpdate();
        }

        private void chCondBtn_Click(object sender, RoutedEventArgs e)
        {
            if (condGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите одно условие хранения из таблицы!"); return; }
            empCondWindow a = new empCondWindow(employee, (Condition)condGrid.SelectedItem);
            a.ShowDialog();
            if (a.DialogResult == true) gridUpdate();
        }

        private void dlCondBtn_Click(object sender, RoutedEventArgs e)
        {
            if (condGrid.SelectedItems.Count!=1) { MessageBox.Show("Выберите одно условие хранения из таблицы!"); return; }
            Condition a = (Condition)condGrid.SelectedItem;
            if(MessageBox.Show("Удалить уловие хранения "+a.Title+"?","Удаление",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MessageBox.Show("Данное условие будет отмечено как 'Удалено' и не станет отображаться в свойствах продуктов или мест хранения");
                a.Title = "Удалено";
                stockEntities.GetStockEntity().userRequest.Add(new userRequest
                {
                    requestTypeID = 4,
                    FullName = employee.Full_Name + " удалил условие хранения\n" + a.Title,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G"),
                    computerName = Environment.MachineName + " " + Environment.UserName,
                    userID = employee.IDEmp
                });
                stockEntities.GetStockEntity().SaveChanges();
                gridUpdate();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

        private void chTransBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void crTransBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dlTransBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
