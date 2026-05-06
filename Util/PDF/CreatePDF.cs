using Domains.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Util.PDF.Interfaces;
using Document = iTextSharp.text.Document;

namespace Util.PDF
{
    public class CreatePDF : ICreatePDF
    {
        public byte[] CreateTicketPDFDocument(List<Ticket> tickets)
        {
            using var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();
            foreach (var ticket in tickets)
            {
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                var header = new Paragraph("Ticket", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(header);

                var subtitle = new Paragraph("Bedankt voor jouw aankoop!", valueFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 16f
                };
                doc.Add(subtitle);

                var contentTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };
                contentTable.SetWidths(new float[] { 2f, 1f });

                var detailsTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };
                detailsTable.SetWidths(new float[] { 1f, 2f });

                AddDetailRow(detailsTable, "Ticket ID", ticket.Id.ToString(), labelFont, valueFont);
                AddDetailRow(detailsTable, "Match", $"{ticket.Match?.HomeClub?.Name} vs {ticket.Match?.AwayClub?.Name}", labelFont, valueFont);
                AddDetailRow(detailsTable, "Datum", ticket.Match?.MatchDate.ToString("dd MMM yyyy HH:mm") ?? string.Empty, labelFont, valueFont);
                AddDetailRow(detailsTable, "Stadion", ticket.Vak?.Stadium?.Name ?? string.Empty, labelFont, valueFont);
                AddDetailRow(detailsTable, "Vak", ticket.Vak?.Name ?? string.Empty, labelFont, valueFont);
                AddDetailRow(detailsTable, "Seat", ticket.Seat ?? string.Empty, labelFont, valueFont);
                AddDetailRow(detailsTable, "Prijs", ticket.Vak != null ? $"€{ticket.Vak.Price:0.00}" : string.Empty, labelFont, valueFont);

                var detailsCell = new PdfPCell(detailsTable)
                {
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    PaddingRight = 10f
                };

                var qrCell = new PdfPCell
                {
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                var qrImage = CreateQrCodeImage(ticket.Id.ToString());
                qrCell.AddElement(qrImage);

                contentTable.AddCell(detailsCell);
                contentTable.AddCell(qrCell);

                doc.Add(contentTable);

                if (ticket != tickets.Last())
                {
                    doc.NewPage();
                }
            }

            doc.Close();
            return ms.ToArray();
        }

        // This method is for converting bitmap into a byte array
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private static void AddDetailRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                PaddingBottom = 6f
            };
            var valueCell = new PdfPCell(new Phrase(value, valueFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                PaddingBottom = 6f
            };

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private static iTextSharp.text.Image CreateQrCodeImage(string qrText)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            using var qrBitmap = qrCode.GetGraphic(20);
            var qrBytes = BitmapToBytes(qrBitmap);
            var qrImage = iTextSharp.text.Image.GetInstance(qrBytes);
            qrImage.ScaleToFit(200f, 200f);
            qrImage.Alignment = Element.ALIGN_CENTER;
            return qrImage;
        }
    }
}
