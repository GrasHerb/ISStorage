using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace IS_Storage.classes
{
    public class transactionControll
    {        
        public int IDt { get; set; }
        public string Client { get; set; }
        public Client aClient { get; set; }
        public string Date { get; set; }
        public DateTime actualDate { get; set; }
        public List<Transaction> actualList { get; set; }

        public static List<transactionControll> listConvert(List<Transaction> tr)
        {
            stockEntities localCont = stockEntities.GetStockEntityD();
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
                        aClient = a.Client,
                        Date = a.Date,
                        actualDate = DateTime.ParseExact(a.Date.Split(' ')[0] + " 00:00:00", "G", cultureInfo),
                        actualList = new List<Transaction>() { a }
                    }) ;
                }                
            }
            return converted;
        }


        


        public static void excelExport(List<transactionControll> a, string filePath, string date = null, string client = null)
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
                excelSheet.Name = "Список транзакций";
                int c = 0;
                excelSheet.Cells[1, 1].Value2 = "Транзакции";

                excelSheet.Cells[1, 2].Value2 = "Дата составления: ";
                excelSheet.Cells[1, 3].Value2 = DateTime.Now.ToString("G");

                if (date != null)
                {
                    c++;
                    excelSheet.Cells[1 + c, 1].Value2 = "Период транзакций: ";
                    excelSheet.Cells[1 + c, 2].Value2 = date.Split(' ')[0];
                    excelSheet.Cells[1 + c, 3].Value2 = date.Split(' ')[1];
                }

                if(client != null)
                {
                    c++;
                    excelSheet.Cells[1 + c, 1].Value2 = "Клиент: ";
                    excelSheet.Cells[1 + c, 2].Value2 = client;
                }

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Client));

                excelSheet.Cells[2 + c, 1].Value2 = "Номер";
                excelSheet.Cells[2 + c, 2].Value2 = "ФИО/Название";
                excelSheet.Cells[2 + c, 3].Value2 = "Дата";

                c++;

                for (int i = 0; i < a.Count; i++)
                {
                    excelSheet.Cells[i + 3 + c, 1].Value2 = a[i].IDt;
                    excelSheet.Cells[i + 3 + c, 2].Value2 = a[i].Client;
                    excelSheet.Cells[i + 3 + c, 3].Value2 = a[i].Date;
                }

                excelSheet.Columns.AutoFit();

                excel.Application.ActiveWorkbook.SaveAs(filePath, XlFileFormat.xlExcel12);
                excel.Quit();
            }
            catch { }
        }
        public static void wordExport(List<transactionControll> a, string filePath, string date = null, string client = null)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application word;

                word = new Microsoft.Office.Interop.Word.Application();

                word.Visible = false;

                var document = word.Documents.Add();

                var p = document.Paragraphs.Add();

                p = document.Paragraphs.Add();

                p.Range.Text = "Транзакции";

                p = document.Paragraphs.Add();

                p.Range.Text = "Дата составления: " + DateTime.Now.ToString("G");

                p = document.Paragraphs.Add();


                if (date != null)
                {
                    p.Range.Text = "Период транзакций: " + date.Split(' ')[0]+" "+date.Split(' ')[1];
                    p = document.Paragraphs.Add();
                }

                if (client != null)
                {
                    p.Range.Text = "Клиент: " + client;
                    p = document.Paragraphs.Add();
                }
                p = document.Paragraphs.Add();
                p = document.Paragraphs.Add();
                Table table = document.Tables.Add(p.Range,a.Count+1,3);

                table.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;


                table.Cell(1, 1).Range.Text = "Номер";
                table.Cell(1, 2).Range.Text = "ФИО/Название";
                table.Cell(1, 3).Range.Text = "Дата";
                for (int i = 0; i < a.Count; i++)
                {
                    table.Cell(i+2, 1).Range.Text = a[i].IDt.ToString();
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i+2, 2).Range.Text = a[i].Client;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i+2, 3).Range.Text = a[i].Date;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                }

                word.Application.ActiveDocument.SaveAs2(filePath, WdSaveFormat.wdFormatDocumentDefault);
                word.Quit();
            }
            catch { }
        }
    }
}
