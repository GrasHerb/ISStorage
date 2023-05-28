using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Storage.classes
{
    public static class transactionControll
    {
        public static List<Product> ProdofClient(Client cl)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            List<Transaction> tr = localCont.Transaction.Where(p => p.ID_Client == cl.IDClient).ToList();

            List<Product> products = new List<Product>();

            foreach (Transaction t in tr.Where(p=>p.ID_TrTType == 1).ToList())
            {
                if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0) products.Find(p => p.IDProduct == t.ID_Product).Amount += t.Amount;
                else products.Add(localCont.Product.Where(p => p.IDProduct == t.ID_Product).First());
            }
            foreach (Transaction t in tr.Where(p => p.ID_TrTType == 2).ToList())
            {
                if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0) products.Find(p => p.IDProduct == t.ID_Product).Amount -= t.Amount;
            }
            return products;

        }

        public static userRequest delClient(Client delClient, Employee managerLog)
        {
            try
            {
                delClient.Name = "___" + delClient.Name;

                return new userRequest()
                {
                    computerName = Environment.MachineName,
                    FullName = managerLog.Full_Name + " (" + managerLog.Emp_Login + ")" + " удалил " + delClient.Name,
                    requestTypeID = 4,
                    userID = managerLog.IDEmp,
                    requestState = 1,
                    requestTime = DateTime.Now.ToString("G"),
                };
            }
            catch { return new userRequest() { ID_Request = -2 }; }
        }
    }
}
