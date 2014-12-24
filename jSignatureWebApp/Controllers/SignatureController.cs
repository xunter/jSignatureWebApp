using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace jSignatureWebApp.Controllers
{
    public class SignatureController : Controller
    {
        // GET: Signature
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string signPNGImageBase64, string printNamePNGImageBase64)
        {
            string pdfPath = HttpContext.Server.MapPath("~/App_Data/SAYSO.pdf");

            var signImageBytes = Convert.FromBase64String(signPNGImageBase64.Replace("data:image/png;base64,", String.Empty));
            WebImage signImage = new WebImage(signImageBytes);
            string fileNameSignImage = HttpContext.Server.MapPath("~/App_Data/sign.png");
            signImage.Save(filePath: fileNameSignImage);


            var printNameImageBytes = Convert.FromBase64String(printNamePNGImageBase64.Replace("data:image/png;base64,", String.Empty));
            WebImage printNameImage = new WebImage(printNameImageBytes);
            string fileNamePrintNameImage = HttpContext.Server.MapPath("~/App_Data/print_name.png");
            printNameImage.Save(filePath: fileNamePrintNameImage);

            string participant = "Pavel Nazarov";
            var date = DateTime.Now;

            int dayNum = date.Day;
            string monthName = date.ToString("MMMM");
            int year = date.Year;

            var pdfCreator = new SignDocPdfCreator { Participant = participant, Day = dayNum, MonthName = monthName, Year = year, SignFilename = fileNameSignImage, PrintNameFilename = fileNamePrintNameImage };
            pdfCreator.CreateDocument();
            var pdfBytes = pdfCreator.CreateAsBytes();

            return File(pdfBytes, "application/pdf");
        }
    }

    public class SignDocPdfCreator
    {
        public string Participant { get; set; }
        public int Day { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }

        public string SignFilename { get; set; }
        public string PrintNameFilename { get; set; }

        private Document document;

        public SignDocPdfCreator()
        {

        }
        
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Signed doc";
            this.document.Info.Subject = "";
            this.document.Info.Author = "Pavel Nazarov";

            DefineStyles();

            CreatePage();

            FillContent();

            return this.document;
        }
        
        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Arial";
            style.Font.Size = 12;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
        
        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();

            Paragraph p = null;

            var headerParagraph = section.AddParagraph("MEDIA CONSENT AND RELEASE FORM");
            headerParagraph.Format.Font.Bold = true;
            headerParagraph.Format.Font.Size = Unit.FromPoint(14);
            headerParagraph.Format.Alignment = ParagraphAlignment.Center;

            section.AddParagraph();

            p = section.AddParagraph("THIS DOCUMENT CONTAINS A RELEASE OF RIGHTS.  DO NOT SIGN THIS DOCUMENT UNTIL YOU HAVE READ IT CAREFULLY AND UNDERSTAND IT.");
            p.Format.Font.Bold = true;

            p = section.AddParagraph(String.Format("This Consent and Release Form (the “Release”) is executed this {0} day of {1}, {2}, by {3} (“Participant”).", Day, MonthName, Year, Participant));

            p = section.AddParagraph("For and in consideration of Say-So Inc and/or its affiliates (collectively, “SAY-SO”) allowing Participant to be videotaped and/or photographed and/or recorded (each a “Video”) by SAY-SO or SAY-SO’s agent(s), Participant agrees to be bound by the following:");

            p = section.AddParagraph("1.	Participant is Receiving Value.  Participant agrees that participating in the Video is of value to Participant, and that Participant is participating in the Video of their own free will and accord.");
            p = section.AddParagraph("2.	Release of Liability.  Participant for themselves, their legal representatives, heirs, and assigns, agrees to release, waive, and discharge SAY-SO, its corporate affiliates, and its and their officers, board members, shareholders, employees, and agents (collectively, the “SAY-SO Releases”), from all liability and/or claims of loss or damage, whether for personal injury to Participant, damage to Participant’s property, or otherwise, arising out of or in connection with Participant participating in the Video.  Participant agrees not to bring any claims or lawsuits against SAY-SO for any injury to Participant or damage to Participant’s property which arises from Participant participating in the Video.");
            p = section.AddParagraph("3.	Media Rights Release.  Participant understands and agrees that SAY-SO shall use the Video for general media purposes, including but not limited to, television, radio, newspaper, and/or internet advertising.  Participant agrees that any video, voice recording, still pictures, or other media content created by SAY-SO or SAY-SO ‘S agents or employees during the Video (the “Video Content”) shall be and remain the property of SAY-SO upon creation.  Participant agrees that SAY-SO shall own all right, title and interest to the Video Content (including all rights embodied therein) and that SAY-SO may edit, modify, distribute, and otherwise exploit the Video Content, without limitation, and without compensation, further permission, or notification to Participant.  Participant hereby waives any inspection or approval of use rights.  Participant also agrees to waive and release SAY-SO from any claims based upon invasion of privacy, right of publicity, defamation, or claim of visual or audio alteration or faulty mechanical reproduction.");

            p = section.AddParagraph("Participant agrees that this Release is intended to be as broad and inclusive as permitted by the laws of the State of California, and that if any portion of this Release is held invalid, it is agreed that the balance shall; notwithstanding, continue in full legal force and effect.");

            p = section.AddParagraph("Participant has executed this Release as of the day and year first above written.");

            p = section.AddParagraph("By signing this Electronic Signature Acknowledgment Form, I agree that my electronic signature is the legally binding equivalent to my handwritten signature.  Whenever I execute an electronic signature, it has the same validity and meaning as my handwritten signature.  I will not, at any time in the future, repudiate the meaning of my electronic signature or claim that my electronic signature is not legally binding.");

            p = section.AddParagraph("Agreed");
            p.Format.Font.Bold = true;

            p = section.AddParagraph();
            var image = p.AddImage(SignFilename);
            //image.Height = "2.5cm";
            image.Width = "5cm";
            image.Height = "1cm";
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;


            p = section.AddParagraph("Signature of Participant");


            p = section.AddParagraph();
            image = p.AddImage(PrintNameFilename);
            //image.Height = "2.5cm";
            image.Width = "5cm";
            image.Height = "1cm";
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;


            p = section.AddParagraph("Print Name");
        }

        void FillContent()
        {

        }

        public byte[] CreateAsBytes()
        {
            MigraDoc.Rendering.DocumentRenderer dr = new MigraDoc.Rendering.DocumentRenderer(this.document);
            dr.PrepareDocument();
            MigraDoc.Rendering.PdfDocumentRenderer pdfDr = new MigraDoc.Rendering.PdfDocumentRenderer();
            pdfDr.Document = this.document;
            pdfDr.DocumentRenderer = dr;
            pdfDr.PrepareRenderPages();
            pdfDr.RenderDocument();
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                pdfDr.Save(ms, false);
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
}