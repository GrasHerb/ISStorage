﻿using IS_Storage.workViews;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
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
        public static void amountCount()
        {
            stockEntities localCont = stockEntities.GetStockEntity();
            var transactions = localCont.Transaction.Where(p => p.ID_TrTType != 3).ToList();


            List<Product> products = new List<Product>();

            foreach (var t in transactions)
            {
                if (t.ID_TrTType == 1)
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
                else
                {
                    if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0) products.Find(p => p.IDProduct == t.ID_Product).Amount -= t.Amount;
                }
                if (t.ID_TrTType == 3)
                    if (products.Where(p => p.IDProduct == t.ID_Product).Count() > 0)
                    {
                        products.Find(p => p.IDProduct == t.ID_Product).Amount = 0;
                    }
                    else
                    {
                        products.Add(localCont.Product.Where(p => p.IDProduct == t.ID_Product).First()); products.Last().Amount = 0;
                    }
            }
            localCont.SaveChanges();
        }

        public static List<Product> ProductsSearch(transactionControll transaction = null, Client cl = null, Place place = null, int type = 0)
        {
            stockEntities localCont = stockEntities.GetStockEntityD();
            List<Transaction> tr = localCont.Transaction.ToList();

            if (transaction != null) tr = transaction.actualList;
            if (cl != null && cl != new Client()) tr = tr.Where(p => p.ID_Client == cl.IDClient).ToList();
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
            if (type == 1)
                foreach (Product pr in localCont.Product) if (products.Where(p => p.IDProduct == pr.IDProduct).Count() == 0) { products.Add(localCont.Product.Where(p => p.IDProduct == pr.IDProduct).First()); products.Last().Amount = 0; }
            return products.Where(p => !p.Name.Contains("___")).ToList();
        }

        public static List<pControl> pControlConvert(transactionControll trC = null)
        {
            stockEntities localCont = stockEntities.GetStockEntityD();

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
                            prodName = t.Product.Name,
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
                            prodName = t.Product.Name,
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
                Object defaultTableBehavior = Type.Missing;
                Object autoFitBehavior = Type.Missing;

                Microsoft.Office.Interop.Word.Application word;

                word = new Microsoft.Office.Interop.Word.Application();

                word.Visible = false;

                var document = word.Documents.Add();

                object oCollapseEnd = WdCollapseDirection.wdCollapseEnd;

                Microsoft.Office.Interop.Word.Range rng = document.Range();
                rng.Collapse(ref oCollapseEnd);

                rng.InsertParagraphAfter();
                rng.Collapse(ref oCollapseEnd);

                rng.Text += Head;

                rng.InsertParagraphAfter();
                rng.Collapse(ref oCollapseEnd);

                rng.Text = "Дата составления: " + DateTime.Now.ToString("G");

                rng.InsertParagraphAfter();
                rng.Collapse(ref oCollapseEnd);

                Table table = document.Tables.Add(rng, product.Count + 1, 3, ref defaultTableBehavior, ref autoFitBehavior);



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

                rng = document.Content;

                rng.Collapse(ref oCollapseEnd);


                rng.Text = "Подробнее";

                rng.InsertParagraphAfter();
                rng.Collapse(ref oCollapseEnd);

                Table table1 = document.Tables.Add(rng, productExtra.Count + 1, 6, ref defaultTableBehavior, ref autoFitBehavior);

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
                word.Application.ActiveDocument.SaveAs2(filePath != "" ? filePath : "tempdocument.docx", WdSaveFormat.wdFormatDocumentDefault);
                word.Quit(WdSaveOptions.wdSaveChanges);
            }
            catch
            {
            }
        }
    }
}
