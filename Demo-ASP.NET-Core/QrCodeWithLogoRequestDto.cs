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
    /// DTO for QR code generation with logo upload and color customization
    /// Following DDD principles for data transfer
    /// </summary>
    public class QrCodeWithLogoRequestDto
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
        /// Logo shape (circle or square)
        /// </summary>
        [RegularExpression(@"^(circle|square)$", ErrorMessage = "Logo shape must be 'circle' or 'square'")]
        public string LogoShape { get; set; } = "circle";
    }
}
