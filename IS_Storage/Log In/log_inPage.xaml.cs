﻿using IS_Storage.classes;
using IS_Storage.Log_In;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для log_inPage.xaml
    /// </summary>
    public partial class log_inPage : Page
    {
        
        MainWindow window = (MainWindow)Application.Current.MainWindow;
        BackgroundWorker bg = new BackgroundWorker() { WorkerSupportsCancellation = true };
        statusWindow sWin = new statusWindow("", "");
        int result = -1;
        bool working = true;
        public log_inPage()
        {
            InitializeComponent();
            txtLog.Focus();
        }
        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sWin.Close();
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            mainWork();
            bg.Dispose();
            bg = new BackgroundWorker();
            working = false;
        }
        
        void messageClose(object sender, RunWorkerCompletedEventArgs e)
        {
            bg.Dispose();
            bg = new BackgroundWorker();
            sWin.Close();
            if ((txtPass.Password != "" || txtPassVisible.Text != "") && txtLog.Text != "")
            {
                result = uControll.passwCheck(txtPass.Password, txtLog.Text);
                if (working)
                {
                    working = false;
                    switch (result)
                    {
                        case 0: break;
                        case 1: MessageBox.Show("Вход отменён."); result = -1; break;
                        case 2: MessageBox.Show("Ваша учётная запись ещё не была подтверждена администратором."); result = -1; break;
                        case 3: MessageBox.Show("Ваша учётная запись была удалена администратором."); result = -1; break;
                        case 4: MessageBox.Show("Даннная учётная запись используется другим пользователем."); result = -1; break;
                        case 5: MessageBox.Show("Учётная запись не найдена."); result = -1; break;
                        default: break;
                    }
                    
                }
                
            }
            else MessageBox.Show("Введите логин и пароль пользователя.");
            
        }
        void startEntering()
        {
            sWin = new statusWindow("Загрузка", "Пожалуйста подождите...");

            working = true;
            sWin.Show();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.RunWorkerAsync();
            bg.RunWorkerCompleted += messageClose;
            
        }
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            result = 0;
            startEntering();
        }

        private void lblForgetPass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            window.pageChange(1);
        }

        private void visibilityCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (visibilityCheck.IsChecked == true)
            {
                txtPass.Visibility = Visibility.Collapsed;
                txtPassVisible.Visibility = Visibility.Visible;
                txtPassVisible.Text = txtPass.Password;
            }
            else
            {
                txtPass.Visibility = Visibility.Visible;
                txtPassVisible.Visibility = Visibility.Collapsed;
                txtPass.Password = txtPassVisible.Text;
            }
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtLog.IsFocused)
                    if (visibilityCheck.IsChecked == true) Keyboard.Focus(txtPassVisible);
                    else Keyboard.Focus(txtPass);
                else { result = 0; startEntering(); }
            }
        }
        void mainWork()
        {
                stockEntities localCont = stockEntities.GetStockEntity();                
                Employee employee = new Employee();
                Dispatcher.Invoke(DispatcherPriority.Background, new
                            Action(() =>
                            {
                                if (txtLog.Text != "" &&
                            txtPass.Visibility == Visibility.Visible
                            ?
                            txtPass.Password != ""
                            :
                            txtPassVisible.Text != ""
                            )
                                {
                                    int result = uControll.passwCheck(txtPass.Password, txtLog.Text);
                                    switch (result)
                                    {
                                        case 0:
                                            uControll.statusChange(txtLog.Text, 1);
                                            window.pageChange(2, txtLog.Text);


                                            break;
                                        case 1:
                                            MessageBox.Show("Вы были зарегистрированы администратором.\n Для продолжения работы установите пароль.");
                                            passChange passwindow = new passChange(localCont.Employee.Where(p => p.Emp_Login == txtLog.Text).FirstOrDefault());
                                            if (passwindow.ShowDialog() == true)
                                            {
                                                employee = localCont.Employee.Where(p => p.Emp_Login == txtLog.Text).FirstOrDefault();
                                                employee.Emp_Pass = passwindow.newEmp.Emp_Pass;
                                                localCont.SaveChanges();
                                                uControll.statusChange(txtLog.Text, 1);
                                                window.pageChange(2, txtLog.Text);
                                            }
                                            else
                                            {

                                            }
                                            break;
                                        default: return;
                                    }

                                }
                                else return;
                            }));
            
        }
    }
}
