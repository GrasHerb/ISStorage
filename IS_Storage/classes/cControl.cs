using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Globalization;

namespace IS_Storage.classes
{

    public class cControl
    {
        public int numActual { get; set; }
        public int numInGrid { get; set; }
        public List<Product> productsOfClient { get;set; }
        public string Name { get; set; }
        public string PNumber { get; set; }
        public string Email { get; set; }

        public static List<cControl> listConvert(List<Client> clients)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            List<cControl> converted = new List<cControl>();
            int c = 1;
            foreach (Client a in clients)
            {
                    converted.Add
                    (new cControl()
                    {
                        numActual = a.IDClient,
                        numInGrid = c++,
                        productsOfClient = ProdofClient(a),
                        Name = a.Name,
                        PNumber = a.PNumber,
                        Email = a.Email
                    });
            }

            return converted;
        }

        public static List<Product> ProdofClient(Client cl)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            List<Transaction> tr = localCont.Transaction.Where(p => p.ID_Client == cl.IDClient).ToList();

            List<Product> products = new List<Product>();

            foreach (Transaction t in tr.Where(p => p.ID_TrTType == 1).ToList())
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
        public static List<Client> excelImport(List<Client> a, string filePath) 
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook excelworkBook;
                Microsoft.Office.Interop.Excel.Worksheet excelSheet;
                List<Client> addRange = new List<Client>();

                int c = 3;

                excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.DisplayAlerts = false;

                excelworkBook = excel.Workbooks.Open(filePath,null,false);
                excelSheet = excelworkBook.Sheets[1];
                while (excelSheet.Cells[c,2].Value!=null)
                {
                    if (a.Where(p => p.Name == excelSheet.Cells[c, 2].Value2).Count() == 0)
                    {
                        addRange.Add(new Client
                        {                            
                            Name = Convert.ToString(excelSheet.Cells[c,2].Value2),
                            PNumber = "+"+Convert.ToString(excelSheet.Cells[c, 3].Value2),
                            Email = Convert.ToString(excelSheet.Cells[c, 4].Value2)
                        });
                    }
                    c++;
                }
                excelworkBook.Close(0);
                return addRange;
            }
            catch { return new List<Client>(); }
        }
        public static List<Client> jsonImport(List<Client> a, string filePath) 
        {
            var j = File.ReadAllText(filePath, Encoding.GetEncoding(1251));
            var jlist = JsonConvert.DeserializeObject<List<Client>>(j);

            return jlist;
        }
        public static void excelExport(List<Client> a, string filePath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook excelworkBook;
                Microsoft.Office.Interop.Excel.Worksheet excelSheet;

                excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.DisplayAlerts = false;

                excelworkBook = excel.Workbooks.Add(Type.Missing);

                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = "Список клиентов";

                excelSheet.Cells[1, 1] = "Клиенты в базе";
                excelSheet.Cells[1, 2] = "Дата списка: " + DateTime.Now.ToShortDateString();

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Client));

                excelSheet.Cells[2, 1].Value2 = "Номер";
                excelSheet.Cells[2, 2].Value2 = "ФИО/Название";
                excelSheet.Cells[2, 3].Value2 = "Контактный номер";
                excelSheet.Cells[2, 4].Value2 = "Электронная почта";

                for (int i = 0; i < a.Count; i++)
                {
                    excelSheet.Cells[i + 3, 1].Value2 = a[i].IDClient;
                    excelSheet.Cells[i + 3, 2].Value2 = a[i].Name;
                    excelSheet.Cells[i + 3, 3].Value2 = a[i].PNumber;
                    excelSheet.Cells[i + 3, 4].Value2 = a[i].Email;
                }

                excel.Application.ActiveWorkbook.SaveAs(filePath, XlFileFormat.xlExcel12, "", "", false,false,XlSaveAsAccessMode.xlShared) ;
                excelworkBook.Close(0);
            }
            catch { }
        }
        public static void jsonExport(List<Client> a, string filePath)
        {
            try
            {
                var jList = a.Select(p => new { p.IDClient, p.Name, p.PNumber, p.Email }).ToList(); 
                var j = JsonConvert.SerializeObject(jList);
                File.WriteAllText(filePath, j, Encoding.GetEncoding(1251));
            }
            catch { }
        }
    }
}
