using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebProject.Models
{
    public static class ExportHelper
    {
        private static Font GetTurkishFont(float size, int style = Font.NORMAL)
        {
            try
            {
                string arialPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                if (File.Exists(arialPath))
                {
                    BaseFont bf = BaseFont.CreateFont(arialPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    return new Font(bf, size, style);
                }
            }
            catch { }
            return new Font(Font.HELVETICA, size, style);
        }

        // ========================================================
        // FİLMLER EXCEL & PDF
        // ========================================================

        public static byte[] ExportFilmsToExcel(IEnumerable<Film> films)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Filmler");
                
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Film Adı";
                worksheet.Cell(1, 3).Value = "Tür";
                worksheet.Cell(1, 4).Value = "Süre (Dakika)";
                worksheet.Cell(1, 5).Value = "Yapım Yılı";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in films)
                {
                    worksheet.Cell(row, 1).Value = item.FilmId;
                    worksheet.Cell(row, 2).Value = item.FilmAd;
                    worksheet.Cell(row, 3).Value = item.Tur;
                    worksheet.Cell(row, 4).Value = item.Sure;
                    worksheet.Cell(row, 5).Value = item.YapimYili;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportFilmsToPdf(IEnumerable<Film> films)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = GetTurkishFont(16, Font.BOLD);
                Font headerFont = GetTurkishFont(10, Font.BOLD);
                Font cellFont = GetTurkishFont(9, Font.NORMAL);

                Paragraph title = new Paragraph("Film Listesi Raporu", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 35f, 25f, 15f, 15f });

                string[] headers = { "ID", "Film Adı", "Tür", "Süre (dk)", "Yapım Yılı" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = new BaseColor(30, 41, 59);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 8;
                    cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
                    table.AddCell(cell);
                }

                foreach (var item in films)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.FilmId.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.FilmAd, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Tur, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Sure.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.YapimYili.ToString(), cellFont)) { Padding = 6 });
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }

        // ========================================================
        // DİZİLER EXCEL & PDF
        // ========================================================

        public static byte[] ExportDizilerToExcel(IEnumerable<Dizi> diziler)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Diziler");
                
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Dizi Adı";
                worksheet.Cell(1, 3).Value = "Tür";
                worksheet.Cell(1, 4).Value = "Bölüm Sayısı";
                worksheet.Cell(1, 5).Value = "Yapım Yılı";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in diziler)
                {
                    worksheet.Cell(row, 1).Value = item.DiziId;
                    worksheet.Cell(row, 2).Value = item.DiziAd;
                    worksheet.Cell(row, 3).Value = item.Tur;
                    worksheet.Cell(row, 4).Value = item.BolumSayisi;
                    worksheet.Cell(row, 5).Value = item.YapimYili;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportDizilerToPdf(IEnumerable<Dizi> diziler)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = GetTurkishFont(16, Font.BOLD);
                Font headerFont = GetTurkishFont(10, Font.BOLD);
                Font cellFont = GetTurkishFont(9, Font.NORMAL);

                Paragraph title = new Paragraph("Dizi Listesi Raporu", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 35f, 25f, 15f, 15f });

                string[] headers = { "ID", "Dizi Adı", "Tür", "Bölüm Sayısı", "Yapım Yılı" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = new BaseColor(30, 41, 59);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 8;
                    cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
                    table.AddCell(cell);
                }

                foreach (var item in diziler)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.DiziId.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.DiziAd, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Tur, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.BolumSayisi.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.YapimYili.ToString(), cellFont)) { Padding = 6 });
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }

        // ========================================================
        // ÇİZGİ FİLMLER EXCEL & PDF
        // ========================================================

        public static byte[] ExportCizgiFilmlerToExcel(IEnumerable<CizgiFilm> cizgiFilmler)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("CizgiFilmler");
                
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Çizgi Film Adı";
                worksheet.Cell(1, 3).Value = "Tür";
                worksheet.Cell(1, 4).Value = "Bölüm Sayısı";
                worksheet.Cell(1, 5).Value = "Yaş Aralığı";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in cizgiFilmler)
                {
                    worksheet.Cell(row, 1).Value = item.Id;
                    worksheet.Cell(row, 2).Value = item.Ad;
                    worksheet.Cell(row, 3).Value = item.Tur;
                    worksheet.Cell(row, 4).Value = item.BolumSayisi;
                    worksheet.Cell(row, 5).Value = item.YasAraligi;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportCizgiFilmlerToPdf(IEnumerable<CizgiFilm> cizgiFilmler)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = GetTurkishFont(16, Font.BOLD);
                Font headerFont = GetTurkishFont(10, Font.BOLD);
                Font cellFont = GetTurkishFont(9, Font.NORMAL);

                Paragraph title = new Paragraph("Çizgi Film Listesi Raporu", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 35f, 25f, 15f, 15f });

                string[] headers = { "ID", "Çizgi Film Adı", "Tür", "Bölüm Sayısı", "Yaş Aralığı" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = new BaseColor(30, 41, 59);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 8;
                    cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
                    table.AddCell(cell);
                }

                foreach (var item in cizgiFilmler)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.Id.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Ad, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Tur, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.BolumSayisi.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.YasAraligi, cellFont)) { Padding = 6 });
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }
    }
}
