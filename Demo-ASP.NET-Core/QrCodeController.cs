//
// QR code generator library (.NET)
// https://github.com/manuelbl/QrCodeGenerator
//
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Microsoft.AspNetCore.Mvc;
using Net.Codecrete.QrCodeGenerator;
using Net.Codecrete.QrCodeGenerator.Demo.Services;
using Net.Codecrete.QrCodeGenerator.Demo.Dtos;
using SkiaSharp;
using System;
using System.IO;
using System.Text;

namespace Net.Codecrete.QrCodeGenerator.Demo
{
    /// <summary>
    /// Controller for generating QR code as PNG or SVG images with optional logo
    /// </summary>
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private static readonly QrCode.Ecc[] errorCorrectionLevels = { QrCode.Ecc.Low, QrCode.Ecc.Medium, QrCode.Ecc.Quartile, QrCode.Ecc.High };
        private readonly IQrCodeService _qrCodeService;

        public QrCodeController(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        /// <summary>
        /// Generates QR code as PNG image
        /// </summary>
        /// <param name="text">Text to encode in QR code</param>
        /// <param name="ecc">Error correction level (0: low ... 3: high)</param>
        /// <param name="borderWidth">Border width in multiples of a module (QR code pixel)</param>
        /// <returns>PNG image</returns>
        [HttpGet("qrcode/png")]
        [ResponseCache(Duration = 2592000)]
        public ActionResult<byte[]> GeneratePng([FromQuery(Name = "text")] string text,
            [FromQuery(Name = "ecc")] int? ecc, [FromQuery(Name = "border")] int? borderWidth)
        {
            ecc = Math.Clamp(ecc ?? 1, 0, 3);
            borderWidth = Math.Clamp(borderWidth ?? 3, 0, 999999);

            var qrCode = QrCode.EncodeText(text, errorCorrectionLevels[(int)ecc]);
            byte[] png = qrCode.ToPng(20, (int)borderWidth);
            return new FileContentResult(png, "image/png");
        }

        /// <summary>
        /// Generates QR code as SVG image
        /// </summary>
        /// <param name="text">Text to encode in QR code</param>
        /// <param name="ecc">Error correction level (0: low ... 3: high)</param>
        /// <param name="borderWidth">Border width in multiples of a module (QR code pixel)</param>
        /// <returns>SVG image</returns>
        [HttpGet("qrcode/svg")]
        [ResponseCache(Duration = 2592000)]
        public ActionResult<byte[]> GenerateSvg([FromQuery(Name = "text")] string text,
            [FromQuery(Name = "ecc")] int? ecc, [FromQuery(Name = "border")] int? borderWidth)
        {
            ecc = Math.Clamp(ecc ?? 1, 0, 3);
            borderWidth = Math.Clamp(borderWidth ?? 3, 0, 999999);

            var qrCode = QrCode.EncodeText(text, errorCorrectionLevels[(int)ecc]);
            byte[] svg = Encoding.UTF8.GetBytes(qrCode.ToSvgString((int)borderWidth));
            return new FileContentResult(svg, "image/svg+xml; charset=utf-8");
        }

        /// <summary>
        /// Generates QR code as PNG image with embedded logo
        /// </summary>
        /// <param name="text">Text to encode in QR code</param>
        /// <param name="ecc">Error correction level (0: low ... 3: high)</param>
        /// <param name="borderWidth">Border width in multiples of a module (QR code pixel)</param>
        /// <param name="logoSize">Logo size as percentage of QR code (10-30)</param>
        /// <returns>PNG image with logo</returns>
        [HttpGet("qrcode/png-with-logo")]
        [ResponseCache(Duration = 2592000)]
        public ActionResult<byte[]> GeneratePngWithLogo([FromQuery(Name = "text")] string text,
            [FromQuery(Name = "ecc")] int? ecc, [FromQuery(Name = "border")] int? borderWidth,
            [FromQuery(Name = "logoSize")] int? logoSize)
        {
            ecc = Math.Clamp(ecc ?? 1, 0, 3);
            borderWidth = Math.Clamp(borderWidth ?? 3, 0, 999999);
            logoSize = Math.Clamp(logoSize ?? 15, 10, 30);

            byte[] png = _qrCodeService.GenerateQrCodeWithLogo(text, errorCorrectionLevels[(int)ecc], 20, (int)borderWidth, (int)logoSize);
            return new FileContentResult(png, "image/png");
        }

        /// <summary>
        /// Generates QR code as PNG image with custom logo and colors
        /// </summary>
        /// <param name="request">QR code generation request with logo and colors</param>
        /// <returns>PNG image with custom logo and colors</returns>
        [HttpPost("qrcode/custom")]
        public ActionResult<byte[]> GenerateCustomQrCode([FromForm] QrCodeWithLogoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                byte[] png = _qrCodeService.GenerateQrCodeWithCustomLogo(
                    request.Text,
                    errorCorrectionLevels[request.Ecc],
                    20,
                    request.BorderWidth,
                    request.LogoSize,
                    request.LogoFile,
                    request.ForegroundColor,
                    request.BackgroundColor,
                    request.LogoBorderColor,
                    request.LogoBackgroundColor
                );

                return new FileContentResult(png, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating QR code: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates QR code as PNG image with gradient frame and custom logo
        /// </summary>
        /// <param name="request">QR code generation request with gradient frame</param>
        /// <returns>PNG image with gradient frame and custom logo</returns>
        [HttpPost("qrcode/gradient-frame")]
        public ActionResult<byte[]> GenerateGradientFrameQrCode([FromForm] QrCodeWithGradientFrameRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Debug: Log the request values
            System.Diagnostics.Debug.WriteLine($"BottomTextFontSize: {request.BottomTextFontSize}");
            System.Diagnostics.Debug.WriteLine($"BottomText: {request.BottomText}");

            try
            {
                byte[] png = _qrCodeService.GenerateQrCodeWithGradientFrame(
                    request.Text,
                    errorCorrectionLevels[request.Ecc],
                    20,
                    request.BorderWidth,
                    request.LogoSize,
                    request.LogoFile,
                    request.ForegroundColor,
                    request.BackgroundColor,
                    request.LogoBorderColor,
                    request.LogoBackgroundColor,
                    request.GradientStartColor,
                    request.GradientEndColor,
                    request.FrameWidth,
                    request.CornerRadius,
                    request.BottomText,
                    request.BottomTextColor,
                    request.BottomTextFontSize
                );

                return new FileContentResult(png, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating QR code: {ex.Message}");
            }
        }


    }
}
