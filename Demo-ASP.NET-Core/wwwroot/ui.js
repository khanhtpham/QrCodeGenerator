(function () {
    var qrCodeImage;
    var textField;
    var borderField;
    var eccSelect;
    var qrTypeSelect;
    var logoSizeField;
    var logoSizeContainer;

    // Custom form elements
    var customForm;
    var customQrCodeImage;
    var customTextField;
    var customBorderField;
    var customEccSelect;
    var customLogoSizeField;
    var logoFileField;
    var foregroundColorField;
    var backgroundColorField;
    var logoBorderColorField;
    var logoBackgroundColorField;

    // Gradient form elements
    var gradientForm;
    var gradientQrCodeImage;
    var gradientTextField;
    var gradientBorderField;
    var gradientEccSelect;
    var gradientLogoSizeField;
    var gradientLogoFileField;
    var gradientForegroundColorField;
    var gradientBackgroundColorField;
    var gradientLogoBorderColorField;
    var gradientLogoBackgroundColorField;
    var gradientStartColorField;
    var gradientEndColorField;
    var frameWidthField;
    var cornerRadiusField;
    var bottomTextField;

    function updateQrCode() {
        var qrType = qrTypeSelect.value;
        var url = new URL('qrcode/' + qrType, document.baseURI);
        url.searchParams.append('text', textField.value);
        url.searchParams.append('ecc', eccSelect.value);
        url.searchParams.append('border', borderField.value);
        
        // Add logo size parameter if using PNG with logo
        if (qrType === 'png-with-logo') {
            url.searchParams.append('logoSize', logoSizeField.value);
        }
        
        qrCodeImage.src = url;
    }

    function toggleLogoSizeVisibility() {
        if (qrTypeSelect.value === 'png-with-logo') {
            logoSizeContainer.style.display = 'block';
        } else {
            logoSizeContainer.style.display = 'none';
        }
    }

    function handleCustomFormSubmit(event) {
        event.preventDefault();
        
        var formData = new FormData(customForm);
        
        // Show loading state
        var submitButton = customForm.querySelector('button[type="submit"]');
        var originalText = submitButton.textContent;
        submitButton.textContent = 'Đang tạo...';
        submitButton.disabled = true;
        
        fetch('qrcode/custom', {
            method: 'POST',
            body: formData
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.blob();
        })
        .then(blob => {
            // Create object URL and update image
            var imageUrl = URL.createObjectURL(blob);
            customQrCodeImage.src = imageUrl;
            
            // Clean up object URL after image loads
            customQrCodeImage.onload = function() {
                URL.revokeObjectURL(imageUrl);
            };
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi tạo QR code: ' + error.message);
        })
        .finally(() => {
            // Restore button state
            submitButton.textContent = originalText;
            submitButton.disabled = false;
        });
    }

    function handleGradientFormSubmit(event) {
        event.preventDefault();
        
        var formData = new FormData(gradientForm);
        
        // Show loading state
        var submitButton = gradientForm.querySelector('button[type="submit"]');
        var originalText = submitButton.textContent;
        submitButton.textContent = 'Đang tạo...';
        submitButton.disabled = true;
        
        fetch('qrcode/gradient-frame', {
            method: 'POST',
            body: formData
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.blob();
        })
        .then(blob => {
            // Create object URL and update image
            var imageUrl = URL.createObjectURL(blob);
            gradientQrCodeImage.src = imageUrl;
            
            // Clean up object URL after image loads
            gradientQrCodeImage.onload = function() {
                URL.revokeObjectURL(imageUrl);
            };
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi tạo QR code: ' + error.message);
        })
        .finally(() => {
            // Restore button state
            submitButton.textContent = originalText;
            submitButton.disabled = false;
        });
    }

    function init() {
        // Basic form elements
        textField = document.getElementById('text');
        qrCodeImage = document.getElementById('qrcode');
        borderField = document.getElementById('border');
        eccSelect = document.getElementById('ecc');
        qrTypeSelect = document.getElementById('qrType');
        logoSizeField = document.getElementById('logoSize');
        logoSizeContainer = document.getElementById('logoSizeContainer');

        // Custom form elements
        customForm = document.getElementById('customForm');
        customQrCodeImage = document.getElementById('customQrcode');
        customTextField = document.getElementById('customText');
        customBorderField = document.getElementById('customBorder');
        customEccSelect = document.getElementById('customEcc');
        customLogoSizeField = document.getElementById('customLogoSize');
        logoFileField = document.getElementById('logoFile');
        foregroundColorField = document.getElementById('foregroundColor');
        backgroundColorField = document.getElementById('backgroundColor');
        logoBorderColorField = document.getElementById('logoBorderColor');
        logoBackgroundColorField = document.getElementById('logoBackgroundColor');

        // Gradient form elements
        gradientForm = document.getElementById('gradientForm');
        gradientQrCodeImage = document.getElementById('gradientQrcode');
        gradientTextField = document.getElementById('gradientText');
        gradientBorderField = document.getElementById('gradientBorder');
        gradientEccSelect = document.getElementById('gradientEcc');
        gradientLogoSizeField = document.getElementById('gradientLogoSize');
        gradientLogoFileField = document.getElementById('gradientLogoFile');
        gradientForegroundColorField = document.getElementById('gradientForegroundColor');
        gradientBackgroundColorField = document.getElementById('gradientBackgroundColor');
        gradientLogoBorderColorField = document.getElementById('gradientLogoBorderColor');
        gradientLogoBackgroundColorField = document.getElementById('gradientLogoBackgroundColor');
        gradientStartColorField = document.getElementById('gradientStartColor');
        gradientEndColorField = document.getElementById('gradientEndColor');
        frameWidthField = document.getElementById('frameWidth');
        cornerRadiusField = document.getElementById('cornerRadius');
        bottomTextField = document.getElementById('bottomText');

        // Add event listeners for basic form
        textField.onchange = function () { updateQrCode(); }
        textField.oninput = function () { updateQrCode(); }
        borderField.onchange = function () { updateQrCode(); }
        borderField.oninput = function () { updateQrCode(); }
        eccSelect.onchange = function () { updateQrCode(); }
        qrTypeSelect.onchange = function () { 
            toggleLogoSizeVisibility();
            updateQrCode(); 
        }
        logoSizeField.onchange = function () { updateQrCode(); }
        logoSizeField.oninput = function () { updateQrCode(); }

        // Add event listeners for forms
        customForm.addEventListener('submit', handleCustomFormSubmit);
        gradientForm.addEventListener('submit', handleGradientFormSubmit);

        // Initialize logo size visibility
        toggleLogoSizeVisibility();
    }

    init();
})();
