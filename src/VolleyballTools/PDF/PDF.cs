using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Reflection;
using VolleyballTools.PDF;

namespace VolleyballTools.PDF
{
    public class PDF
    {
        public PDF()
        {
            GlobalFontSettings.FontResolver = new JapaneseFontResolver();
        }

        enum ScoresheetTemplate
        {
            three,
            five
        }
        private static Stream Template(ScoresheetTemplate set)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (set == ScoresheetTemplate.three)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.3SET.pdf")??throw new ArgumentException("3SET.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
            else
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.5SET.pdf")??throw new ArgumentException("5SET.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
        }

        public XFont AutoFontSize(string text, double maxSize, double width, XGraphics gfx)
        {
            for (double i = maxSize; i >= 0; i-=0.5)
            {
                var font = new XFont("notosans", i, XFontStyle.Regular);
                var size = gfx.MeasureString(text, font);
                if (size.Width < width)
                {
                    return font;
                }
            }
            throw new Exception();
        }
        public Stream Generate3SET(string? MatchName = null, string? Venue = null, string? Hall = null, DateTime? Date = null, string? MatchNumber = null, string? ATeam = null, string? BTeam = null, bool? isMen = null, DateTime? MatchTime = null)
        {
            var pdfdoc = new PdfDocument();

            PdfDocument inputDocument = PdfReader.Open(Template(ScoresheetTemplate.three), PdfDocumentOpenMode.Import);
            var template = inputDocument.Pages[0];
            template.Size = PdfSharpCore.PageSize.A4;
            template.Orientation = PdfSharpCore.PageOrientation.Landscape;

            //var page = pdfdoc.AddPage();
            var page = pdfdoc.AddPage(template);
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
            page.Size = PdfSharpCore.PageSize.A3;
            page.Rotate = 0;

            var gfx = XGraphics.FromPdfPage(page);
            gfx.RotateAtTransform(-90, new XPoint(0, 0));
            gfx.TranslateTransform(-842, 0);

            var textColor = XBrushes.Black;
            var format = XStringFormats.CenterLeft;


            //gfx.DrawRectangle(textColor, new XRect(650, 180, 55, 15));

            if (MatchName is not null)
            {
                gfx.DrawString(MatchName,
                    AutoFontSize(MatchName, 15, 500, gfx),
                    textColor,
                    new XRect(60, 35, 500, 25),
                    format);
            }

            if (Venue is not null)
            {
                gfx.DrawString(Venue,
                    AutoFontSize(Venue, 10, 190, gfx),
                    textColor,
                    new XRect(60, 65, 190, 15),
                    format);
            }

            if (Hall is not null)
            {
                gfx.DrawString(Hall,

                    AutoFontSize(Hall, 10, 180, gfx),
                textColor,
                    new XRect(60, 85, 180, 15),
                    format);

            }

            if (MatchNumber is not null)
            {
                gfx.DrawString(MatchNumber,
                    AutoFontSize(MatchNumber, 10, 60, gfx),
                    textColor,
                    new XRect(280, 65, 60, 15),
                    XStringFormats.Center);
            }

            if (Date is not null)
            {
                gfx.DrawString(Date.Value.Year.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(360, 65, 30, 15),
                   XStringFormats.BottomCenter);

                gfx.DrawString(Date.Value.Month.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(395, 65, 30, 15),
                   XStringFormats.BottomCenter);

                gfx.DrawString(Date.Value.Day.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(430, 65, 30, 15),
                   XStringFormats.BottomCenter);
            }

            if (ATeam is not null)
            {
                gfx.DrawString(ATeam,
                    AutoFontSize(ATeam, 8, 90, gfx),
                   textColor,
                   new XRect(405, 85, 90, 15),
                   XStringFormats.Center);

                gfx.DrawString(ATeam,
                    AutoFontSize(ATeam, 8, 55, gfx),
                   textColor,
                   new XRect(650, 180, 55, 15),
                   XStringFormats.Center);
            }
            if (BTeam is not null)
            {
                gfx.DrawString(BTeam,
                    AutoFontSize(BTeam, 8, 90, gfx),
                   textColor,
                   new XRect(515, 85, 90, 15),
                   XStringFormats.Center);

                gfx.DrawString(BTeam,
                    AutoFontSize(BTeam, 8, 55, gfx),
                   textColor,
                   new XRect(735, 180, 55, 15),
                   XStringFormats.Center);
            }

            if (isMen is not null)
            {
                if ((bool)isMen)
                {
                    //男子
                    gfx.DrawLine(new XPen(textColor), 290, 98, 279, 88);
                    gfx.DrawLine(new XPen(textColor), 290, 88, 279, 98);
                }
                else
                {
                    //女子
                    gfx.DrawLine(new XPen(textColor), 313, 98, 302, 88);
                    gfx.DrawLine(new XPen(textColor), 313, 88, 302, 98);
                }
            }

            if (MatchTime is not null)
            {

                gfx.DrawString(MatchTime.Value.Hour.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(525, 65, 20, 15),
                   XStringFormats.Center);

                gfx.DrawString(MatchTime.Value.Minute.ToString("00"),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(565, 65, 20, 15),
                   XStringFormats.Center);
            }
            pdfdoc.Pages[0].Rotate = 90;

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        public Stream Generate5SET(string? MatchName = null, string? Venue = null, string? Hall = null, DateTime? Date = null, string? MatchNumber = null, string? ATeam = null, string? BTeam = null, bool? isMen = null, DateTime? MatchTime = null)
        {
            var pdfdoc = new PdfDocument();

            PdfDocument inputDocument = PdfReader.Open(Template(ScoresheetTemplate.five), PdfDocumentOpenMode.Import);
            var template = inputDocument.Pages[0];
            template.Size = PdfSharpCore.PageSize.A4;
            template.Orientation = PdfSharpCore.PageOrientation.Landscape;

            //var page = pdfdoc.AddPage();
            var page = pdfdoc.AddPage(template);
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
            page.Size = PdfSharpCore.PageSize.A3;
            page.Rotate = 0;

            var gfx = XGraphics.FromPdfPage(page);
            gfx.RotateAtTransform(-90, new XPoint(0, 0));
            gfx.TranslateTransform(-842, 0);

            var textColor = XBrushes.Black;
            var format = XStringFormats.CenterLeft;

            if (MatchName is not null)
            {
                gfx.DrawString(MatchName,
                    AutoFontSize(MatchName, 8, 420, gfx),
                    textColor,
                    new XRect(85, 34, 420, 12),
                    format);
            }


            if (Venue is not null)
            {
                gfx.DrawString(Venue,

                    AutoFontSize(Venue, 8, 110, gfx),
                textColor,
                    new XRect(55, 48, 110, 12),
                    format);
            }

            if (Hall is not null)
            {
                gfx.DrawString(Hall,

                    AutoFontSize(Hall, 8, 110, gfx),
                textColor,
                    new XRect(55, 62, 110, 12),
                    format);

            }

            if (MatchNumber is not null)
            {
                gfx.DrawString(MatchNumber,

                    AutoFontSize(MatchNumber, 8, 25, gfx),
                    textColor,
                    new XRect(210, 48, 25, 12),
                    XStringFormats.Center);
            }

            if (Date is not null)
            {
                gfx.DrawString(Date.Value.Year.ToString(),
                   new XFont("notosans", 6, XFontStyle.Regular),
                   textColor,
                   new XRect(208, 62, 16, 12),
                   XStringFormats.Center);

                gfx.DrawString(Date.Value.Month.ToString(),
                   new XFont("notosans", 6, XFontStyle.Regular),
                   textColor,
                   new XRect(232, 62, 14, 12),
                   XStringFormats.Center);

                gfx.DrawString(Date.Value.Day.ToString(),
                   new XFont("notosans", 6, XFontStyle.Regular),
                   textColor,
                   new XRect(252, 62, 14, 12),
                   XStringFormats.Center);
            }

            if (ATeam is not null)
            {
                gfx.DrawString(ATeam,

                    AutoFontSize(ATeam, 8, 84, gfx),
                   textColor,
                   new XRect(305, 67, 84, 12),
                   XStringFormats.Center);

                gfx.DrawString(ATeam,

                    AutoFontSize(ATeam, 5, 65, gfx),
                   textColor,
                   new XRect(660, 310.5, 65, 8),
                   XStringFormats.Center);
            }
            if (BTeam is not null)
            {
                gfx.DrawString(BTeam,
                    AutoFontSize(BTeam, 8, 84, gfx),
                   textColor,
                   new XRect(405, 67, 84, 12),
                   XStringFormats.Center);

                gfx.DrawString(BTeam,
                    AutoFontSize(BTeam, 5, 65, gfx),
                   textColor,
                   new XRect(745, 310.5, 65, 8),
                   XStringFormats.Center);
            }

            if (isMen is not null)
            {
                if ((bool)isMen)
                {
                    //男子
                    gfx.DrawLine(new XPen(textColor), 69, 87, 87, 74);
                    gfx.DrawLine(new XPen(textColor), 69, 74, 87, 87);
                }
                else
                {
                    //女子
                    gfx.DrawLine(new XPen(textColor), 104, 87, 120, 74);
                    gfx.DrawLine(new XPen(textColor), 104, 74, 120, 87);
                }
            }

            if (MatchTime is not null)
            {
                gfx.DrawString(MatchTime.Value.Hour.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(208, 75, 30, 12),
                   XStringFormats.Center);

                gfx.DrawString(MatchTime.Value.Minute.ToString("00"),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(243, 75, 30, 12),
                   XStringFormats.Center);
            }
            pdfdoc.Pages[0].Rotate = 90;

            pdfdoc.Save("aaa.pdf");

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
    }
}
