using IS_Storage.classes;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для empView.xaml
    /// </summary>
    public partial class empView : System.Windows.Controls.Page
    {
        Employee employee = null;
        List<Condition> conditions = new List<Condition>();
        public empView(Employee curEmployee)
        {
            InitializeComponent();
            stockEntities.GetStockEntityD();
            employee = curEmployee;
            gridUpdate();
        }

        void gridUpdate()
        {
            condGrid.ItemsSource = stockEntities.GetStockEntity().Condition.Where(p => !p.Title.Contains("Удалено___")).ToList();
            placeGrid.ItemsSource = stockEntities.GetStockEntity().Place.Where(p => !p.SpecialCode.Contains("Удалено___")).ToList();
            transGrid.ItemsSource = transactionControll.listConvert(stockEntities.GetStockEntityD().Transaction.AsNoTracking().ToList());
            conditionsP = convertList(stockEntities.GetStockEntityD().Condition.Where(p => p.Title != "Удалено").AsNoTracking().ToList());
            condGridP.ItemsSource = null;
            condGridP.ItemsSource = conditionsP;
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
                a.SpecialCode = "Удалено___"+a.SpecialCode;
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
            if (transGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите одну транзакцию!"); return; }
            empTransaction a = new empTransaction(employee,(transactionControll)transGrid.SelectedItem);
            a.ShowDialog();
            if (a.DialogResult == true) 
            {
                stockEntities.GetStockEntity().userRequest.Add(new userRequest 
                { 
                    requestTypeID = 3, 
                    FullName = employee.Full_Name + a.actions, 
                    requestState = 1, 
                    requestTime = DateTime.Now.ToString("G"), 
                    computerName = Environment.MachineName + " " + Environment.UserName, 
                    userID = employee.IDEmp 
                });
                var trq = a.transaction.actualList;
                foreach (Transaction transaction in trq)
                {
                    if (stockEntities.GetStockEntity().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).Count() == 1)
                    {
                        var changing = stockEntities.GetStockEntity().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).First();
                        changing = transaction;
                    }
                    else
                    {
                        stockEntities.GetStockEntity().Transaction.Add(transaction);
                    }
                }
            }
        }

        private void crTransBtn_Click(object sender, RoutedEventArgs e)
        {
            empTransaction a = new empTransaction(employee);
            a.ShowDialog();
            if (a.DialogResult == true)
            {
                stockEntities.GetStockEntity().userRequest.Add(new userRequest
                {
                    requestTypeID = 2,
                    FullName = employee.Full_Name + a.actions,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G"),
                    computerName = Environment.MachineName + " " + Environment.UserName,
                    userID = employee.IDEmp
                });
                var trq = a.transaction.actualList;
                    foreach (Transaction transaction in trq)
                    {
                        if (stockEntities.GetStockEntityD().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).Count() == 1)
                        {
                            var changing = stockEntities.GetStockEntity().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).First();
                            changing = new Transaction() 
                            {                                
                                Date = DateTime.Now.ToString("G"),
                                ID_Client = transaction.ID_Client,
                                ID_TrTType = transaction.ID_TrTType,
                                ID_Emp = transaction.ID_Emp,
                                ID_Place = transaction.ID_Place,
                                ID_Product = transaction.ID_Product,
                                Amount = transaction.Amount,
                                IDTransaction = changing.IDTransaction,
                                Client = changing.Client,
                            };
                        }
                        else
                        {
                            stockEntities.GetStockEntity().Transaction.Add(new Transaction() 
                            { 
                                Date = DateTime.Now.ToString("G"),
                                ID_Client = transaction.ID_Client,
                                ID_TrTType = transaction.ID_TrTType,
                                ID_Emp = transaction.ID_Emp,
                                ID_Place = transaction.ID_Place,
                                ID_Product = transaction.ID_Product,
                                Amount = transaction.Amount
                            } );
                        }
                        stockEntities.GetStockEntity().SaveChanges();
                    }
                    pControl.amountCount();                
            }
        }

        private void dlTransBtn_Click(object sender, RoutedEventArgs e)
        {
            if (transGrid.SelectedItems.Count != 1) { MessageBox.Show("Выберите одну транзакцию!"); return; }
            if (MessageBox.Show("Удалить транзакцию?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var tr = (transactionControll)transGrid.SelectedItem;

                    foreach (Transaction transaction in tr.actualList)
                    {
                        transaction.ID_TrTType = 3;
                    }
                foreach (Transaction transaction in tr.actualList)
                {
                    if (stockEntities.GetStockEntity().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).Count() == 1)
                    {
                        var changing = stockEntities.GetStockEntity().Transaction.Where(p => p.IDTransaction == transaction.IDTransaction).First();
                        changing = transaction;
                    }
                    else
                    {
                        stockEntities.GetStockEntity().Transaction.Add(transaction);
                    }
                }

                stockEntities.GetStockEntity().SaveChanges();
                pControl.amountCount();
                stockEntities.GetStockEntity().SaveChanges();
            }
        }
        //---------------------------------------------------------------------------------------------------------
        Product prodF = null;
        List<conditionsOfProduct> conditionsP = new List<conditionsOfProduct>();
        List<conditionsOfProduct> conditionsO = new List<conditionsOfProduct>();
        private void prodAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!txtNewName.Text.Contains("___"))
            {
                if (txtNewName.Text != "" && txtNewArticle.Text != "")
                    if (stockEntities.GetStockEntityD().Product.Where(p => p.Name == txtNewName.Text).Count() == 0 && stockEntities.GetStockEntityD().Product.Where(p => p.Article == txtNewArticle.Text).Count() == 0)
                    {
                        prodF = new Product() { Amount = 0, Article = txtNewArticle.Text, Name = txtNewName.Text, UnitID = 1 };
                        string actions = "";

                        foreach (conditionsOfProduct Wcond in conditionsO)
                        {
                            stockEntities.GetStockEntity().ProdCond.Add(new ProdCond() {  ID_Product= prodF.IDProduct, ID_Condition = Wcond.number });
                            actions += "\nCвойство: " + Wcond.Title + "\n";
                        }
                            stockEntities.GetStockEntity().userRequest.Add(new userRequest()
                            {
                                requestTypeID = 2,
                                FullName = employee.Full_Name + " создал продукцию " + prodF.Name + "\n" + actions,
                                requestState = 1,
                                requestTime = DateTime.Now.ToString("G"),
                                computerName = Environment.MachineName + " " + Environment.UserName,
                                userID = employee.IDEmp
                            });

                            stockEntities.GetStockEntity().Product.Add(prodF);
                            stockEntities.GetStockEntity().SaveChanges();
                    }
                    else MessageBox.Show("Такой товар уже существует!");
                else MessageBox.Show("Заполните название и артикул продукции!");
            }
            else MessageBox.Show("Название не может содержать '___'");
        }

        private void prodChBtn_Click(object sender, RoutedEventArgs e)
        {
            if (prodF.IDProduct != 0)//изменение
            {
                int indexP = prodF.IDProduct;
                string actions = "Изменения продукции " + prodF.IDProduct + "\n";
                if (txtNewName.Text != prodF.Name && txtNewArticle.Text != prodF.Article)
                    if (stockEntities.GetStockEntityD().Product.Where(p => p.Name == txtNewName.Text&&p.Article==prodF.Article).Count() != 0)
                    { MessageBox.Show("Данный продукт существует!"); return; }
                    else
                    {
                        actions += "\nИзменен продукт: " + prodF.Name + "=>" + txtNewName.Text+" "+prodF.Article+"=>"+txtNewArticle.Text;
                        prodF.Name = txtNewName.Text;
                        prodF.Article = txtNewArticle.Text;
                    }

                foreach (conditionsOfProduct Wcond in conditionsO)//поиск новых свойств
                {
                    if (prodF.ProdCond.Where(p => Wcond.number == p.ID_Condition).Count() == 0)
                    {
                        stockEntities.GetStockEntity().PlaceCond.Add(new PlaceCond() { ID_Place = prodF.IDProduct, ID_Condition = Wcond.number });
                        actions += "\nДобавлено свойство: " + Wcond.Title + "\n";
                    }
                }
                foreach (ProdCond Pcond in prodF.ProdCond)//удаление старых свойств
                {
                    if (conditionsO.Where(p => Pcond.ID_Condition == p.number).Count() == 0)
                    {
                        stockEntities.GetStockEntity().ProdCond.Remove(Pcond);
                        actions += "\nУдалено свойство: " + Pcond.Condition.Title + "\n";
                    }
                }
                    stockEntities.GetStockEntity().userRequest.Add(new userRequest()
                    {
                        requestTypeID = 3,
                        FullName = employee.Full_Name + " изменил продукцию \n" + actions,
                        requestState = 1,
                        requestTime = DateTime.Now.ToString("G"),
                        computerName = Environment.MachineName + " " + Environment.UserName,
                        userID = employee.IDEmp
                    });
                    stockEntities.GetStockEntity().SaveChanges();
            }
        }

        private void prodDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (prodF.IDProduct != 0)
            {

                MessageBox.Show(prodF.Name + " удалено");
                prodF.Name = "Удалено" + prodF.Name;
                stockEntities.GetStockEntity().SaveChanges();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var temp = (conditionsOfProduct)condGridP.SelectedItem;
            var Cpl = conditionsP.Find(p => p.Title == temp.Title);
            if (Cpl.Chosen == " ")
            {
                Cpl.Chosen = "+";
                conditionsO.Add(Cpl);
            }
            else
            {
                Cpl.Chosen = " ";
                conditionsO.Remove(Cpl);
            }
            condGrid.ItemsSource = null;
            condGrid.ItemsSource = conditionsP;
        }
        class conditionsOfProduct
        {
            public string Title { get; set; }
            public string Chosen { get; set; }
            public int number { get; set; }
        }

        List<conditionsOfProduct> convertList(List<Condition> conditionw)
        {
            List<conditionsOfProduct> listC = new List<conditionsOfProduct>();
            foreach (Condition cond in conditionw)
            {
                listC.Add(new conditionsOfProduct { Chosen = " ", number = cond.IDCondition, Title = cond.Title });
            }
            return listC;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (txtNewArticle.Text != "" && txtNewName.Text != "")
            {
                try
                {
                    if (stockEntities.GetStockEntityD().Product.Where(p => p.Name == txtNewName.Text && p.Article == txtNewArticle.Text).Count() != 0)
                    {
                        prodF = stockEntities.GetStockEntityD().Product.Where(p => p.Name == txtNewName.Text && p.Article == txtNewArticle.Text).First();
                        foreach (conditionsOfProduct c in conditionsP)
                        {
                            foreach (ProdCond pc in prodF.ProdCond)
                            {
                                if (pc.ID_Condition == c.number)
                                {
                                    var find = -1;
                                    try { find = stockEntities.GetStockEntity().Condition.Where(p => p.Title != "Удалено").Select(p => p.Title).ToList().IndexOf(c.Title); }
                                    catch { }
                                    if (find != -1)
                                    {
                                        conditionsP[find].Chosen = "+";
                                        conditionsO.Add(conditionsP[find]);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else { MessageBox.Show("Продукция не найдена!"); }
                }
                catch { }
            }
        }
    }
}
