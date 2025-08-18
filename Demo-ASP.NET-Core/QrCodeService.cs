//
// QR code generator library (.NET)
// https://github.com/manuelbl/QrCodeGenerator
//
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Net.Codecrete.QrCodeGenerator;
using Net.Codecrete.QrCodeGenerator.Demo.Utils;
using SkiaSharp;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Net.Codecrete.QrCodeGenerator.Demo.Services
{
    /// <summary>
    /// Service for generating QR codes with advanced features like logo embedding
    /// Following DDD principles and ABP Framework patterns
    /// </summary>
    public interface IQrCodeService
    {
        /// <summary>
        /// Generates QR code as PNG with embedded logo
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <param name="ecc">Error correction level</param>
        /// <param name="scale">Scale factor</param>
        /// <param name="border">Border width</param>
        /// <param name="logoSizePercent">Logo size as percentage</param>
        /// <returns>PNG image bytes</returns>
        byte[] GenerateQrCodeWithLogo(string text, QrCode.Ecc ecc, int scale, int border, int logoSizePercent);

        /// <summary>
        /// Generates QR code as PNG with custom logo and colors
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <param name="ecc">Error correction level</param>
        /// <param name="scale">Scale factor</param>
        /// <param name="border">Border width</param>
        /// <param name="logoSizePercent">Logo size as percentage</param>
        /// <param name="logoFile">Uploaded logo file</param>
        /// <param name="foregroundColor">QR code foreground color</param>
        /// <param name="backgroundColor">QR code background color</param>
        /// <param name="logoBorderColor">Logo border color</param>
        /// <param name="logoBackgroundColor">Logo background color</param>
        /// <returns>PNG image bytes</returns>
        byte[] GenerateQrCodeWithCustomLogo(string text, QrCode.Ecc ecc, int scale, int border, int logoSizePercent,
            IFormFile? logoFile, string foregroundColor, string backgroundColor, string logoBorderColor, string logoBackgroundColor);
    }

    /// <summary>
    /// Implementation of QR code service with logo embedding capabilities
    /// </summary>
    public class QrCodeService : IQrCodeService
    {
        private static QrCode.Ecc EnsureMinQuartile(QrCode.Ecc ecc)
        {
            if (ecc == QrCode.Ecc.Low || ecc == QrCode.Ecc.Medium)
                return QrCode.Ecc.Quartile;
            return ecc;
        }

        /// <summary>
        /// Render QR code bitmap and reserve a centered area in module units so no module is partially covered.
        /// This greatly improves scan reliability when embedding a logo.
        /// </summary>
        /// <param name="qrCode">QR code entity</param>
        /// <param name="scale">Module scale (pixels per module)</param>
        /// <param name="border">Border width (modules)</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <param name="reservedModules">Width/height of the centered square to keep empty (modules); 0 to disable</param>
        /// <returns>Bitmap with reserved area</returns>
        private SKBitmap RenderQrWithReservedArea(QrCode qrCode, int scale, int border, SKColor foreground, SKColor background, int reservedModules)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale));
            if (border < 0)
                throw new ArgumentOutOfRangeException(nameof(border));

            int size = qrCode.Size;
            int dim = (size + border * 2) * scale;

            // clamp reserved area and align to even number to center perfectly
            if (reservedModules < 0)
                reservedModules = 0;
            int maxReserved = Math.Max(0, size - 16); // keep a safety margin away from edges and finder/timing areas
            reservedModules = Math.Min(reservedModules, maxReserved);
            if ((reservedModules & 1) == 1)
                reservedModules -= 1; // even width centers better

            int reservedStart = (size - reservedModules) / 2;
            int reservedEnd = reservedStart + reservedModules; // exclusive

            SKBitmap bitmap = new SKBitmap(dim, dim, SKColorType.Rgb888x, SKAlphaType.Opaque);
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                using (SKPaint paint = new SKPaint { Color = background })
                {
                    canvas.DrawRect(0, 0, dim, dim, paint);
                }

                using (SKPaint paint = new SKPaint { Color = foreground, IsAntialias = false })
                {
                    for (int y = 0; y < size; y++)
                    {
                        bool insideReservedY = reservedModules > 0 && y >= reservedStart && y < reservedEnd;
                        for (int x = 0; x < size; x++)
                        {
                            if (reservedModules > 0 && insideReservedY && x >= reservedStart && x < reservedEnd)
                                continue; // skip reserved center area
                            if (qrCode.GetModule(x, y))
                            {
                                canvas.DrawRect((x + border) * scale, (y + border) * scale, scale, scale, paint);
                            }
                        }
                    }
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Compute number of modules to reserve based on logo size percent and QR size.
        /// Adds 2-module padding so the logo never touches data modules.
        /// </summary>
        private int ComputeReservedModules(int qrSize, int logoSizePercent)
        {
            double ratio = Math.Clamp(logoSizePercent / 100.0, 0.1, 0.3);
            int modules = (int)Math.Round(qrSize * ratio);
            modules = Math.Max(1, modules);
            modules += 2; // padding around logo
            return modules;
        }

        /// <summary>
        /// Generates QR code with logo using SkiaSharp
        /// </summary>
        /// <param name="text">Text to encode in QR code</param>
        /// <param name="ecc">Error correction level</param>
        /// <param name="scale">Scale factor for QR code modules</param>
        /// <param name="border">Border width</param>
        /// <param name="logoSizePercent">Logo size as percentage of QR code</param>
        /// <returns>PNG image bytes</returns>
        public byte[] GenerateQrCodeWithLogo(string text, QrCode.Ecc ecc, int scale, int border, int logoSizePercent)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be null or empty", nameof(text));

            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be positive");

            if (border < 0)
                throw new ArgumentOutOfRangeException(nameof(border), "Border cannot be negative");

            if (logoSizePercent < 10 || logoSizePercent > 30)
                throw new ArgumentOutOfRangeException(nameof(logoSizePercent), "Logo size must be between 10% and 30%");

            // Ensure minimum ECC when embedding logo
            var eccForLogo = EnsureMinQuartile(ecc);

            // Generate QR code
            var qrCode = QrCode.EncodeText(text, eccForLogo);

            // Render QR with a reserved center area aligned to module grid
            int reservedModules = ComputeReservedModules(qrCode.Size, logoSizePercent);
            using SKBitmap qrBitmap = RenderQrWithReservedArea(qrCode, scale, border, SKColors.Black, SKColors.White, reservedModules);
            
            // Create final bitmap with same dimensions
            using SKBitmap finalBitmap = new SKBitmap(qrBitmap.Width, qrBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using SKCanvas canvas = new SKCanvas(finalBitmap);

            // Draw QR code
            canvas.DrawBitmap(qrBitmap, 0, 0);

            // Calculate logo size and position based on reserved pixel area
            int modulePixels = scale;
            int reservedPixels = reservedModules * modulePixels;
            int logoSize = (int)Math.Round(reservedPixels * 0.8); // keep margin to the reserved border
            float centerX = qrBitmap.Width / 2.0f;
            float centerY = qrBitmap.Height / 2.0f;
            float radius = logoSize / 2.0f;

            // Create logo background (white circle with border)
            using SKPaint logoPaint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using SKPaint borderPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true
            };

            // Draw logo
            canvas.DrawCircle(centerX, centerY, radius, logoPaint);
            canvas.DrawCircle(centerX, centerY, radius, borderPaint);

            // Encode to PNG
            using SKData data = finalBitmap.Encode(SKEncodedImageFormat.Png, 90);
            return data.ToArray();
        }

        /// <summary>
        /// Generates QR code with custom logo and colors using SkiaSharp
        /// </summary>
        /// <param name="text">Text to encode in QR code</param>
        /// <param name="ecc">Error correction level</param>
        /// <param name="scale">Scale factor for QR code modules</param>
        /// <param name="border">Border width</param>
        /// <param name="logoSizePercent">Logo size as percentage of QR code</param>
        /// <param name="logoFile">Uploaded logo file</param>
        /// <param name="foregroundColor">QR code foreground color</param>
        /// <param name="backgroundColor">QR code background color</param>
        /// <param name="logoBorderColor">Logo border color</param>
        /// <param name="logoBackgroundColor">Logo background color</param>
        /// <returns>PNG image bytes</returns>
        public byte[] GenerateQrCodeWithCustomLogo(string text, QrCode.Ecc ecc, int scale, int border, int logoSizePercent,
            IFormFile? logoFile, string foregroundColor, string backgroundColor, string logoBorderColor, string logoBackgroundColor)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be null or empty", nameof(text));

            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be positive");

            if (border < 0)
                throw new ArgumentOutOfRangeException(nameof(border), "Border cannot be negative");

            if (logoSizePercent < 10 || logoSizePercent > 30)
                throw new ArgumentOutOfRangeException(nameof(logoSizePercent), "Logo size must be between 10% and 30%");

            // Convert colors
            var fgColor = ColorUtils.HexToSkColor(foregroundColor);
            var bgColor = ColorUtils.HexToSkColor(backgroundColor);
            var logoBorder = ColorUtils.HexToSkColor(logoBorderColor);
            var logoBg = ColorUtils.HexToSkColor(logoBackgroundColor);

            // Ensure minimum ECC when embedding logo
            var eccForLogo = EnsureMinQuartile(ecc);

            // Generate QR code
            var qrCode = QrCode.EncodeText(text, eccForLogo);

            // Render QR with a reserved center area aligned to module grid
            int reservedModules = ComputeReservedModules(qrCode.Size, logoSizePercent);
            using SKBitmap qrBitmap = RenderQrWithReservedArea(qrCode, scale, border, fgColor, bgColor, reservedModules);
            
            // Create final bitmap with same dimensions
            using SKBitmap finalBitmap = new SKBitmap(qrBitmap.Width, qrBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using SKCanvas canvas = new SKCanvas(finalBitmap);

            // Draw QR code
            canvas.DrawBitmap(qrBitmap, 0, 0);

            // Calculate logo size and position based on reserved pixel area
            int modulePixels = scale;
            int reservedPixels = reservedModules * modulePixels;
            int logoSize = (int)Math.Round(reservedPixels * 0.8); // keep margin to the reserved border
            float centerX = qrBitmap.Width / 2.0f;
            float centerY = qrBitmap.Height / 2.0f;
            float radius = logoSize / 2.0f;

            // Create logo background (circle with custom colors)
            using SKPaint logoPaint = new SKPaint
            {
                Color = logoBg,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            using SKPaint borderPaint = new SKPaint
            {
                Color = logoBorder,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true
            };

            // Draw logo background circle
            canvas.DrawCircle(centerX, centerY, radius, logoPaint);
            canvas.DrawCircle(centerX, centerY, radius, borderPaint);

            // Draw uploaded logo if provided
            if (logoFile != null && logoFile.Length > 0)
            {
                try
                {
                    using var stream = logoFile.OpenReadStream();
                    using var logoBitmap = SKBitmap.Decode(stream);
                    
                    if (logoBitmap != null)
                    {
                        // Calculate logo display size (slightly smaller than background)
                        float logoDisplaySize = radius * 1.6f; // 80% of background circle
                        
                        // Create a rounded rectangle clip for the logo
                        using var logoClip = new SKPath();
                        logoClip.AddCircle(centerX, centerY, logoDisplaySize / 2);
                        
                        // Save canvas state
                        canvas.Save();
                        canvas.ClipPath(logoClip);
                        
                        // Calculate logo position to center it
                        float logoX = centerX - logoDisplaySize / 2;
                        float logoY = centerY - logoDisplaySize / 2;
                        
                        // Draw logo with scaling
                        var logoRect = new SKRect(logoX, logoY, logoX + logoDisplaySize, logoY + logoDisplaySize);
                        canvas.DrawBitmap(logoBitmap, logoRect);
                        
                        // Restore canvas state
                        canvas.Restore();
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue without logo
                    System.Diagnostics.Debug.WriteLine($"Error loading logo: {ex.Message}");
                }
            }

            // Encode to PNG
            using SKData data = finalBitmap.Encode(SKEncodedImageFormat.Png, 90);
            return data.ToArray();
        }
    }
}
