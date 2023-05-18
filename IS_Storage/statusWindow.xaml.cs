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
using System.Windows.Threading;

namespace IS_Storage
{
    /// <summary>
    /// Логика взаимодействия для statusWindow.xaml
    /// </summary>
    public partial class statusWindow : Window
    {
        public statusWindow(string Head, string Body)
        {
            InitializeComponent();
            headL.Text = Head + "\n";
            textL.Text = Body;
        }
    }
}
