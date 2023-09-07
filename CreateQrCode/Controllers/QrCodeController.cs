using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace CreateQrCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QrCodeController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;

        public QrCodeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        //[HttpGet]
        //public IActionResult GenerateQRCode(string link)
        //{
        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
        //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
        //    QRCode qrCode = new QRCode(qrCodeData);

        //    // Generate QR code as Bitmap
        //    Bitmap qrCodeImage = qrCode.GetGraphic(20);

        //    // Load logo image
        //    Bitmap logo = new Bitmap("logo/sfda.png"); // Replace with the actual path

        //    // Get dimensions for logo placement
        //    int deltaHeight = qrCodeImage.Height - logo.Height;
        //    int deltaWidth = qrCodeImage.Width - logo.Width;

        //    // Draw the logo on top of the QR code
        //    Graphics g = Graphics.FromImage(qrCodeImage);
        //    g.DrawImage(logo, new Point(deltaWidth / 2, deltaHeight / 2));

        //    // Convert the image to byte array
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        qrCodeImage.Save(ms, ImageFormat.Png);
        //        return File(ms.ToArray(), "image/png");
        //    }
        //}

        [HttpGet]
        public IActionResult GenerateQRCode(string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // Construct the path to the logo file in the wwwroot folder
            string logoPath = Path.Combine(_env.WebRootPath,  "sfda.png");

            if (System.IO.File.Exists(logoPath))
            {
                using (Bitmap logo = new Bitmap(logoPath))
                {
                    // Make sure the logo has a transparent background
                    logo.MakeTransparent();

                    // Scale and position calculations remain the same
                    int scale = 4;
                    int newWidth = qrCodeImage.Width / scale;
                    int newHeight = qrCodeImage.Height / scale;
                    Bitmap resizedLogo = new Bitmap(logo, new Size(newWidth, newHeight));

                    int x = (qrCodeImage.Width - resizedLogo.Width) / 2;
                    int y = (qrCodeImage.Height - resizedLogo.Height) / 2;

                    using (Graphics g = Graphics.FromImage(qrCodeImage))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                        // The following line ensures that the overlaying image is transparent.
                        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                        g.DrawImage(resizedLogo, new Point(x, y));
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
            else
            {
                return BadRequest("Logo file does not exist");
            }
        }


    }




}
