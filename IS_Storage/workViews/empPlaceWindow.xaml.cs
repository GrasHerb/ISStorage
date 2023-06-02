using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// Логика взаимодействия для empPlaceWindow.xaml
    /// </summary>
    public partial class empPlaceWindow : System.Windows.Window
    {
        List<Condition> q = new List<Condition>();
        Place place = new Place();
        List<conditionsOfPlace> conditions = new List<conditionsOfPlace>();
        List<conditionsOfPlace> conditionsO = new List<conditionsOfPlace>();
        Employee cEmp = null;
        public empPlaceWindow(Employee ew,Place startPlace = null)
        {
            InitializeComponent();
            conditions = convertList(stockEntities.GetStockEntityD().Condition.Where(p => p.Title != "Удалено").ToList());
            cEmp = ew;
            if (startPlace != null)
            {
                place = startPlace;
                txtCode.Text = startPlace.SpecialCode;
                foreach (Condition c in stockEntities.GetStockEntityD().Condition)
                {
                    foreach (PlaceCond pc in startPlace.PlaceCond)
                    {
                        if (pc.ID_Condition == c.IDCondition)
                        {
                            var find = -1;
                            try { find = stockEntities.GetStockEntityD().Condition.Where(p => p.Title != "Удалено").Select(p => p.Title).ToList().IndexOf(c.Title); }
                            catch { }
                            if (find == -1)
                            {
                                conditions.Add((conditionsOfPlace)condGrid.Items[find]);
                                conditions.Last().Chosen = "+";
                            }
                            break;
                        }
                    }
                }
            }
            condGrid.ItemsSource = conditions;
        }

        private void btnA_Click(object sender, RoutedEventArgs e)
        {
            if (txtCode.Text != "")
            {
                try
                {
                    string actions = "";
                    int indexP = 0;
                    if (place.IDPlace != 0)//изменение места
                    {
                        indexP = place.IDPlace;
                        actions = "Изменения места " + place.IDPlace + "\n";
                        if (txtCode.Text != place.SpecialCode)
                            if (stockEntities.GetStockEntityD().Place.Where(p => p.SpecialCode == txtCode.Text).Count() != 0)
                            { MessageBox.Show("Данный код уже занят!"); return; }
                            else
                            {
                                actions += "Изменен код места: " + place.SpecialCode + "=>" + txtCode.Text;
                                place.SpecialCode = txtCode.Text;
                            }

                        foreach (conditionsOfPlace Wcond in conditionsO)//поиск новых свойств
                        {
                            if (place.PlaceCond.Where(p => Wcond.number == p.ID_Condition).Count() == 0)
                            {
                                stockEntities.GetStockEntity().PlaceCond.Add(new PlaceCond() { ID_Place = place.IDPlace, ID_Condition = Wcond.number });
                                actions += "Добавлено свойство: " + Wcond.Title + "\n";
                            }
                        }
                        foreach (PlaceCond Pcond in place.PlaceCond)//поиск новых свойств
                        {
                            if (conditionsO.Where(p => Pcond.ID_Condition == p.number).Count() == 0)
                            {
                                stockEntities.GetStockEntity().PlaceCond.Remove(Pcond);
                                actions += "Удалено свойство: " + Pcond.Condition.Title + "\n";
                            }
                        }

                        if (MessageBox.Show("Применить изменения?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            stockEntities.GetStockEntity().userRequest.Add(new userRequest()
                            {
                                requestTypeID = 3,
                                FullName = cEmp.Full_Name + " изменил место хранения \n" + actions,
                                requestState = 1,
                                requestTime = DateTime.Now.ToString("G"),
                                computerName = Environment.MachineName + " " + Environment.UserName,
                                userID = cEmp.IDEmp
                            });
                            stockEntities.GetStockEntity().SaveChanges();
                            DialogResult = true;
                        }
                    }
                    else //добавление нового
                    {
                        if (stockEntities.GetStockEntityD().Place.Where(p => p.SpecialCode == txtCode.Text).Count() != 0)
                        { MessageBox.Show("Данный код уже занят!"); return; }
                        else
                        {
                            place.SpecialCode = txtCode.Text;
                        }
                        foreach (conditionsOfPlace Wcond in conditionsO)
                        {
                            stockEntities.GetStockEntity().PlaceCond.Add(new PlaceCond() { ID_Place = place.IDPlace, ID_Condition = Wcond.number });
                            actions += "Cвойство: " + Wcond.Title + "\n";
                        }

                        if (MessageBox.Show("Создать место хранения?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            stockEntities.GetStockEntity().userRequest.Add(new userRequest()
                            {
                                requestTypeID = 2,
                                FullName = cEmp.Full_Name + " создал место хранения " + place.SpecialCode + "\n" + actions,
                                requestState = 1,
                                requestTime = DateTime.Now.ToString("G"),
                                computerName = Environment.MachineName + " " + Environment.UserName,
                                userID = cEmp.IDEmp
                            });
                            stockEntities.GetStockEntity().Place.Add(place);
                            stockEntities.GetStockEntity().SaveChanges();
                            DialogResult = true;
                        }
                    }
                }
                catch { }
            }
            else MessageBox.Show("Код места хранения не может быть путым!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var Cpl = conditions.Find(p=> p == (conditionsOfPlace)condGrid.SelectedItem);
            if (Cpl.Chosen==" ") 
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
            condGrid.ItemsSource = conditions;
        }
        class conditionsOfPlace
        {
            public string Title { get; set; }
            public string Chosen { get; set; }
            public int number { get; set; }
        }

        List<conditionsOfPlace> convertList(List<Condition> conditionw)
        {
            List<conditionsOfPlace> listC = new List<conditionsOfPlace>();
            foreach (Condition cond in conditionw)
            {
                listC.Add(new conditionsOfPlace { Chosen = " ", number = cond.IDCondition, Title = cond.Title });
            }
            return listC;
        }
    }
}
