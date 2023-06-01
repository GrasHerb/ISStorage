using IS_Storage.workViews;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IS_Storage.classes
{
    public class pControl
    {
        public int Id { get; set; }
        public int prodId { get; set; }
        public string prodName { get; set; }
        public int trId { get; set; }
        public string article { get; set; }
        public string Place { get; set; }
        public int idPlace { get; set; }
        public double Amount { get; set; }
        public string Client { get; set; }
        public string ArivalDate { get; set; }
        public DateTime actualArivalDate { get; set; }

        public static List<Product> ProductsSearch(transactionControll transaction = null, Client cl = null, Place place = null)
        {
            asonov_KPEntities localCont = asonov_KPEntities.GetStockEntity();
            List<Transaction> tr = localCont.Transaction.ToList();

            if (transaction != null) tr = transaction.actualList;
            if (cl != null&&cl!=new Client()) tr = tr.Where(p => p.ID_Client == cl.IDClient).ToList();
            if (place != null) tr = tr.Where(p => p.ID_Place == place.IDPlace).ToList();

            List<Product> products = new List<Product>();

            foreach (Transaction t in tr.Where(p => p.ID_TrTType == 1).ToList())
            {
                if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0)
                {
                    products.Find(p => p.IDProduct == t.ID_Product).Amount += t.Amount;
                }
                else 
                { 
                    products.Add(localCont.Product.Where(p => p.IDProduct == t.ID_Product).First()); products.Last().Amount = t.Amount; 
                }
            }
            foreach (Transaction t in tr.Where(p => p.ID_TrTType == 2).ToList())
            {
                if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0) products.Find(p => p.IDProduct == t.ID_Product).Amount -= t.Amount;
            }
            return products;
        }

        public static List<pControl> pControlConvert(transactionControll trC = null)
        {
            asonov_KPEntities localCont = asonov_KPEntities.GetStockEntity();

            List<Transaction> tr = localCont.Transaction.ToList();

            var cultureInfo = new CultureInfo("ru-RU");
            int c = 1;
            List<pControl> products = new List<pControl>();
            if (trC == null)
            {
                foreach (Transaction t in tr.Where(p => p.ID_TrTType == 1).ToList())
                {
                    if (products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).Count() != 0)
                    {
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().Amount += t.Amount;
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().actualArivalDate = DateTime.ParseExact(t.Date, "G", cultureInfo);
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().ArivalDate = t.Date;
                    }
                    else
                    {
                        products.Add
                        (new pControl
                        {
                            Id = c++,
                            prodId = t.ID_Product,
                            trId = t.IDTransaction,
                            Client = t.Client.Name,
                            ArivalDate = t.Date,
                            actualArivalDate = DateTime.ParseExact(t.Date, "G", cultureInfo),
                            Amount = t.Amount,
                            idPlace = t.ID_Place,
                            Place = localCont.Place.Where(p => p.IDPlace == t.ID_Place).First().SpecialCode,
                            article = t.Product.Article
                        }
                        );
                    }
                }
                foreach (Transaction t in tr.Where(p => p.ID_TrTType == 2).ToList())
                {
                    products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().Amount -= t.Amount;
                }
            }
            else
            {
                foreach (Transaction t in trC.actualList)
                {
                    if (products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).Count() != 0)
                    {
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().Amount += t.ID_TrTType == 1 ? t.Amount : -1 * t.Amount;
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().actualArivalDate = DateTime.ParseExact(t.Date, "G", cultureInfo);
                        products.Where(p => p.Client == t.Client.Name && p.idPlace == t.ID_Place && p.prodId == t.ID_Product).First().ArivalDate = t.Date;
                    }
                    else
                    {
                        products.Add
                        (new pControl
                        {
                            Id = c++,
                            prodId = t.ID_Product,
                            trId = t.IDTransaction,
                            Client = t.Client.Name,
                            ArivalDate = t.Date,
                            actualArivalDate = DateTime.ParseExact(t.Date, "G", cultureInfo),
                            Amount = t.ID_TrTType == 1 ? t.Amount : -1 * t.Amount,
                            idPlace = t.ID_Place,
                            Place = localCont.Place.Where(p => p.IDPlace == t.ID_Place).First().SpecialCode
                        }
                        );
                    }
                }
            }
            return products;
        }
        
        public static void wordExport(List<pControl> productExtra, List<Product> product, string filePath, string Head)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application word;

                word = new Microsoft.Office.Interop.Word.Application();

                word.Visible = false;

                var document = word.Documents.Add();

                var p = document.Paragraphs.Add();

                p = document.Paragraphs.Add();

                p.Range.Text = Head;

                p = document.Paragraphs.Add();

                p.Range.Text = "Дата составления: " + DateTime.Now.ToString("G");

                p.Range.InsertParagraphAfter();
                Table table = document.Tables.Add(p.Range, product.Count + 1, 3);

                table.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;


                table.Cell(1, 1).Range.Text = "Название";
                table.Cell(1, 2).Range.Text = "Артикль";
                table.Cell(1, 3).Range.Text = "Количество";
                for (int i = 0; i < product.Count; i++)
                {
                    table.Cell(i + 2, 1).Range.Text = product[i].Name;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Text = product[i].Article.ToString();
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Text = product[i].Amount.ToString();
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                }
                p.Range.InsertParagraphAfter();
                p.Range.Text = "Подробнее";

                p.Range.InsertParagraphAfter();

                Table table1 = document.Tables.Add(p.Range, productExtra.Count + 1, 6);

                table1.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                table1.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                table1.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                table1.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;


                table1.Cell(1, 1).Range.Text = "Название";
                table1.Cell(1, 2).Range.Text = "Артикль";
                table1.Cell(1, 3).Range.Text = "Клиент";
                table1.Cell(1, 4).Range.Text = "Место хранения";
                table1.Cell(1, 5).Range.Text = "Количество";
                table1.Cell(1, 6).Range.Text = "Последняя поставка";
                for (int i = 0; i < product.Count; i++)
                {
                    table1.Cell(i + 2, 1).Range.Text = productExtra[i].prodName;
                    table1.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 1).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 2).Range.Text = productExtra[i].article;
                    table1.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 2).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 3).Range.Text = productExtra[i].Client;
                    table1.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 3).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 4).Range.Text = productExtra[i].Place;
                    table1.Cell(i + 2, 4).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 4).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 4).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 4).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 5).Range.Text = productExtra[i].Amount.ToString();
                    table1.Cell(i + 2, 5).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 5).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 5).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 5).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 6).Range.Text = productExtra[i].ArivalDate;
                    table1.Cell(i + 2, 6).Range.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 6).Range.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 6).Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    table1.Cell(i + 2, 6).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                }
                word.Application.ActiveDocument.SaveAs2(filePath!=""?filePath:"tempdocument.docx", WdSaveFormat.wdFormatDocumentDefault);             
                word.Quit(WdSaveOptions.wdSaveChanges);
            }
            catch
            {
            }
        }
    }
}
