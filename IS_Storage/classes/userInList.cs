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
        public int uNumInGrid { get; set; }
        public string uFullName { get; set; }
        public string uRole { get; set; }
        public string uLastTime { get; set; }
        public string uComputer { get; set; }
        public static ObservableCollection<userInList> listConvert(List<Employee> employees)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            ObservableCollection<userInList> converted = new ObservableCollection<userInList>();
            List<UserRole> roles = localCont.UserRole.ToList();
            int c = 1;
            foreach (Employee a in employees)
            {
                converted.Add
                    (new userInList()
                    {
                        oStatus = a.OStatus ? '►' : ' ',
                        uNumber = a.IDEmp,
                        uNumInGrid = a.Emp_Login.Contains("___") ? 0 : c++,
                        uFullName = a.Full_Name,
                        uRole = roles.Where(p => p.IDRole == a.ID_Role).FirstOrDefault().Title,
                        uLastTime = a.sysInfo != " " ? a.sysInfo.Split('*')[0] : " ",
                        uComputer = a.sysInfo != " " ? a.sysInfo.Split('*')[1] : " "
                    });
            }

            return converted;
        }
    }
    public class deluserInList
    {
        public int uNumber { get; set; }
        public string uFullName { get; set; }
        public int reqId { get; set; }
        public static ObservableCollection<deluserInList> listConvert(List<Employee> employees)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            ObservableCollection<deluserInList> converted = new ObservableCollection<deluserInList>();
            List<userRequest> delRequests = localCont.userRequest.Where(p => p.requestTypeID == 4).ToList();
            foreach (Employee a in employees)
            {
                if (a.Emp_Login.Contains("___"))
                    converted.Add
                        (new deluserInList()
                        {
                            uNumber = a.IDEmp,
                            uFullName = a.Full_Name + " " + a.Emp_Login.Remove(0, 3),
                            reqId = delRequests.Where(p => p.FullName.Split(' ')[5] == a.Emp_Login.Remove(0,3)).FirstOrDefault().ID_Request
                        }) ;
            }

            return converted;
        }
    }
}
