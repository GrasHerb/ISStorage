using IS_Storage.classes;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using MailMessage = System.Net.Mail.MailMessage;
using System.Net.Http;
using System.IO;
using System.ComponentModel;

namespace IS_Storage.workViews
{
    /// <summary>
    /// Логика взаимодействия для transactionWindow.xaml
    /// </summary>
    public partial class transactionWindow : System.Windows.Window
    {
        stockEntities localCont = stockEntities.GetStockEntityD();
        List<pControl> productsExtra;
        List<Product> products;
        transactionControll trans;
        Client cClient = new Client();
        string document = "";
        MailMessage m;
        Employee cEmp;
        public transactionWindow(Client cl = null,transactionControll tr = null, Place pl = null, Employee cE = null)
        {
            InitializeComponent();
            if (cl != null)
            {
                products = pControl.ProductsSearch(null,cl);
                productsExtra = pControl.pControlConvert().Where(p=>p.Client==cl.Name).ToList();
                cClient = cl;
                m.Subject = "Продукция клиента: " + cl.Name;
                document = "Продукция клиента: " + cl.Name;
            }
            if (tr != null)
            {
                products = pControl.ProductsSearch(tr);
                productsExtra = pControl.pControlConvert(tr).ToList();
                m.Subject = "Информация о транзакции: " + tr.Date+" "+tr.Client;
                document = "Информация о транзакции: " + tr.Date+" "+tr.Client;
            }
            if (pl != null)
            {
                products = pControl.ProductsSearch(null, null, pl);
                productsExtra = pControl.pControlConvert().Where(p=>p.Place == pl.SpecialCode).ToList();
                mailSendBtn.IsEnabled = false;
                document = "Продукция на складе: " + pl.SpecialCode;
            }
            gridUpdate();
        }

        void gridUpdate()
        {

            if (extraCh.IsChecked == true)
            {
                yExtra.Visibility = Visibility.Visible;
                noExtra.Visibility = Visibility.Collapsed;
            }
            else
            {
                noExtra.Visibility = Visibility.Visible;
                yExtra.Visibility = Visibility.Collapsed;
            }
            mainGrid.ItemsSource = products;
            mainGridExtra.ItemsSource = productsExtra;
        }
        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\tempdocument.docx");
            }
            catch { }
            startExporting(1);
        }

        private void mailSendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\tempdocument.docx");
            }
            catch { }
            startExporting(2);
        }
        void startExporting(int type = 2)
        {
            switch (type) 
            {
                case 1:
                    SaveFileDialog a = new SaveFileDialog();
                    a.Filter = "Файлы Word | *.docx";
                    if (a.ShowDialog() == true)
                        pControl.wordExport(productsExtra, products, a.FileName, document);
                    break;
                case 2:
                    if(MessageBox.Show("Отправить данные на почту " + cClient.Email + "?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        pControl.wordExport(productsExtra, products, "", document);

                        MailAddress from = new MailAddress("is_storage@rambler.ru", "ИС Склад");
                        MailAddress to = new MailAddress(cClient.Email);
                        m = new MailMessage(from, to);
                        m.Body = "<p style=\"text - align: center; \">ИС Склад</p><p style=\"text - align: left; \">Здравствуйте, "+ cClient.Name+ ", ваш отчёт находится в прикреплённом документе.</p><p style=\"text - align: left; \">Дата отчёта: "+DateTime.Now.ToString("G")+"</p>";
                        m.IsBodyHtml = true;
                        mailManager.Sending(m, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\tempdocument.docx");
                        if (cEmp != null) 
                        {
                            localCont.userRequest.Add(new userRequest()
                            {
                                FullName = cEmp.Full_Name + " (" + cEmp.Emp_Login + ") " + " создал и отправил документ '" + document + " клиенту " + cClient.Name + " на почту " + cClient.Email,
                                computerName = Environment.MachineName,
                                requestTypeID = 2,
                                userID = cEmp.IDEmp,
                                requestState = 1,
                                requestTime = DateTime.Now.ToString("G"),

                            });
                            localCont.SaveChanges();
                        }
                        
                        if (MessageBox.Show("Документ отправлен!\nЗакрыть окно?", "Отправка документа", MessageBoxButton.YesNo) == MessageBoxResult.Yes) this.Close();
                    }
                    break;
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            gridUpdate();
        }

    }
}
