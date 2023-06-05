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
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;
using IS_Storage.classes;
using System.Windows.Threading;

namespace IS_Storage.Log_In
{
    /// <summary>
    /// Логика взаимодействия для pass_recoverPage.xaml
    /// </summary>
    public partial class pass_recoverPage : Page
    {
        Random rnd = new Random();
        string code = "";
        int recover = 0;
        string usLog = "";
        Employee newEmp = new Employee();
        MainWindow window = (MainWindow)Application.Current.MainWindow;
        stockEntities _context = stockEntities.GetStockEntity();
        DispatcherTimer timer = new DispatcherTimer();
        string reqTime = "";
        public pass_recoverPage(string usLogIn)
        {
            InitializeComponent();
            usLog = usLogIn;
            maintxt.Text = usLog;
        }
        private async void recoverProcess(int stage)
        {
            switch (stage)
            {
                case 0:

                    if (maintxt.Text != "")
                    {
                        if (_context.Employee
                            .Where(p => p.EEmail == maintxt.Text || p.Emp_Login == maintxt.Text).FirstOrDefault() != null)
                        {
                            if (!_context.Employee
                            .Where(p => p.EEmail == maintxt.Text || p.Emp_Login == maintxt.Text).FirstOrDefault().OStatus)
                            {
                                newEmp = _context.Employee.Where(p => p.EEmail == maintxt.Text || p.Emp_Login == maintxt.Text).FirstOrDefault();
                                if (newEmp.EEmail != "-")
                                {
                                    MailSend(newEmp.EEmail);
                                    recover = 1;
                                    recoverBtn.Content = "Подтвердить";
                                    mainLbl.Content = "Введите код подтверждения";
                                    usLog = maintxt.Text;
                                    maintxt.Text = "";
                                }
                                else
                                {
                                    timer.Tick += new EventHandler(timer_Tick);
                                    timer.Interval = new TimeSpan(0, 0, 5);
                                    reqTime = DateTime.Now.ToString("G");
                                    _context.userRequest.Add(new userRequest() { requestTypeID = 1, FullName = newEmp.Full_Name+": запрос на восстановление пароля", requestState = 0, requestTime = reqTime, computerName = Environment.MachineName + " " + Environment.UserName, userID = newEmp.IDEmp });
                                    int a = await _context.SaveChangesAsync();
                                    _context = stockEntities.GetStockEntity();
                                    timer.Start();
                                    mainLbl.Content = "Запрос на восстановление пароля отправлен.\n Свяжитесь с администратором."; 
                                    recoverBtn.Visibility = Visibility.Collapsed; }
                            }
                            else
                                MessageBox.Show("Данный пользователь в данный момент авторизован в системе.");
                        }
                        else
                            MessageBox.Show("Пользователь не найден.\n Для регистрации обратитесь к уполномоченному лицу.");
                    }
                    else MessageBox.Show("Необходимо указать почту или логин пользователя. \n Для регистрации обратитесь к уполномоченному лицу.");
                    break;

                case 1:

                    if (code == maintxt.Text || recover == 3)
                    {
                        recover = 2;
                        recoverProcess(recover);
                    }
                    else
                    {
                        MessageBox.Show("Был введён неверный код.");                        
                        cleaning();
                    }
                    break;
                case 2:
                    passChange passwindow = new passChange(newEmp);
                    
                    if (passwindow.ShowDialog() == true)
                    {
                        newEmp = passwindow.newEmp;
                        _context.SaveChanges();
                        cleaning();
                        window.pageChange(0);
                    }
                    else { MessageBox.Show("Восстановление отменено.");
                        window.pageChange(0);
                    }
                    
                    break;
            }
        }
        private void recoverBtn_Click(object sender, RoutedEventArgs e)
        {
            recoverProcess(recover);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                _context = stockEntities.GetStockEntity();
                if (stockEntities.GetStockEntityD().userRequest.Where(p => p.userID == newEmp.IDEmp && p.requestTime == reqTime).FirstOrDefault().requestState == 1)
                {
                    timer.Stop();
                    recover = 3;
                    recoverProcess(1);
                }
                if (stockEntities.GetStockEntityD().userRequest.Where(p => p.userID == newEmp.IDEmp && p.requestTime == reqTime).FirstOrDefault().requestState == 2)
                {
                    MessageBox.Show("Запрос был отменён администратором.");
                    timer.Stop();
                    cleaning();
                }
            }
            catch
            {
                timer.Stop();
                MessageBox.Show("Запрос не найден.");
            }
        }
        private void cleaning()
        {
            recoverBtn.Content = "Восстановить";
            recoverBtn.Visibility = Visibility.Visible;
            mainLbl.Content = "Укажите логин/почту пользователя";
            maintxt.Visibility = Visibility.Visible;
            maintxt.Text = "";
            code = "";
            recover = 0;
            usLog = "";
            newEmp = new Employee();
            window = (MainWindow)Application.Current.MainWindow;
            _context = stockEntities.GetStockEntity();
            timer = new DispatcherTimer();
            reqTime = "";
        }
        private void MailSend(string umail)
        {
                code = "";

                for (int i = 0; i < 6; i++)
                {
                    code += rnd.Next(0, 10);
                }

                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("is_storage@rambler.ru", "ИС Склад");
                // кому отправляем
                MailAddress to = new MailAddress(umail);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Восстановление пароля";
            // текст письма
                m.Body =
                "<h1 style=\"text-align:center\"><span style=\"font-size:26px\">ИС &quot;Склад&quot;" +
                "</span></h1>\r\n\r\n<p style=\"text-align:center\">" +
                "<span style=\"font-size:24px\">Здравствуйте, "+ _context.Employee.Where(p => p.EEmail == umail).FirstOrDefault().Full_Name + 
                "</span>"+"</p>\r\n\r\n<p style=\"text-align:center\"><span style=\"font-size:24px\">" +
                "Вы запросили восстановление пароля. Пожалуйста, для восстановления вашего пароля " +
                "в системе введите в соответсвующее поле код для восстановления пароля.</span></p>" +
                "\r\n\r\n<hr />\r\n<p style=\"text-align:center\"><span style=\"font-size:26px\">" +
                "<span style=\"background-color:#84d183\">Код для восстановления пароля: </span><strong>" +
                "<span style=\"background-color:#84d183\">"+code+"</span></strong></span></p>\r\n\r\n" +
                "<hr />\r\n<p>&nbsp;</p>\r\n\r\n<p><strong><span style=\"font-size:14px\">" +
                "Это письмо создано автоматически!&nbsp;Почта для&nbsp;обратной связи:&nbsp;grasonof@gmail.com</span></strong></p>";
                // письмо представляет код html
                m.IsBodyHtml = true;
            mailManager.Sending(m);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();            
            window.pageChange(0);
        }
        
    }
}
