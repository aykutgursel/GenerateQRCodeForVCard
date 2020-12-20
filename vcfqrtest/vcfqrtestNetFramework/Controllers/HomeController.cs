using System.Web.Mvc;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.Drawing.Imaging;
using vcfqrtestNetFramework.Models;
using System.Web;
using System.Threading;
using System.Globalization;
using System;
using vcfqrtestNetFramework.Extensions;

namespace vcfqrtestNetFramework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View(new VCardPageModelRequest());
        }

        [HttpPost]
        public ActionResult Index(VCardPageModelRequest request)
        {
            VCard vCard = new VCard
            {
                FirstName = request.VCard.FirstName,
                LastName = request.VCard.LastName,
                Phone = request.VCard.Phone,
                JobTitle = request.VCard.JobTitle,
                Email = request.VCard.Email,
                City = request.VCard.City,
                CountryName = request.VCard.CountryName,
                HomePage = request.VCard.HomePage,
                Mobile = request.VCard.Mobile,
                Zip = request.VCard.Zip,
                Organization = request.VCard.Organization,
                StreetAddress = request.VCard.StreetAddress
            };

            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(vCard.ToString(), out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            //////Logo

            DrawingSize dSize = renderer.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            renderer.Draw(g, qrCode.Matrix);

            Image img = Image.FromFile(Server.MapPath(@"/img/logo6.png"));

            Point imgPoint = new Point((map.Width - img.Width) / 2, (map.Height - img.Height) / 2);
            g.DrawImage(img, imgPoint.X, imgPoint.Y, img.Width, img.Height);

            //string fileName = Guid.NewGuid().ToString().Replace("-", "") + "." + ImageFormat.Jpeg;
            //string savePath = Server.MapPath(@"/Img/") + fileName;
            //map.Save(savePath);

            //////endLogo
            ///
            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
            memoryStream.Position = 0;

            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = string.Format("{0}.png", vCard.ToString());

            if (request.Logo)
                ViewBag.QrCode = map.ToByteArray(ImageFormat.Png);
            else
                ViewBag.QrCode = ReadFully(resultStream.FileStream);

            return View(request);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public ActionResult Change(string languageSelect)
        {
            if (languageSelect != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageSelect);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageSelect);
            }

            HttpCookie cookie = new HttpCookie("language");
            cookie.Value = languageSelect;
            Response.Cookies.Add(cookie);

            return View("Index");
        }
    }
}