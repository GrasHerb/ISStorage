using IS_Storage.Log_In;
using IS_Storage.workViews;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            pageUpd();            
            mainFrame.Content = pages[0];            
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
                    Employee a = stockEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == uLog).FirstOrDefault();
                    workWindow work = new workWindow(a);
                    this.Visibility = Visibility.Hidden;
                    work.Show();                   
                }
                catch { }
            }
            else mainFrame.Content = pages[pageI];
        
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        
    }
}
