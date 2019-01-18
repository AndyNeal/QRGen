using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZXing.QrCode;

namespace QRGen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string data = "SAMPLE")
        {
            return GenCode(data, 250, 250, 0);
        }

        [HttpPost]
        [HttpPut]
        public async Task<IActionResult> Post([FromBody]QRData data)
        {
            return GenCode(data.data, data.height, data.width, data.margin);
        }

        private IActionResult GenCode(string data, int height, int width, int margin)
        {
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(data);

            var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            var ms = new MemoryStream();

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            return File(ms, "image/png");
        }
    }
    public class QRData
    {
        public string data { get; set; } = "SAMPLE";
        public int height { get; set; } = 250;
        public int width { get; set; } = 250;
        public int margin { get; set; } = 0;
    }
}