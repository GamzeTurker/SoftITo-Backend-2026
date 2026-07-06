using ClosedXML.Excel;
using dpperUygulama.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dpperUygulama.Models
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
            return new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, size, style);
        }

        // ========================================================
        // ARAÇLAR (VEHICLES) EXCEL & PDF
        // ========================================================

        public static byte[] ExportVehiclesToExcel(IEnumerable<Vehicle> vehicles, IEnumerable<Customer> customers)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Araçlar");
                
                // Başlıklar
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Marka";
                worksheet.Cell(1, 3).Value = "Model";
                worksheet.Cell(1, 4).Value = "Plaka";
                worksheet.Cell(1, 5).Value = "Müşteri Adı";

                // Başlık stil
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in vehicles)
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == item.CustomerId);
                    worksheet.Cell(row, 1).Value = item.VehicleId;
                    worksheet.Cell(row, 2).Value = item.Brand;
                    worksheet.Cell(row, 3).Value = item.Model;
                    worksheet.Cell(row, 4).Value = item.Plate;
                    worksheet.Cell(row, 5).Value = customer?.FullName ?? "";
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

        public static byte[] ExportVehiclesToPdf(IEnumerable<Vehicle> vehicles, IEnumerable<Customer> customers)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = GetTurkishFont(16, Font.BOLD);
                Font headerFont = GetTurkishFont(10, Font.BOLD);
                Font cellFont = GetTurkishFont(9, Font.NORMAL);

                Paragraph title = new Paragraph("Araç Listesi Raporu", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 22f, 22f, 20f, 26f });

                // Başlık hücreleri
                string[] headers = { "ID", "Marka", "Model", "Plaka", "Müşteri" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = new BaseColor(30, 41, 59);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 8;
                    cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
                    table.AddCell(cell);
                }

                foreach (var item in vehicles)
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == item.CustomerId);
                    table.AddCell(new PdfPCell(new Phrase(item.VehicleId.ToString(), cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Brand, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Model, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(item.Plate, cellFont)) { Padding = 6 });
                    table.AddCell(new PdfPCell(new Phrase(customer?.FullName ?? "", cellFont)) { Padding = 6 });
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }

        // ========================================================
        // SERVİS KAYITLARI (SERVICE RECORDS) EXCEL & PDF
        // ========================================================

        public static byte[] ExportServicesToExcel(IEnumerable<ServiceRecord> services)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Servis Kayıtları");
                
                // Başlıklar
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Müşteri";
                worksheet.Cell(1, 3).Value = "Araç";
                worksheet.Cell(1, 4).Value = "Plaka";
                worksheet.Cell(1, 5).Value = "Personel";
                worksheet.Cell(1, 6).Value = "Tarih";
                worksheet.Cell(1, 7).Value = "Açıklama";
                worksheet.Cell(1, 8).Value = "Ücret (₺)";

                // Başlık stil
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                int row = 2;
                foreach (var item in services)
                {
                    worksheet.Cell(row, 1).Value = item.ServiceId;
                    worksheet.Cell(row, 2).Value = item.FullName ?? "";
                    worksheet.Cell(row, 3).Value = item.Brand ?? "";
                    worksheet.Cell(row, 4).Value = item.Plate ?? "";
                    worksheet.Cell(row, 5).Value = item.EmployeeName ?? "";
                    worksheet.Cell(row, 6).Value = item.ServiceDate.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 7).Value = item.Description ?? "";
                    worksheet.Cell(row, 8).Value = item.Cost;
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

        public static byte[] ExportServicesToPdf(IEnumerable<ServiceRecord> services)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30); // Yatay sayfa
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = GetTurkishFont(16, Font.BOLD);
                Font headerFont = GetTurkishFont(9, Font.BOLD);
                Font cellFont = GetTurkishFont(8, Font.NORMAL);

                Paragraph title = new Paragraph("Servis Kayıtları Raporu", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 6f, 14f, 12f, 10f, 14f, 10f, 24f, 10f });

                // Başlık hücreleri
                string[] headers = { "ID", "Müşteri", "Araç", "Plaka", "Personel", "Tarih", "Açıklama", "Ücret" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));
                    cell.BackgroundColor = new BaseColor(30, 41, 59);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 6;
                    cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
                    table.AddCell(cell);
                }

                foreach (var item in services)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.ServiceId.ToString(), cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.FullName ?? "", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.Brand ?? "", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.Plate ?? "", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.EmployeeName ?? "", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.ServiceDate.ToString("dd.MM.yyyy"), cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.Description ?? "", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(item.Cost.ToString("F2") + " TL", cellFont)) { Padding = 4 });
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }
    }
}
