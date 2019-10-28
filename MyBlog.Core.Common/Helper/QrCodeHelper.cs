using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MyBlog.Core.Common.Helper
{
    public class QrCodeHelper
    {
        #region  QRCode

        /// <summary>
        /// 生成jpeg 格式 二维码
        /// </summary>
        /// <param name="plainText">文本内容</param>
        /// <param name="pixel">像素</param>
        /// <returns></returns>
        public Bitmap GetQRCode(string plainText, int pixel)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);//纠错程度

            var qrCode = new QRCoder.QRCode(qrCodeData);
            var bitmap = qrCode.GetGraphic(pixel);
            return bitmap;
        }
        #endregion
        #region 在二维码中间加入头像
        /// <summary>
        /// 在二维码中间加入头像
        /// </summary>
        /// <param name="plainText">文本内容</param>
        /// <param name="pixel">像素</param>
        /// <param name="logoPath"></param>
        /// <returns></returns>
        public Bitmap GetQRCodeWithLogo(string plainText, int pixel, string logoPath)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCoder.QRCode(qrCodeData);

            var bitmap = qrCode.GetGraphic(pixel, System.Drawing.Color.Black, System.Drawing.Color.White, (Bitmap)Image.FromFile(logoPath), 15, 6);
            return bitmap;
        }
        #endregion

        #region 生成svg格式的矢量二维码

        public string GetSvgQRCode(string plainText, int pixel)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
            var qrcode = new SvgQRCode(qrCodeData);
            return qrcode.GetGraphic(pixel);
        }
        #endregion
    }
}
