using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZXing;
using ZXing.QrCode;


namespace QRGen.Controllers
{
    
    [ApiController]
    public class QRController : ControllerBase
    {
        [Route("[controller]/png")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]QRData data)
        {
            var ms = GenCode(data.data, data.height, data.width, data.margin);
            return File(ms, "image/png");
        }

        [Route("[controller]/base64")]
        [HttpPost]
        public string PostBase64Response([FromBody]QRData data)
        {
            var ms = GenCode(data.data, data.height, data.width, data.margin);
            var mBytes = ms.ToArray();          
            return "data: image / png; base64," + Convert.ToBase64String(mBytes);
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
            var bitMatrix = qrCodeWriter.Encode(data);

            var renderer = new ZXing.SkiaSharp.Rendering.SKBitmapRenderer();
            var bitmap = renderer.Render(bitMatrix, BarcodeFormat.QR_CODE, data);
            
            var skStream = new SkiaSharp.SKDynamicMemoryWStream();
            bitmap.Encode(skStream, SkiaSharp.SKEncodedImageFormat.Png, 100);
            var stream = skStream.CopyToData().AsStream();

            var ms = new MemoryStream();
            stream.CopyTo(ms);
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