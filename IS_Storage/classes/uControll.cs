using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IS_Storage.classes
{
    public static class uControll
    {
        
        public static bool[] passwordReq(string passw)
        {
            bool[] checks = { true, true, true };
            if (passw.Length < 6) checks[0] = false;
            if (passw == passw.ToLower()) checks[1] = false;
            if (!(passw.AsEnumerable().Any(ch => char.IsDigit(ch)) && passw.AsEnumerable().Any(ch => char.IsLetter(ch)))) checks[2] = false;
            return checks;
        }
        public static string Sha256password(string passw)
        {
            var passcode = SHA256.Create();
            StringBuilder builder = new StringBuilder();
            byte[] bytes = passcode.ComputeHash(Encoding.UTF8.GetBytes(passw));
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            passw = builder.ToString();
            return passw;
        }
        public static int passwCheck(string passw, string login)
        {
            try
            {
                var passwCheck = stockEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == login).FirstOrDefault();
                if (stockEntities.GetStockEntity().userRequest.Where(p => p.userID == passwCheck.IDEmp && p.requestState == 0 && p.requestTypeID == 1).FirstOrDefault() != null) return 2;
                if (passwCheck.Emp_Pass == "-") return 1;
                if (passwCheck.Emp_Login.Contains("___")) return 3;
                passw = Sha256password(passw);/*входящий пароль кодируется*/
                if (passw == passwCheck.Emp_Pass /*пароль в базе*/ )
                    return 0;
                else return -1;
            }
            catch { MessageBox.Show("Ошибка проверки пароля!"); return -1; }
        }
        public static void statusChange(string login,int i)
        {
            try
            {
                stockEntities localCont = stockEntities.GetStockEntity();
                Employee a = localCont.Employee.Where(p => p.Emp_Login == login).FirstOrDefault();
                
                switch (i)
                {
                    case 1: a.OStatus = true; a.sysInfo = DateTime.Now.ToString("G") + "*" + Environment.MachineName; break;
                    case 2:
                        a.OStatus = false;                        
                        break;
                    default: break;
                }
                localCont.SaveChanges();
            }
            catch { MessageBox.Show("Ошибка изменения статуса!"); }
        }
        //public static int newEmployee(string newLogin, string newPass, string newFullName)
        //{
        //    try
        //    {
        //        if (stockEntities.GetStockEntity().Employee.Where(p => p.Emp_Login == newLogin).Count() > 0) return 1;
        //        Employee employee = new Employee()
        //        {

        //        };
        //        return 0;

        //    }
        //    catch
        //    {
        //        return -1;
        //    }
        //}
        public static int deleteEmp(string delLogin, string admLogin)
        {
            try
            {
                stockEntities localCont = stockEntities.GetStockEntity();
                Employee delemp = localCont.Employee.Where(p => p.Emp_Login == delLogin).FirstOrDefault();
                Employee admepm = localCont.Employee.Where(p => p.Emp_Login == admLogin).FirstOrDefault();
                if (delemp.OStatus) return 1;
                delemp.Emp_Login = "___" + delLogin;
                localCont.userRequest.Add(new userRequest() 
                { 
                    computerName = Environment.MachineName,
                    FullName = admepm.Full_Name + " удалил " + delemp.Full_Name,
                    requestTypeID = 4,
                    userID = admepm.IDEmp,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G")
                });
                localCont.SaveChanges();
                return 0;
            }
            catch { return -1; }
        }
    }
}
