using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Common.Helper;

namespace MyBlog.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private QrCodeHelper _qrCoderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public QrCodeController(IHostingEnvironment hostingEnvironment)
        {
            _qrCoderService = new QrCodeHelper();
            _hostingEnvironment = hostingEnvironment;
        }
        #region 生成jpeg 格式 二维码
        [HttpGet, Route("GetJPEGQRPic")]
        public IActionResult GetJPEGQRPic()
        {
            //获取QRCoder Bitmap
            var bm = _qrCoderService.GetQRCode("http://www.baidu.com", 15);
            var ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Jpeg);
            return File(ms.GetBuffer(), "image/jpeg");
        }
        #endregion
        #region 生成带Icon二维码
        [HttpGet, Route("GetQRCodeWithLogo")]
        public IActionResult GetQRCodeWithLogo()
        {
            var logoPath = $"{_hostingEnvironment.WebRootPath}/icon.png";
            var bm = _qrCoderService.GetQRCodeWithLogo("hello world", 15, logoPath);
            var ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Jpeg);

            return File(ms.GetBuffer(), "image/jpeg");
        }
        #endregion
        #region 生成svg图片
        [HttpGet,Route("GetSvgQRPic")]
        public IActionResult GetSvgQRPic()
        {
            var svgText = _qrCoderService.GetSvgQRCode("http://www.baidu.com", 15);
            string rootPath = _hostingEnvironment.WebRootPath;

            string svgName = $"{Guid.NewGuid().ToString()}.svg";
            string filePath = $"{rootPath}/{svgName}";
            System.IO.File.WriteAllText(filePath, svgText);
            var byts = System.IO.File.ReadAllBytes(filePath);
            return File(byts, "image/svg", svgName);
        }
        #endregion
    }
}