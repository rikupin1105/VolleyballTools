using PdfSharpCore.Fonts;
using System.Reflection;

namespace VolleyballTools.PDF
{
    public class JapaneseFontResolver : IFontResolver
    {
        private static readonly string NOTO_SANS_JP_REGULAR_TTF =
            "VolleyballTools.fonts.NotoSansJP-Regular.ttf";

        public string DefaultFontName => "NotoSansJP";

        public byte[]? GetFont(string faceName)
        {
            return faceName switch
            {
                "NotoSansJP#Regular" => LoadFontData(NOTO_SANS_JP_REGULAR_TTF),
                _ => null,
            };
        }

        public FontResolverInfo ResolveTypeface(
                    string familyName, bool isBold, bool isItalic)
        {
            var fontName = familyName.ToLower();

            return fontName switch
            {
                "notosans" => new FontResolverInfo("NotoSansJP#Regular"),
                // デフォルトのフォント
                _ => PlatformFontResolver.ResolveTypeface("NotoSansJP", isBold, isItalic),
            };
        }

        // 埋め込みリソースからフォントファイルを読み込む
        private static byte[] LoadFontData(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new ArgumentException("No resource with name " + resourceName);

            int count = (int)stream.Length;
            byte[] data = new byte[count];
            stream.Read(data, 0, count);
            return data;
        }

    }
}