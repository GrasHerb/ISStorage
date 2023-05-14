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
        public BitmapImage oStatus { get; set; }
        public int uNumber { get; set; }
        public string uFullName { get; set; }
        public string uRole { get; set; }
        public string uLastTime { get; set; }
        public string uComputer { get; set; }
        public static ObservableCollection<userInList> listConvert(List<Employee> employees)
        {
            ObservableCollection<userInList> converted = new ObservableCollection<userInList>();

            foreach (Employee a in employees)
            {
                converted.Add
                    (new userInList()
                    {
                        oStatus = a.OStatus ? new BitmapImage(new Uri("images\\online.png")) : new BitmapImage(),
                        uNumber = a.IDEmp,
                        uFullName = a.Full_Name,
                        uRole = stockEntities.GetStockEntity().UserRole.Where(p=>p.IDRole == a.ID_Role).FirstOrDefault().Title,
                        uLastTime = a.sysInfo.Split('*')[0],
                        uComputer = a.sysInfo.Split('*')[1]
                    });
            }

            return converted;
        }
    }   
}
