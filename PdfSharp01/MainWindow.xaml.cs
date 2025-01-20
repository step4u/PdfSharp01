using HelloMigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp01.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PdfSharp01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btn0.Click += Btn0_Click;
            btn1.Click += Btn1_Click;
            btn2.Click += Btn2_Click;
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            var doc = new Document();
            HelloMigraDoc.Styles.DefineStyles(doc);

            var section = doc.AddSection();
            var pageSetup = doc.LastSection.PageSetup;

            pageSetup.PageFormat = PageFormat.A4;
            pageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
            pageSetup.TopMargin = Unit.FromCentimeter(2.0);
            pageSetup.BottomMargin = Unit.FromCentimeter(2.0);
            pageSetup.LeftMargin = Unit.FromCentimeter(2.0);
            pageSetup.RightMargin = Unit.FromCentimeter(2.0);
            
            var pageHeight = Unit.FromCentimeter(29.7).Point - pageSetup.TopMargin.Point - pageSetup.BottomMargin.Point;
            var pageWidth = Unit.FromCentimeter(21).Point - pageSetup.LeftMargin.Point - pageSetup.RightMargin.Point;

            var data = File.ReadAllText("data01.json");
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataModel>>(data);


            foreach (var item in jsonData)
            {
                if (item.seq == 0)
                {
                    doc.LastSection.AddParagraph(item.subject, "Title");
                }
                else
                {
                    doc.LastSection.AddParagraph();
                    doc.LastSection.AddParagraph($"{item.seq}. {item.subject}", StyleNames.Heading4);
                }


                var content = item.content;

                if (content != null)
                {
                    foreach (var citem in content)
                    {
                        var obj = citem.Count == 0 ? null : citem.First as JProperty;
                        if (obj != null)
                        {
                            if (obj.Name.ToLower() == "text")
                            {
                                var paragraph = doc.LastSection.AddParagraph();
                                //paragraph.Format.LeftIndent = Unit.FromPoint(10);
                                paragraph.AddText(obj.Value.ToString());
                            }
                            else if (obj.Name.ToLower() == "table")
                            {
                                var table = doc.LastSection.AddTable();
                                table.Borders.Width = 0.7;

                                var rows = obj.Value as JArray;
                                var cols = rows[0] as JArray;

                                for (int i = 0; i < cols.Count; i++)
                                {
                                    var col = table.AddColumn();

                                    if (cols.Count == 2)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            col.Width = Unit.FromPoint(60);
                                        }
                                        else
                                        {
                                            col.Width = Unit.FromPoint(300);
                                        }
                                    }
                                    else if (cols.Count == 4)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            col.Width = Unit.FromPoint(100);
                                        }
                                        else
                                        {
                                            col.Width = Unit.FromPoint(130);
                                        }
                                    }
                                }

                                for (int i = 0; i < rows.Count; i++)
                                {
                                    var row = table.AddRow();
                                    for (int j = 0; j < cols.Count; j++)
                                    {
                                        var cell = row.Cells[j];
                                        cell.AddParagraph(rows[i][j].ToString());
                                    }
                                }

                                table.SetEdge(0, 0, cols.Count, rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                            }
                            else if (obj.Name.ToLower() == "image")
                            {
                                var imageSource = new BitmapImage(new Uri("d:\\안드로이드_Meitu.jpg", UriKind.RelativeOrAbsolute));
                                var baseAxis = imageSource.Width >= imageSource.PixelHeight ? BaseAxis.X : BaseAxis.Y;

                                var paragraph = doc.LastSection.AddParagraph();
                                var image = paragraph.AddImage("d:\\안드로이드_Meitu.jpg");
                                image.LockAspectRatio = true;
                                //paragraph.Format.LeftIndent = Unit.FromPoint(10);

                                if (baseAxis == BaseAxis.X)
                                {
                                    image.Width = pageWidth <= imageSource.PixelWidth ? Unit.FromPoint(pageWidth) : Unit.FromPoint(imageSource.PixelWidth);
                                }
                                else
                                {
                                    image.Height = pageHeight <= imageSource.PixelHeight ? Unit.FromPoint(pageHeight) : Unit.FromPoint(imageSource.PixelHeight);
                                    image.Height -= Unit.FromPoint(200);
                                }
                            }
                        }
                    }
                }
            }

            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(doc, "MigraDoc.mdddl");

            PdfDocumentRenderer renderer = new PdfDocumentRenderer();
            renderer.Document = doc;
            renderer.RenderDocument();

            XPen pen = new XPen(XColors.Black, 0.7);
            double rectOutMargin = 22;
            double gap = 1.6;
            double rectInMargin = rectOutMargin + gap;

            foreach (var page in renderer.PdfDocument.Pages)
            {
                var gfx = XGraphics.FromPdfPage(page);

                //// 첫 번째 테두리 설정 (외곽)
                //XPen outerPen = new XPen(XColors.Black, 1);
                //gfx.DrawRectangle(outerPen, 29, 29, pageSetup.PageWidth.Point - 57, pageSetup.PageHeight.Point - 58);

                //// 두 번째 테두리 설정 (내부)
                //XPen innerPen = new XPen(XColors.Black, 1);
                //gfx.DrawRectangle(innerPen, 30, 30, pageSetup.PageWidth.Point - 60, pageSetup.PageHeight.Point - 60);

                var rectOut = new XRect(rectOutMargin, rectOutMargin, pageSetup.PageWidth.Point - rectOutMargin * 2, pageSetup.PageHeight.Point - rectOutMargin * 2);
                var rectIn = new XRect(rectInMargin, rectInMargin, pageSetup.PageWidth.Point - rectInMargin * 2, pageSetup.PageHeight.Point - rectInMargin * 2);

                gfx.DrawRectangle(pen, rectOut);
                gfx.DrawRectangle(pen, rectIn);
            }

            // Save the document...
            string filename = "HelloMigraDoc2.pdf";
            renderer.PdfDocument.Save(filename);
            PdfFileUtility.ShowDocument(filename);

            //RtfDocumentRenderer rtfrenderer = new RtfDocumentRenderer();
            //rtfrenderer.Render(doc, "TEST01.rtf", null);
            //PdfFileUtility.ShowDocument("TEST01.rtf");
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            // Create a MigraDoc document
            Document document = Documents.CreateDocument();

            //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            PdfDocumentRenderer renderer = new PdfDocumentRenderer();
            renderer.Document = document;

            renderer.RenderDocument();

            // Save the document...
            string filename = "HelloMigraDoc.pdf";
            renderer.PdfDocument.Save(filename);

            PdfFileUtility.ShowDocument(filename);
        }


        private void Btn0_Click(object sender, RoutedEventArgs e)
        {
            // Create a new PDF document.
            var document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";
            document.Info.Subject = "Just a simple Hello-World program.";

            // Create an empty page in this document.
            var page = document.AddPage();
            page.Size = PageSize.A4;

            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            // Draw two lines with a red default pen.
            var width = page.Width.Point;
            var height = page.Height.Point;
            gfx.DrawLine(XPens.Red, 0, 0, width, height);
            gfx.DrawLine(XPens.Red, width, 0, 0, height);

            int rectOutMargin = 22;
            int gap = 2;
            int rectInMargin = rectOutMargin + gap;


            var rectOut = new XRect(rectOutMargin, rectOutMargin, width - rectOutMargin * 2, height - rectOutMargin * 2);
            var rectIn = new XRect(rectInMargin, rectInMargin, width - rectInMargin * 2, height - rectInMargin * 2);

            gfx.DrawRectangle(XPens.Black, rectOut);
            gfx.DrawRectangle(XPens.Black, rectIn);

            // Draw a circle with a red pen which is 1.5 point thick.
            var r = width / 5;
            gfx.DrawEllipse(new XPen(XColors.Red, 1.5), XBrushes.White, new XRect(width / 2 - r, height / 2 - r, 2 * r, 2 * r));

            // Create a font.
            var font = new XFont("Times New Roman", 20, XFontStyleEx.BoldItalic);

            // Draw the text.
            gfx.DrawString("Hello, PDFsharp!", font, XBrushes.Black,
                new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.Center);

            // Save the document...
            var filename = PdfFileUtility.GetTempPdfFullFileName("samples/HelloWorldSample");
            document.Save(filename);
            // ...and start a viewer.
            PdfFileUtility.ShowDocument(filename);

        }
    }

    public enum BaseAxis
    {
        X,
        Y
    }
}
