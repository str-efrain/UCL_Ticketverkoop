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

            // Landscape A4
            var pageSize = new iTextSharp.text.Rectangle(PageSize.A4.Height, PageSize.A4.Width);
            var doc = new Document(pageSize, 0, 0, 0, 0);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            foreach (var ticket in tickets)
            {
                RenderTicketPage(doc, ticket);
                if (ticket != tickets.Last())
                    doc.NewPage();
            }

            doc.Close();
            return ms.ToArray();
        }

        public byte[] CreateAbonnementPDFDocument(List<Abonnement> abonnements)
        {
            using var ms = new MemoryStream();

            var pageSize = new iTextSharp.text.Rectangle(PageSize.A4.Height, PageSize.A4.Width);
            var doc = new Document(pageSize, 0, 0, 0, 0);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            foreach (var sub in abonnements)
            {
                RenderAbonnementPage(doc, sub);
                if (sub != abonnements.Last())
                    doc.NewPage();
            }

            doc.Close();
            return ms.ToArray();
        }

        // ── Ticket page ───────────────────────────────────────────────
        private static void RenderTicketPage(Document doc, Ticket ticket)
        {
            float pageW = doc.PageSize.Width;
            float pageH = doc.PageSize.Height;

            // Colors
            var navyDark = new BaseColor(11, 21, 48);
            var navy = new BaseColor(17, 32, 96);
            var gold = new BaseColor(212, 175, 55);
            var white = BaseColor.White;
            var mutedBlue = new BaseColor(143, 163, 208);

            // Fonts
            var fontBrand = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, white);
            var fontBig = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, white);
            var fontMed = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, white);
            var fontSmall = FontFactory.GetFont(FontFactory.HELVETICA, 10, white);
            var fontLabel = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, mutedBlue);
            var fontValue = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, white);
            var fontGold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, gold);
            var fontGoldSm = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, gold);

            // ── PdfPTable as full-page canvas ─────────────────────────
            // Two columns: left body (wide) + right stub (narrow)
            float stubW = 160f;
            float bodyW = pageW - stubW;

            var page = new PdfPTable(2);
            page.TotalWidth = pageW;
            page.LockedWidth = true;
            page.SetWidths(new float[] { bodyW, stubW });

            // ── LEFT: main body ───────────────────────────────────────
            var body = new PdfPTable(1);
            body.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            // Top bar: brand + UCL label
            var topBar = new PdfPTable(2);
            topBar.SetWidths(new float[] { 1f, 1f });
            topBar.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            topBar.DefaultCell.BackgroundColor = navyDark;
            topBar.DefaultCell.PaddingTop = 18f;
            topBar.DefaultCell.PaddingBottom = 12f;
            topBar.DefaultCell.PaddingLeft = 28f;
            topBar.DefaultCell.PaddingRight = 28f;

            var brandPhrase = new Phrase();
            brandPhrase.Add(new Chunk("EfraThib", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, white)));
            brandPhrase.Add(new Chunk("_Ticketing", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, gold)));
            var brandCell = new PdfPCell(brandPhrase)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                PaddingTop = 18f,
                PaddingBottom = 12f,
                PaddingLeft = 28f
            };
            topBar.AddCell(brandCell);

            var uclCell = new PdfPCell(new Phrase("UEFA CHAMPIONS LEAGUE", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 18f,
                PaddingBottom = 12f,
                PaddingRight = 28f
            };
            topBar.AddCell(uclCell);

            var topBarCell = new PdfPCell(topBar)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 0f
            };
            body.AddCell(topBarCell);

            // Gold divider
            body.AddCell(GoldDividerCell(bodyW, gold));

            // Match title area (navy gradient feel)
            string homeTeam = ticket.Match?.HomeClub?.Name ?? "—";
            string awayTeam = ticket.Match?.AwayClub?.Name ?? "—";
            string matchStr = $"{homeTeam}  vs  {awayTeam}";

            var matchCell = new PdfPCell(new Phrase(matchStr, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, white)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                PaddingTop = 24f,
                PaddingBottom = 8f,
                PaddingLeft = 28f,
                PaddingRight = 28f,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            body.AddCell(matchCell);

            // Date + stadium
            string dateStr = ticket.Match?.MatchDate.ToString("dddd d MMMM yyyy  ·  HH:mm") ?? "—";
            string stadiumStr = ticket.Vak?.Stadium?.Name ?? "—";

            var subMatchCell = new PdfPCell(new Phrase($"{dateStr}    {stadiumStr}", FontFactory.GetFont(FontFactory.HELVETICA, 10, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                PaddingBottom = 24f,
                PaddingLeft = 28f,
                PaddingRight = 28f
            };
            body.AddCell(subMatchCell);

            // Detail row: Section / Ring / Seat / Price
            var details = new PdfPTable(4);
            details.SetWidths(new float[] { 1f, 1f, 1f, 1f });
            details.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            details.DefaultCell.BackgroundColor = navyDark;

            string ring = !string.IsNullOrEmpty(ticket.Vak?.Ring) ? ticket.Vak.Ring : "—";

            AddDetailBlock(details, "SECTION", ticket.Vak?.Name ?? "—", navyDark, fontLabel, fontValue);
            AddDetailBlock(details, "RING", ring, navyDark, fontLabel, fontValue);
            AddDetailBlock(details, "SEAT", ticket.Seat ?? "—", navyDark, fontLabel, fontGold);
            AddDetailBlock(details, "PRICE", $"€{ticket.Vak?.Price:0.##}", navyDark, fontLabel, fontGold);

            var detailsCell = new PdfPCell(details)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 0f
            };
            body.AddCell(detailsCell);

            // Bottom bar: ticket ID
            var bottomBar = new PdfPCell(new Phrase($"TICKET  #  {ticket.Id:D8}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                PaddingTop = 10f,
                PaddingBottom = 10f,
                PaddingLeft = 28f,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            body.AddCell(bottomBar);

            // Fill remaining height
            body.AddCell(FillerCell(navyDark));

            var bodyOuter = new PdfPCell(body)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 0f,
                FixedHeight = pageH
            };
            page.AddCell(bodyOuter);

            // ── RIGHT: stub ───────────────────────────────────────────
            var stub = new PdfPTable(1);
            stub.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            // Stub top band
            stub.AddCell(new PdfPCell(new Phrase(" "))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = gold,
                FixedHeight = 6f
            });

            // Rotated match label
            var stubTitle = new PdfPCell(new Phrase($"{homeTeam} vs {awayTeam}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, white)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                PaddingTop = 20f,
                PaddingBottom = 10f,
                PaddingLeft = 12f,
                PaddingRight = 12f,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            stub.AddCell(stubTitle);

            // Dashed separator line
            stub.AddCell(DashedSeparatorCell(stubW, mutedBlue));

            // QR code
            var qrImage = CreateQrCodeImage(ticket.Id.ToString());
            qrImage.ScaleToFit(120f, 120f);
            var qrCell = new PdfPCell(iTextSharp.text.Image.GetInstance(qrImage))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 16f,
                PaddingBottom = 8f,
                PaddingLeft = 12f,
                PaddingRight = 12f
            };
            stub.AddCell(qrCell);

            // Seat on stub
            stub.AddCell(new PdfPCell(new Phrase("SEAT", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 6f
            });
            stub.AddCell(new PdfPCell(new Phrase(ticket.Seat ?? "—", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 16f
            });

            // Dashed separator
            stub.AddCell(DashedSeparatorCell(stubW, mutedBlue));

            // Price on stub
            stub.AddCell(new PdfPCell(new Phrase("PRICE", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 10f
            });
            stub.AddCell(new PdfPCell(new Phrase($"€{ticket.Vak?.Price:0.##}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 8f
            });

            // Filler
            stub.AddCell(FillerCell(navyDark));

            var stubOuter = new PdfPCell(stub)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 0f,
                FixedHeight = pageH
            };
            page.AddCell(stubOuter);

            doc.Add(page);
        }

        // ── Season pass page ──────────────────────────────────────────
        private static void RenderAbonnementPage(Document doc, Abonnement sub)
        {
            float pageW = doc.PageSize.Width;
            float pageH = doc.PageSize.Height;

            var navyDark = new BaseColor(11, 21, 48);
            var navy = new BaseColor(17, 32, 96);
            var gold = new BaseColor(212, 175, 55);
            var white = BaseColor.White;
            var mutedBlue = new BaseColor(143, 163, 208);

            float stubW = 160f;
            float bodyW = pageW - stubW;

            var page = new PdfPTable(2);
            page.TotalWidth = pageW;
            page.LockedWidth = true;
            page.SetWidths(new float[] { bodyW, stubW });

            // ── LEFT body ─────────────────────────────────────────────
            var body = new PdfPTable(1);
            body.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            // Top bar
            var topBar = new PdfPTable(2);
            topBar.SetWidths(new float[] { 1f, 1f });
            topBar.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            topBar.DefaultCell.BackgroundColor = navyDark;

            var brandPhrase = new Phrase();
            brandPhrase.Add(new Chunk("EfraThib", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, white)));
            brandPhrase.Add(new Chunk("_Ticketing", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, gold)));
            topBar.AddCell(new PdfPCell(brandPhrase)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                PaddingTop = 18f,
                PaddingBottom = 12f,
                PaddingLeft = 28f
            });
            topBar.AddCell(new PdfPCell(new Phrase("SEASON PASS", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 18f,
                PaddingBottom = 12f,
                PaddingRight = 28f
            });

            body.AddCell(new PdfPCell(topBar) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 0f });
            body.AddCell(GoldDividerCell(bodyW, gold));

            // Club + season name
            string clubName = sub.Club?.Name ?? "—";
            string seizoen = sub.Seizoen?.Name ?? "—";

            body.AddCell(new PdfPCell(new Phrase(clubName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, white)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                PaddingTop = 24f,
                PaddingBottom = 6f,
                PaddingLeft = 28f,
                PaddingRight = 28f
            });

            body.AddCell(new PdfPCell(new Phrase(seizoen, FontFactory.GetFont(FontFactory.HELVETICA, 11, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                PaddingBottom = 24f,
                PaddingLeft = 28f,
                PaddingRight = 28f
            });

            // Detail blocks
            var details = new PdfPTable(4);
            details.SetWidths(new float[] { 1f, 1f, 1f, 1f });
            details.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            details.DefaultCell.BackgroundColor = navyDark;

            string ring = !string.IsNullOrEmpty(sub.Vak?.Ring) ? sub.Vak.Ring : "—";
            var fontLabel = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, mutedBlue);
            var fontValue = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, white);
            var fontGold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, gold);

            AddDetailBlock(details, "SECTION", sub.Vak?.Name ?? "—", navyDark, fontLabel, fontValue);
            AddDetailBlock(details, "RING", ring, navyDark, fontLabel, fontValue);
            AddDetailBlock(details, "SEAT", sub.Seat, navyDark, fontLabel, fontGold);
            AddDetailBlock(details, "PRICE / MATCH", $"€{sub.Vak?.SubscriptionPrice:0.##}", navyDark, fontLabel, fontGold);

            body.AddCell(new PdfPCell(details) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 0f });

            // Bottom
            body.AddCell(new PdfPCell(new Phrase($"SEASON PASS  #  {sub.Id:D8}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                PaddingTop = 10f,
                PaddingBottom = 10f,
                PaddingLeft = 28f
            });

            body.AddCell(FillerCell(navyDark));

            page.AddCell(new PdfPCell(body) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 0f, FixedHeight = pageH });

            // ── RIGHT stub ────────────────────────────────────────────
            var stub = new PdfPTable(1);
            stub.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            stub.AddCell(new PdfPCell(new Phrase(" ")) { Border = iTextSharp.text.Rectangle.NO_BORDER, BackgroundColor = gold, FixedHeight = 6f });

            stub.AddCell(new PdfPCell(new Phrase(clubName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, white)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 20f,
                PaddingBottom = 10f,
                PaddingLeft = 12f,
                PaddingRight = 12f
            });

            stub.AddCell(DashedSeparatorCell(stubW, mutedBlue));

            var qrImage = CreateQrCodeImage(sub.Id.ToString());
            qrImage.ScaleToFit(120f, 120f);
            stub.AddCell(new PdfPCell(iTextSharp.text.Image.GetInstance(qrImage))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 16f,
                PaddingBottom = 8f,
                PaddingLeft = 12f,
                PaddingRight = 12f
            });

            stub.AddCell(new PdfPCell(new Phrase("SEAT", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 6f
            });
            stub.AddCell(new PdfPCell(new Phrase(sub.Seat ?? "—", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navy,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 16f
            });

            stub.AddCell(DashedSeparatorCell(stubW, mutedBlue));

            stub.AddCell(new PdfPCell(new Phrase("PRICE / MATCH", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, mutedBlue)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 10f
            });
            stub.AddCell(new PdfPCell(new Phrase($"€{sub.Vak?.SubscriptionPrice:0.##}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, gold)))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = navyDark,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 8f
            });

            stub.AddCell(FillerCell(navyDark));

            page.AddCell(new PdfPCell(stub) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 0f, FixedHeight = pageH });

            doc.Add(page);
        }

        // ── Shared helpers ────────────────────────────────────────────
        private static void AddDetailBlock(PdfPTable table, string label, string value, BaseColor bg, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            var cell = new PdfPTable(1);
            cell.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.DefaultCell.BackgroundColor = bg;

            cell.AddCell(new PdfPCell(new Phrase(label, labelFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = bg,
                PaddingLeft = 28f,
                PaddingTop = 16f,
                PaddingBottom = 4f
            });
            cell.AddCell(new PdfPCell(new Phrase(value, valueFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = bg,
                PaddingLeft = 28f,
                PaddingBottom = 16f
            });

            table.AddCell(new PdfPCell(cell)
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 0f,
                BackgroundColor = bg
            });
        }

        private static PdfPCell GoldDividerCell(float width, BaseColor gold)
        {
            return new PdfPCell(new Phrase(" "))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = gold,
                FixedHeight = 4f,
                Colspan = 1
            };
        }

        private static PdfPCell DashedSeparatorCell(float width, BaseColor color)
        {
            // Simulate dashed line with a thin coloured bar
            return new PdfPCell(new Phrase(" "))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = color,
                FixedHeight = 1f,
                PaddingLeft = 12f,
                PaddingRight = 12f
            };
        }

        private static PdfPCell FillerCell(BaseColor bg)
        {
            return new PdfPCell(new Phrase(" "))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                BackgroundColor = bg,
                MinimumHeight = 400f
            };
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

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
