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
        private Stream Template()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.3SET.pdf"))
            {
                if (stream == null)
                    throw new ArgumentException();

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
        }
        public Stream Generate(string? MatchName = null, string? Venue = null, string? Hall = null, DateTime? Date = null, string? MatchNumber = null, string? ATeam = null, string? BTeam = null, bool? isMen = null, DateTime? MatchTime = null)
        {
            GlobalFontSettings.FontResolver = new JapaneseFontResolver();
            var pdfdoc = new PdfDocument();

            PdfDocument inputDocument = PdfReader.Open(Template(), PdfDocumentOpenMode.Import);
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
                    new XFont("notosans", 15, XFontStyle.Regular),
                textColor,
                    new XRect(60, 35, 500, 25),
                    format);
            }

            if (Venue is not null)
            {
                gfx.DrawString(Venue,
                    new XFont("notosans", 10, XFontStyle.Regular),
                textColor,
                    new XRect(60, 65, 190, 15),
                    format);
            }

            if (Hall is not null)
            {
                gfx.DrawString(Hall,
                    new XFont("notosans", 10, XFontStyle.Regular),
                textColor,
                    new XRect(60, 85, 180, 15),
                    format);

            }

            if (MatchNumber is not null)
            {
                gfx.DrawString(MatchNumber,
                    new XFont("notosans", 10, XFontStyle.Regular),
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
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(405, 85, 90, 15),
                   XStringFormats.Center);
            }
            if (BTeam is not null)
            {
                gfx.DrawString(BTeam,
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(515, 85, 90, 15),
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

                gfx.DrawString(MatchTime.Value.Minute.ToString(),
                   new XFont("notosans", 8, XFontStyle.Regular),
                   textColor,
                   new XRect(565, 65, 20, 15),
                   XStringFormats.Center);
            }
            //gfx.DrawRectangle(textColor, new XRect(525, 65, 20, 15));
            pdfdoc.Pages[0].Rotate = 90;

            MemoryStream stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
    }
}
