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
    /// Логика взаимодействия для transactionWindow.xaml
    /// </summary>
    public partial class transactionWindow : Window
    {
        public transactionWindow(Client a = null, int t = 0)
        {
            InitializeComponent();
            if (a != null )
            {
                
            }
        }
    }
}
