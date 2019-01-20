using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZXing.QrCode;

namespace QRGen.Controllers
{
    
    [ApiController]
    public class QRController : ControllerBase
    {
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]QRData data)
        {
            var ms = GenCode(data.data, data.height, data.width, data.margin);
            return File(ms, "image/png");
        }

        [Route("api/[controller]/base64")]
        [HttpPost]
        public string PostBase64Response([FromBody]QRData data)
        {
            var ms = GenCode(data.data, data.height, data.width, data.margin);
            var mBytes = ms.ToArray();          
            return Convert.ToBase64String(mBytes);
        }

        private MemoryStream GenCode(string data, int height, int width, int margin)
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
            return ms;
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