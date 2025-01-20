// MigraDoc - Creating Documents on the Fly
// See the LICENSE file in the solution root for more information.

using MigraDoc.DocumentObjectModel;

namespace HelloMigraDoc
{
    /// <summary>
    /// Defines the styles used in the document.
    /// </summary>
    public class Styles
    {
        /// <summary>
        /// Defines the styles used in the document.
        /// </summary>
        public static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            var style = document.Styles[StyleNames.Normal];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "���� ���";
            style.Font.Size = 12;
            style.Font.Bold = false;

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            document.AddStyle("Title", StyleNames.Normal);
            style = document.Styles["Title"];
            style.Font.Bold = false;
            style.Font.Size = 28;
            style.Font.Name = "HYHeadLine";
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.ParagraphFormat.SpaceBefore = Unit.FromCentimeter(2);
            style.ParagraphFormat.SpaceAfter = Unit.FromCentimeter(1);

            style = document.Styles[StyleNames.Heading1];
            style.Font.Name = "���� ���";
            style.Font.Size = 16;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;
            // Set KeepWithNext for all headings to prevent headings from appearing all alone
            // at the bottom of a page. The other headings inherit this from Heading1.
            style.ParagraphFormat.KeepWithNext = true;

            style = document.Styles[StyleNames.Heading2];
            style.Font.Size = 14;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles[StyleNames.Heading3];
            style.Font.Size = 12;
            style.Font.Italic = false;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal.
            style = document.Styles.AddStyle(TextBoxStyle, StyleNames.Normal);
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal.
            style = document.Styles.AddStyle(TocStyle, StyleNames.Normal);
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }

        public const string TextBoxStyle = "TextBox";

        public const string TocStyle = "TOC";
    }
}
