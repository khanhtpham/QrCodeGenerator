//
// QR code generator library (.NET)
// https://github.com/manuelbl/QrCodeGenerator
//
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Net.Codecrete.QrCodeGenerator.Demo.Dtos
{
    /// <summary>
    /// DTO for QR code generation with gradient frame
    /// Following DDD principles for data transfer
    /// </summary>
    public class QrCodeWithGradientFrameRequestDto
    {
        /// <summary>
        /// Text to encode in QR code
        /// </summary>
        [Required(ErrorMessage = "Text is required")]
        [StringLength(1000, ErrorMessage = "Text cannot exceed 1000 characters")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Error correction level (0: low, 1: medium, 2: quartile, 3: high)
        /// </summary>
        [Range(0, 3, ErrorMessage = "Error correction level must be between 0 and 3")]
        public int Ecc { get; set; } = 1;

        /// <summary>
        /// Border width in multiples of a module
        /// </summary>
        [Range(0, 999999, ErrorMessage = "Border width must be between 0 and 999999")]
        public int BorderWidth { get; set; } = 3;

        /// <summary>
        /// Logo size as percentage of QR code
        /// </summary>
        [Range(10, 30, ErrorMessage = "Logo size must be between 10% and 30%")]
        public int LogoSize { get; set; } = 15;

        /// <summary>
        /// Uploaded logo file
        /// </summary>
        public IFormFile? LogoFile { get; set; }

        /// <summary>
        /// QR code foreground color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Foreground color must be in hex format (#RRGGBB)")]
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// QR code background color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Background color must be in hex format (#RRGGBB)")]
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// Logo border color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Logo border color must be in hex format (#RRGGBB)")]
        public string LogoBorderColor { get; set; } = "#000000";

        /// <summary>
        /// Logo background color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Logo background color must be in hex format (#RRGGBB)")]
        public string LogoBackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// Gradient start color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Gradient start color must be in hex format (#RRGGBB)")]
        public string GradientStartColor { get; set; } = "#1E3A8A";

        /// <summary>
        /// Gradient end color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Gradient end color must be in hex format (#RRGGBB)")]
        public string GradientEndColor { get; set; } = "#06B6D4";

        /// <summary>
        /// Frame width in pixels
        /// </summary>
        [Range(10, 200, ErrorMessage = "Frame width must be between 10 and 200 pixels")]
        public int FrameWidth { get; set; } = 40;

        /// <summary>
        /// Corner radius in pixels
        /// </summary>
        [Range(0, 100, ErrorMessage = "Corner radius must be between 0 and 100 pixels")]
        public int CornerRadius { get; set; } = 20;

        /// <summary>
        /// Text to display below QR code
        /// </summary>
        [StringLength(100, ErrorMessage = "Bottom text cannot exceed 100 characters")]
        public string? BottomText { get; set; }

        /// <summary>
        /// Bottom text color (hex format: #RRGGBB)
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Bottom text color must be in hex format (#RRGGBB)")]
        public string BottomTextColor { get; set; } = "#1E3A8A";

        /// <summary>
        /// Bottom text font size in pixels
        /// </summary>
        [Range(12, 72, ErrorMessage = "Font size must be between 12 and 72 pixels")]
        public int BottomTextFontSize { get; set; } = 32;
    }
}
