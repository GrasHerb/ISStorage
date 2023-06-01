using IS_Storage.classes;
using IS_Storage.Log_In;
using IS_Storage.workViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace IS_Storage
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Page> pages = new List<Page>();
        public bool working { get; set; }
        statusWindow a;
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            pageUpd();            
            mainFrame.Content = pages[0];
            a = new statusWindow("Прекращение работы", "Пожалуста подождите");
        }
        
        private void pageUpd()
        {
            if (pages.Count != 0) pages.Clear();
            pages.AddRange(new List<Page>
            {
                new log_inPage(),
                new pass_recoverPage("")
            }
            );
        }
        public void pageChange(int pageI = 0, string uLog = "")
        {
            if (pageI > 1&&uLog != "") 
            {
                try
                {
                    Employee a = asonov_KPEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == uLog).FirstOrDefault();
                    workWindow work = new workWindow(a);
                    this.Visibility = Visibility.Hidden;
                    work.Show();
                    working = false;
                }
                catch { }
            }
            else mainFrame.Content = pages[pageI];
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            a.Show();
            var bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerAsync();
            
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new
                        Action(() =>
                        {
                            Environment.Exit(0);
                        }));
        }
    }
}
