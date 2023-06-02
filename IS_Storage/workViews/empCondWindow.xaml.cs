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
    /// Логика взаимодействия для empCondWindow.xaml
    /// </summary>
    public partial class empCondWindow : Window
    {
        Condition cT = null;
        Employee emT =null;
        public empCondWindow(Employee em,Condition cond = null)
        {
            InitializeComponent();
            if (cond != null) {
                condNameTxt.Text = cond.Title; cT=cond;
                this.Title = "Изменение условия хранения";
                btn1.Content = "Изменить";
            }
            emT = em;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (condNameTxt.Text != "")
            {
                if (cT != null && cT.Title.ToLower() != condNameTxt.Text.ToLower())
                {
                    try
                    {
                        if (stockEntities.GetStockEntityD().Condition.Where(p => p.Title.ToLower() == condNameTxt.Text.ToLower()).Count() == 0)
                        {
                            if (MessageBox.Show("Изменить " + cT.Title + " на " + condNameTxt.Text + "?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                stockEntities.GetStockEntity().userRequest.Add(new userRequest
                                {
                                    requestTypeID = 3,
                                    FullName = emT.Full_Name + " изменил условие хранения \n" + cT.Title + "=>" + condNameTxt.Text,
                                    requestState = 1,
                                    requestTime = DateTime.Now.ToString("G"),
                                    computerName = Environment.MachineName + " " + Environment.UserName,
                                    userID = emT.IDEmp
                                });
                                cT = stockEntities.GetStockEntity().Condition.Find(Title == cT.Title);
                                cT.Title = condNameTxt.Text;
                                stockEntities.GetStockEntity().SaveChanges();
                                DialogResult = true;
                            }
                        }
                        else MessageBox.Show("Такое условие уже существует!");
                    }
                    catch
                    {

                    }
                }
                else
                    try
                    {
                        if (stockEntities.GetStockEntityD().Condition.Where(p => p.Title.ToLower() == condNameTxt.Text.ToLower()).Count() == 0)
                        {
                            if (MessageBox.Show("Добавить условие хранения " + condNameTxt.Text + "?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                stockEntities.GetStockEntity().userRequest.Add(new userRequest
                                {
                                    requestTypeID = 2,
                                    FullName = emT.Full_Name + " добавил условие хранения\n" + condNameTxt.Text,
                                    requestState = 1,
                                    requestTime = DateTime.Now.ToString("G"),
                                    computerName = Environment.MachineName + " " + Environment.UserName,
                                    userID = emT.IDEmp
                                });
                                stockEntities.GetStockEntity().Condition.Add(new Condition() { Title = condNameTxt.Text });
                                stockEntities.GetStockEntity().SaveChanges();
                                DialogResult = true;
                            }
                        }

                    }
                    catch { }
            }

            else MessageBox.Show("Напишите название условия хранения!");
        }
    }
}
