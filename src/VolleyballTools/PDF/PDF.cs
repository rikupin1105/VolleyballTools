using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.JSInterop;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using VolleyballTools.Model;
using VolleyballTools.PDF;

namespace VolleyballTools.PDF
{
    public static class Extention
    {
        public static void RenderText(this XGraphics gfx, string? text, XRect rect, double fontSize = 11, XStringFormat? format = null, XBrush? textColor = null)
        {
            if (text is null)
            {
                return;
            }
            textColor ??= XBrushes.Black;
            format ??= XStringFormats.Center;

            gfx.DrawString(text, AutoFontSize(text, fontSize, rect.Width, gfx), textColor, rect, format);
        }
        private static XFont AutoFontSize(string text, double maxSize, double width, XGraphics gfx)
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
    }
    public class PDF
    {
        public PDF()
        {
            GlobalFontSettings.FontResolver = new JapaneseFontResolver();
        }

        enum ScoresheetTemplate
        {
            three,
            five,
            nineParson,
            liberoThree,
            liberoFive,
            beachone,
            beachthree
        }
        private static Stream Template(ScoresheetTemplate template)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (template == ScoresheetTemplate.three)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.3SET.pdf")??throw new ArgumentException("3SET.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
            else if (template == ScoresheetTemplate.nineParson)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.9Parson.pdf")??throw new ArgumentException("9Parson.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
            else if (template==ScoresheetTemplate.liberoThree)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.Libero3.pdf")??throw new ArgumentException("Libero3.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
            else if (template==ScoresheetTemplate.liberoFive)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.Libero5.pdf")??throw new ArgumentException("Libero5.pdf");
                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);

                return new MemoryStream(data);
            }
            else if (template==ScoresheetTemplate.beachone)
            {
                using Stream? stream = assembly.GetManifestResourceStream("VolleyballTools.PDF.Beach1.pdf")??throw new ArgumentException("Libero5.pdf");
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
        private static (XGraphics, PdfDocument) LoadTemplate(ScoresheetTemplate template, PageSize pagesize = PageSize.A3)
        {
            var pdfdoc = new PdfDocument();

            PdfDocument inputDocument = PdfReader.Open(Template(template), PdfDocumentOpenMode.Import);
            var templatePage = inputDocument.Pages[0];

            var page = pdfdoc.AddPage(templatePage);
            page.Orientation = PageOrientation.Landscape;
            page.Size = pagesize;

            var gfx = XGraphics.FromPdfPage(page);
            gfx.RotateAtTransform(-90, new XPoint(0, 0));

            if (pagesize == PageSize.A4)
            {
                gfx.TranslateTransform(-595, 0);
            }
            else if (pagesize == PageSize.A3)
            {
                gfx.TranslateTransform(-842, 0);
            }

            return (gfx, pdfdoc);
        }

        public static Stream Generate3SET(Match match)
        {
            var t = LoadTemplate(ScoresheetTemplate.three);
            var gfx = t.Item1;
            var pdfdoc = t.Item2;

            gfx.RenderText(match.MatchName, new XRect(60, 35, 500, 25), 15);
            gfx.RenderText(match.Venue, new XRect(60, 65, 190, 15));
            gfx.RenderText(match.Hall, new XRect(60, 85, 180, 15));
            gfx.RenderText(match.MatchNumber, new XRect(280, 65, 60, 15));

            if (match.DateTime is not null)
            {
                gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(360, 65, 30, 15), 8, XStringFormats.BottomCenter);
                gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(395, 65, 30, 15), 8, XStringFormats.BottomCenter);
                gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(430, 65, 30, 15), 8, XStringFormats.BottomCenter);
            }

            gfx.RenderText(match.ATeamName, new XRect(405, 85, 90, 15));
            gfx.RenderText(match.ATeamName, new XRect(650, 180, 55, 15));

            gfx.RenderText(match.BTeamName, new XRect(515, 85, 90, 15));
            gfx.RenderText(match.BTeamName, new XRect(735, 180, 55, 15));

            switch (match.Sex)
            {
                case Sex.Men:
                    //男子
                    gfx.DrawLine(new XPen(XBrushes.Black), 290, 98, 279, 88);
                    gfx.DrawLine(new XPen(XBrushes.Black), 290, 88, 279, 98);
                    break;
                case Sex.Women:
                    //女子
                    gfx.DrawLine(new XPen(XBrushes.Black), 313, 98, 302, 88);
                    gfx.DrawLine(new XPen(XBrushes.Black), 313, 88, 302, 98);
                    break;
                case null:
                    break;
            }

            if (match.MatchTime is not null)
            {
                gfx.RenderText(match.MatchTime.Value.Hour.ToString(), new XRect(525, 65, 20, 15), 8);
                gfx.RenderText(match.MatchTime.Value.Minute.ToString("00"), new XRect(565, 65, 20, 15), 8);
            }
            pdfdoc.Pages[0].Rotate = 90;

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        public static Stream Generate5SET(Match match)
        {
            var t = LoadTemplate(ScoresheetTemplate.five);
            var gfx = t.Item1;
            var pdfdoc = t.Item2;

            gfx.RenderText(match.MatchName, new XRect(85, 34, 420, 12), 8);
            gfx.RenderText(match.Venue, new XRect(55, 48, 110, 12), 8, format: XStringFormats.CenterLeft);
            gfx.RenderText(match.Hall, new XRect(55, 62, 110, 12), 8, format: XStringFormats.CenterLeft);

            gfx.RenderText(match.MatchNumber, new XRect(210, 48, 25, 12), 8);

            if (match.DateTime is not null)
            {
                gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(208, 62, 16, 12), 6);
                gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(232, 62, 14, 12), 6);
                gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(252, 62, 14, 12), 6);
            }

            gfx.RenderText(match.ATeamName, new XRect(305, 67, 84, 12), 8);
            gfx.RenderText(match.ATeamName, new XRect(660, 310.5, 65, 8), 5);

            gfx.RenderText(match.BTeamName, new XRect(405, 67, 84, 12), 8);
            gfx.RenderText(match.BTeamName, new XRect(745, 310.5, 65, 8), 5);


            switch (match.Sex)
            {
                case Sex.Men:
                    gfx.DrawLine(new XPen(XBrushes.Black), 69, 87, 87, 74);
                    gfx.DrawLine(new XPen(XBrushes.Black), 69, 74, 87, 87);
                    break;
                case Sex.Women:
                    gfx.DrawLine(new XPen(XBrushes.Black), 104, 87, 120, 74);
                    gfx.DrawLine(new XPen(XBrushes.Black), 104, 74, 120, 87);
                    break;
                case null:
                    break;
            }

            if (match.MatchTime is not null)
            {
                gfx.RenderText(match.MatchTime.Value.Hour.ToString(), new XRect(208, 75, 30, 12), 8);
                gfx.RenderText(match.MatchTime.Value.Minute.ToString("00"), new XRect(243, 75, 30, 12), 8);
            }
            pdfdoc.Pages[0].Rotate = 90;

            pdfdoc.Save("aaa.pdf");

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        public static Stream Generate9Parson(NineParsonMatch match)
        {
            var t = LoadTemplate(ScoresheetTemplate.nineParson);
            var gfx = t.Item1;
            var pdfdoc = t.Item2;

            gfx.RenderText(match.MatchName, new XRect(60, 56, 590, 15), format: XStringFormats.CenterLeft);
            gfx.RenderText(match.Venue, new XRect(60, 75, 170, 15), format: XStringFormats.CenterLeft);
            gfx.RenderText(match.Hall, new XRect(60, 94, 170, 15), format: XStringFormats.CenterLeft);
            gfx.RenderText(match.MatchNumber, new XRect(470, 75, 50, 15));


            if (match.DateTime is not null)
            {
                gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(290, 75, 40, 15));
                gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(350, 75, 20, 15));
                gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(390, 75, 20, 15));
            }

            gfx.RenderText(match.ATeamName, new XRect(310, 94, 105, 15));
            gfx.RenderText(match.ATeamName, new XRect(675, 122, 64, 15), 8);

            gfx.RenderText(match.BTeamName, new XRect(435, 94, 105, 15));
            gfx.RenderText(match.BTeamName, new XRect(742, 122, 64, 15), 8);


            switch (match.Sex)
            {
                case Sex.Men:
                    gfx.DrawLine(new XPen(XBrushes.Black), 290+305, 107.5, 581, 96.5);
                    gfx.DrawLine(new XPen(XBrushes.Black), 290+305, 96.5, 581, 107.5);
                    break;
                case Sex.Women:
                    gfx.DrawLine(new XPen(XBrushes.Black), 633, 107.5, 621, 96.5);
                    gfx.DrawLine(new XPen(XBrushes.Black), 633, 96.5, 621, 107.5);
                    break;
                case null:
                    break;
            }

            if (match.MatchTime is not null)
            {
                gfx.RenderText(match.MatchTime.Value.Hour.ToString(), new XRect(590, 75, 30, 15));
                gfx.RenderText(match.MatchTime.Value.Minute.ToString("00"), new XRect(630, 75, 20, 15));
            }


            if (match.ATeamPlayers is not null)
            {
                for (int i = 0; i < match.ATeamPlayers.Count; i++)
                {
                    gfx.RenderText(match.ATeamPlayers[i].Number.ToString(), new XRect(660, 154 + (i * 14), 15, 10), 8);
                    gfx.RenderText(match.ATeamPlayers[i].Name, new XRect(677, 154 + (i * 14), 60, 10), 8);
                }
            }

            if (match.BTeamPlayers is not null)
            {
                for (int i = 0; i < match.BTeamPlayers.Count; i++)
                {
                    gfx.RenderText(match.BTeamPlayers[i].Number.ToString(), new XRect(742, 154 + (i * 14), 15, 10), 8);
                    gfx.RenderText(match.BTeamPlayers[i].Name, new XRect(759, 154 + (i * 14), 60, 10), 8);
                }
            }

            pdfdoc.Pages[0].Rotate = 90;

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        public static Stream GanerateLibero3SET(Match match)
        {
            var t = LoadTemplate(ScoresheetTemplate.liberoThree, PageSize.A4);
            var gfx = t.Item1;
            var pdfdoc = t.Item2;

            gfx.RenderText(match.MatchName, new XRect(90, 31.6, 483.1, 15.6), format: XStringFormats.CenterLeft);
            gfx.RenderText(match.Venue, new XRect(90, 49.9, 221.7, 12.9), 9, format: XStringFormats.CenterLeft);
            gfx.RenderText(match.Hall, new XRect(90, 65.5, 221.7, 16.3), 9, format: XStringFormats.CenterLeft);
            gfx.RenderText(match.MatchNumber, new XRect(223.9, 84.2, 91.4, 13.4));

            if (match.DateTime is not null)
            {
                gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(361.2, 50.1, 30.4, 18.7), 8);
                gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(401.5, 50.1, 16.0, 18.7), 8);
                gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(426, 50.1, 16.0, 18.7), 8);
            }

            gfx.RenderText(match.ATeamName, new XRect(349.4, 72.2, 81.3, 24.9), 8);
            gfx.RenderText(match.BTeamName, new XRect(463.6, 72.2, 81.3, 24.9), 8);

            switch (match.Sex)
            {
                case Sex.Men:
                    gfx.DrawLine(new XPen(XBrushes.Black), 111.3, 86.4, 125.2, 95.2);
                    gfx.DrawLine(new XPen(XBrushes.Black), 111.3, 95.2, 125.2, 86.4);
                    break;
                case Sex.Women:
                    gfx.DrawLine(new XPen(XBrushes.Black), 154.5, 86.4, 168.4, 95.2);
                    gfx.DrawLine(new XPen(XBrushes.Black), 154.5, 95.2, 168.4, 86.4);
                    break;
                case null:
                    break;
            }

            if (match.MatchTime is not null)
            {
                gfx.RenderText(match.MatchTime.Value.Hour.ToString(), new XRect(516.4, 50.1, 26.4, 18.7), 8);
                gfx.RenderText(match.MatchTime.Value.Minute.ToString("00"), new XRect(546.7, 50.1, 29.5, 18.7), 8);
            }
            pdfdoc.Pages[0].Rotate = 90;

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        public static Stream GanerateLibero5SET(Match match)
        {
            var t = LoadTemplate(ScoresheetTemplate.liberoFive, PageSize.A4);
            var gfx = t.Item1;
            var pdfdoc = t.Item2;

            gfx.RenderText(match.MatchName, new XRect(97.4, 39.3, 478.8, 15.6));
            gfx.RenderText(match.Venue, new XRect(97.4, 57.6, 218.6, 12.9), 9);
            gfx.RenderText(match.Hall, new XRect(97.4, 73.2, 218.6, 16.3), 9);
            gfx.RenderText(match.MatchNumber, new XRect(227.5, 91.9, 91.4, 13.4));

            if (match.DateTime is not null)
            {
                gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(364.5, 57.8, 30.4, 18.7), 8);
                gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(404.8, 57.8, 16.0, 18.7), 8);
                gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(429.3, 57.8, 16.0, 18.7), 8);
            }

            gfx.RenderText(match.ATeamName, new XRect(353, 79.9, 81.3, 24.9), 8);
            gfx.RenderText(match.BTeamName, new XRect(467.2, 79.9, 81.3, 24.9), 8);

            switch (match.Sex)
            {
                case Sex.Men:
                    gfx.DrawLine(new XPen(XBrushes.Black), 115.2, 94.0, 129.1, 102.8);
                    gfx.DrawLine(new XPen(XBrushes.Black), 115.2, 102.8, 129.1, 94.0);
                    break;
                case Sex.Women:
                    gfx.DrawLine(new XPen(XBrushes.Black), 158.4, 94.0, 172.3, 102.8);
                    gfx.DrawLine(new XPen(XBrushes.Black), 158.4, 102.8, 172.3, 94.0);
                    break;
                case null:
                    break;
            }

            if (match.MatchTime is not null)
            {
                gfx.RenderText(match.MatchTime.Value.Hour.ToString(), new XRect(519.1, 57.6, 26.4, 18.7), 9);
                gfx.RenderText(match.MatchTime.Value.Minute.ToString("00"), new XRect(549.4, 57.6, 29.5, 18.7), 8);
            }
            pdfdoc.Pages[0].Rotate = 90;

            var stream = new MemoryStream();
            pdfdoc.Save(stream, false);

            return stream;
        }
        //public Stream GenerateBeach1SET(Pages.BeachVolleyball.BeachMatch match)
        //{
        //    var pdfdoc = new PdfDocument();

        //    PdfDocument inputDocument = PdfReader.Open(Template(ScoresheetTemplate.beachone), PdfDocumentOpenMode.Import);
        //    var templatePage = inputDocument.Pages[0];
        //    templatePage.Size = PdfSharpCore.PageSize.A3;
        //    templatePage.Orientation = PdfSharpCore.PageOrientation.Landscape;

        //    var page = pdfdoc.AddPage(templatePage);
        //    page.Orientation = PdfSharpCore.PageOrientation.Landscape;
        //    page.Size = PdfSharpCore.PageSize.A3;
        //    page.Rotate = 0;

        //    var gfx = XGraphics.FromPdfPage(page);
        //    gfx.RotateAtTransform(-90, new XPoint(0, 0));
        //    gfx.TranslateTransform(-842, 0);

        //    gfx.RenderText(match.MatchName, new XRect(88.3, 50.4, 559.2, 16.5), 15, XStringFormats.CenterLeft);
        //    gfx.RenderText(match.Venue, new XRect(129.8, 73.2, 98.4, 14.88));
        //    gfx.RenderText(match.Hall, new XRect(264, 73.2, 112.3, 14.88));
        //    gfx.RenderText(match.MatchNumber, new XRect(54.9, 78.9, 38.6, 8.8));
        //    if (match.DateTime is not null)
        //    {
        //        gfx.RenderText(match.DateTime.Value.Year.ToString(), new XRect(435.1, 78.9, 31.6, 10.0));
        //        gfx.RenderText(match.DateTime.Value.Month.ToString(), new XRect(477.3, 78.9, 11.0, 10.0));
        //        gfx.RenderText(match.DateTime.Value.Day.ToString(), new XRect(496, 78.9, 12.4, 10.0));
        //    }

        //    if (match.ATeam is not null)
        //    {

        //        gfx.RenderText(match.ATeam.Prefecture, new XRect(319.2, 101, 78.4, 12.4));
        //        gfx.RenderText(match.ATeam.Name, new XRect(84.7, 405.3, 62.8, 20.1));

        //        if (match.ATeam.Player1 is not null)
        //        {
        //            gfx.RenderText(match.ATeam.Player1.Number.ToString(), new XRect(49.4, 441.8, 23.6, 18.4));
        //            gfx.RenderText(match.ATeam.Player1.Name, new XRect(75.8, 441.8, 80.6, 18.4));
        //            gfx.RenderText(match.ATeam.Player1.Name, new XRect(77, 95.2, 106.8, 18.2));
        //            gfx.RenderText(match.ATeam.Player1.Name, new XRect(279.6, 493.9, 29.2, 18.4));
        //        }
        //        if (match.ATeam.Player2 is not null)
        //        {
        //            gfx.RenderText(match.ATeam.Player2.Number.ToString(), new XRect(49.4, 463.9, 23.6, 18.4));
        //            gfx.RenderText(match.ATeam.Player2.Name, new XRect(75.8, 463.9, 80.6, 18.4));
        //            gfx.RenderText(match.ATeam.Player2.Name, new XRect(207.1, 95.2, 106.8, 18.2));
        //            gfx.RenderText(match.ATeam.Player2.Name, new XRect(320.1, 493.9, 29.2, 18.4));
        //        }
        //    }
        //    if (match.BTeam is not null)
        //    {

        //    }

        //    switch (match.Sex)
        //    {
        //        case Sex.Men:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 531.8, 84.9, 541.1, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 531.8, 76.3, 541.1, 84.9);
        //            break;
        //        case Sex.Women:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 558.7, 84.9, 568, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 558.7, 76.3, 568, 84.9);
        //            break;
        //        case null:
        //            break;
        //    }

        //    switch (match.Round)
        //    {
        //        case Round.Final:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 595.6, 84.9, 605, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 595.6, 76.3, 605, 84.9);
        //            break;
        //        case Round.Qualification:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 629.2, 84.9, 638.6, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 629.2, 76.3, 638.6, 84.9);
        //            break;
        //        case null:
        //            break;
        //    }

        //    switch (match.Stage)
        //    {
        //        case Stage.Pool:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 672.9, 84.9, 682.5, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 672.9, 76.3, 682.5, 84.9);
        //            break;
        //        case Stage.Rank:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 706.3, 84.9, 715.9, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 706.3, 76.3, 715.9, 84.9);
        //            break;
        //        case Stage.SemiFinal:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 746.6, 84.9, 756.2, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 746.6, 76.3, 756.2, 84.9);
        //            break;
        //        case Stage.Final:
        //            gfx.DrawLine(new XPen(XBrushes.Black), 780.4, 84.9, 790, 76.3);
        //            gfx.DrawLine(new XPen(XBrushes.Black), 780.4, 76.3, 790, 84.9);
        //            break;
        //        case null:
        //            break;
        //    }


        //    pdfdoc.Pages[0].Rotate = 90;

        //    var stream = new MemoryStream();
        //    pdfdoc.Save(stream, false);

        //    return stream;
        //}
    }
}
