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
                var passCheck = stockEntities.GetStockEntityD().Employee.Where(p => p.Emp_Login == login).FirstOrDefault();
                if (stockEntities.GetStockEntity().userRequest.Where(p => p.userID == passCheck.IDEmp && p.requestState == 0 && p.requestTypeID == 1).FirstOrDefault() != null) return 2;
                if (passCheck.Emp_Pass == "-") return 1;
                if (passCheck.Emp_Login.Contains("___")) return 3;
                if (passCheck.OStatus) return 4;
                passw = Sha256password(passw);/*входящий пароль кодируется*/
                if (passw == passCheck.Emp_Pass /*пароль в базе*/ )
                    return 0;
                else return -1;
            }
            catch { MessageBox.Show("Пользователя с таким логином не существует!"); return -1; }
        }
        public static void statusChange(string login,int i)
        {
            try
            {
                stockEntities localCont = stockEntities.GetStockEntityD();
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
            catch { MessageBox.Show("Ошибка изменения статуса!");}
        }
        public static userRequest deleteEmp(Employee delLogin, Employee admLogin)
        {
            try
            {
                if (delLogin.OStatus) return new userRequest() { ID_Request = -1 };
                if (delLogin==admLogin) return new userRequest { ID_Request = -2 };
                if (delLogin.ID_Role == 1 && stockEntities.GetStockEntity().Employee.Where(p=>p.ID_Role==1).Count()<2) return new userRequest{ ID_Request = -3 };
                delLogin.Emp_Login = "___" + delLogin.Emp_Login;

                return new userRequest()
                {
                    computerName = Environment.MachineName,
                    FullName = admLogin.Full_Name + " (" + admLogin.Emp_Login + ")" + " удалил " + delLogin.Emp_Login + " (" + delLogin.Full_Name + ")",
                    requestTypeID = 4,
                    userID = admLogin.IDEmp,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G"),
                    
                };
            }
            catch { return new userRequest() { ID_Request = -2 }; }
        }
    }
}
