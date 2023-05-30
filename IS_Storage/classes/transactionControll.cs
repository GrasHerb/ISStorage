﻿using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IS_Storage.classes
{
    public class transactionControll
    {        
        public int IDt { get; set; }
        public string Client { get; set; }
        public string Date { get; set; }
        public DateTime actualDate { get; set; }
        public List<Transaction> actualList { get; set; }

        public static List<transactionControll> listConvert(List<Transaction> tr)
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            List<transactionControll> converted = new List<transactionControll>();
            int c = 1;
            var cultureInfo = new CultureInfo("ru-RU");
            foreach (Transaction a in tr)
            {
                if (converted.Where(p => p.Client == a.Client.Name && p.Date == a.Date).ToList().Count != 0)
                    converted.Where(p => p.Client == a.Client.Name && p.Date == a.Date).First().actualList.Add(a);
                else
                {
                    converted.Add
                    (new transactionControll()
                    {
                        IDt = c++,
                        Client = a.Client.Name,
                        Date = a.Date,
                        actualDate = DateTime.ParseExact(a.Date.Split(' ')[0] + " 00:00:00","G",cultureInfo),
                        actualList = new List<Transaction>() {a}
                    });
                }                
            }
            return converted;
        }


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
        public static void excelExport(List<Client> a, string filePath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook excelworkBook;
                Microsoft.Office.Interop.Excel.Worksheet excelSheet;
                Microsoft.Office.Interop.Excel.Range excelCellrange;

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
                    excelSheet.Cells[i+3, 1].Value2 = a[i].IDClient;
                    excelSheet.Cells[i + 3, 2].Value2 = a[i].Name;
                    excelSheet.Cells[i + 3, 3].Value2 = a[i].PNumber;
                    excelSheet.Cells[i + 3, 4].Value2 = a[i].Email;
                }

                excelCellrange = excelSheet.Range[excelSheet.Cells[2, 1], excelSheet.Cells[a.Count, properties.Count]];
                excelCellrange.EntireColumn.AutoFit();

                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;

                excel.Application.ActiveWorkbook.SaveAs(filePath, XlFileFormat.xlExcel12);
                excel.Quit();
            }
            catch { }
        }
    }
}
