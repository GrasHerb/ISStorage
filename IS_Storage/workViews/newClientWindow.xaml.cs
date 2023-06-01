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
    /// Логика взаимодействия для newClientWindow.xaml
    /// </summary>
    public partial class newClientWindow : Window
    {
        asonov_KPEntities localCont = asonov_KPEntities.GetStockEntity();
        int type = 0;
        Employee employee = null;
        public Client clientChange { get; set; }  = new Client();
        public newClientWindow(int type, Employee employee, Client a = null)
        {
            InitializeComponent();
            this.type = type;
            this.employee = employee;
            clientChange = a!=null? a : new Client();
            if (clientChange != new Client()) {
                try
                {
                    txtName.Text = clientChange.Name;
                    txtPhNum.Text = clientChange.PNumber;
                    txtMail.Text = clientChange.Email;
                }
                catch
                {

                }
            }
            
        }

        private void txtKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void applyBtn(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case 0:
                    if (txtMail.Text != "" && txtName.Text != "" && txtPhNum.Text != "")
                    {
                        if (localCont.Client.Where(p => p.Name == txtName.Text).Count() == 0)
                        {
                            if (txtName.Text.Contains("___")) { MessageBox.Show("ФИО или название клиента не может содержать '___'"); return; }
                            if (MessageBox.Show("Добавить клиента?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                            localCont.Client.Add(new Client { Name = txtName.Text, Email = txtMail.Text, PNumber = txtPhNum.Text });
                            localCont.SaveChanges();
                            localCont.userRequest.Add(new userRequest() { requestTypeID = 2, FullName = employee.Full_Name + " создал клиента: " + txtName.Text, requestState = 1, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = employee.IDEmp });
                            localCont.SaveChanges();
                            DialogResult = true;
                        }
                        else MessageBox.Show("Клиент уже существует в базе данных!");
                    }
                    else MessageBox.Show("Заполните все поля!");
                    break;
                case 1:
                    if (txtMail.Text != "" && txtName.Text != "" && txtPhNum.Text != "")
                    {
                        if (txtName.Text != clientChange.Name && localCont.Client.Where(p => p.Name == txtName.Text).Count() == 0)
                        {
                            if (txtName.Text.Contains("___")){ MessageBox.Show("ФИО или название клиента не может содержать '___'"); return; }
                            string reqText = "Изменения";
                            if (txtName.Text != clientChange.Name)
                            {
                                reqText += "\nФИО/Название: " + clientChange.Name + "=>" + txtName.Text;
                                clientChange.Name = txtName.Text;
                            }
                            if (txtPhNum.Text != clientChange.PNumber)
                            {
                                reqText += "\nКонтактный номер: " + clientChange.PNumber + "=>" + txtPhNum.Text;
                                clientChange.PNumber =  txtPhNum.Text;
                            }
                            if (txtMail.Text != clientChange.Email)
                            {
                                reqText += "\nE-mail: " + clientChange.Email + "=>" + txtMail.Text;
                                clientChange.Email = txtMail.Text;
                            }
                            if (MessageBox.Show("Применить изменения?\n" + reqText, "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) { return; }
                            var changing = localCont.Client.Where(p => p.IDClient == clientChange.IDClient).First();
                            changing = clientChange;
                            localCont.SaveChanges();
                            localCont.userRequest.Add(new userRequest() { requestTypeID = 4, FullName = employee.Full_Name + " изменил данные клиента.\n" + reqText, requestState = 1, requestTime = DateTime.Now.ToString("G"), computerName = Environment.MachineName + " " + Environment.UserName, userID = employee.IDEmp });
                            localCont.SaveChanges();
                        }
                        else MessageBox.Show("Клиент уже существует в базе данных!");
                    }
                    else MessageBox.Show("Заполните все поля!");
                    break;
                default: break;
            }
            
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
