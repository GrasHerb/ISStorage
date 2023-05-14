using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IS_Storage.classes
{
    public class userInList
    {
        public char oStatus { get; set; }
        public int uNumber { get; set; }
        public string uFullName { get; set; }
        public string uRole { get; set; }
        public string uLastTime { get; set; }
        public string uComputer { get; set; }
        public static ObservableCollection<userInList> listConvert(List<Employee> employees)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            ObservableCollection<userInList> converted = new ObservableCollection<userInList>();
            List<UserRole> roles = localCont.UserRole.ToList();
            for (int i = 0; i<employees.Count; i++)
            {
                converted.Add(new userInList());
                converted[i].oStatus = employees[i].OStatus ? '►' : ' ';
                converted[i].uNumber = employees[i].IDEmp;
                converted[i].uFullName = employees[i].Full_Name;
                converted[i].uRole = roles.Where(p => p.IDRole == employees[i].ID_Role).FirstOrDefault().Title;
                converted[i].uLastTime = employees[i].sysInfo != " " ? employees[i].sysInfo.Split('*')[0] : " ";
                converted[i].uComputer = employees[i].sysInfo != " " ? employees[i].sysInfo.Split('*')[1] : " ";
            }
            //foreach (Employee a in employees)
            //{
            //    converted.Add
            //        (new userInList()
            //        {
            //            oStatus = a.OStatus ? new BitmapImage(new Uri("images\\online.png")) : new BitmapImage(),
            //            uNumber = a.IDEmp,
            //            uFullName = a.Full_Name,
            //            uRole = roles.Where(p=>p.IDRole == a.ID_Role).FirstOrDefault().Title,
            //            uLastTime = a.sysInfo!="" ? a.sysInfo.Split('*')[0]:"",
            //            uComputer = a.sysInfo != "" ? a.sysInfo.Split('*')[1]:""
            //        });
            //}

            return converted;
        }
    }   
}
