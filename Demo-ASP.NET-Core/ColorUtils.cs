//
// QR code generator library (.NET)
// https://github.com/manuelbl/QrCodeGenerator
//
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using SkiaSharp;
using System;
using System.Globalization;
using System.Linq; // Added missing import for System.Linq

namespace Net.Codecrete.QrCodeGenerator.Demo.Utils
{
    /// <summary>
    /// Utility class for color operations
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Converts hex color string to SKColor
        /// </summary>
        /// <param name="hexColor">Hex color string (e.g., "#FF0000")</param>
        /// <returns>SKColor object</returns>
        public static SKColor HexToSkColor(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                return SKColors.Black;

            // Remove # if present
            hexColor = hexColor.TrimStart('#');

            if (hexColor.Length != 6)
                return SKColors.Black;

            try
            {
                var r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                var g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                var b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

                return new SKColor(r, g, b);
            }
            catch
            {
                return SKColors.Black;
            }
        }

        /// <summary>
        /// Validates hex color string format
        /// </summary>
        /// <param name="hexColor">Hex color string to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidHexColor(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                return false;

            hexColor = hexColor.TrimStart('#');
            return hexColor.Length == 6 && 
                   hexColor.All(c => char.IsLetterOrDigit(c) && 
                   (char.IsDigit(c) || (char.ToUpper(c) >= 'A' && char.ToUpper(c) <= 'F')));
        }
    }
}
