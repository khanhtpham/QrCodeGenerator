(function () {
    var qrCodeImage;
    var textField;
    var borderField;
    var eccSelect;
    var qrTypeSelect;
    var logoSizeField;
    var logoSizeContainer;
    var currentImageUrl = null; // Store the current image URL

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
        currentImageUrl = url;
        
        // Show download button after image loads
        qrCodeImage.onload = function() {
            showDownloadButton();
        };
    }

    function downloadQrCode() {
        if (!currentImageUrl) {
            showNotification('❌ Chưa có QR code để tải xuống!', 'error');
            return;
        }
        
        // Create download link
        var a = document.createElement('a');
        a.href = currentImageUrl;
        a.download = 'basic-qrcode.png';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        
        showNotification('💾 QR Code đã được tải xuống!', 'success');
    }

    function showDownloadButton() {
        // Remove existing download button if any
        var existingButton = document.getElementById('downloadButton');
        if (existingButton) {
            existingButton.remove();
        }
        
        // Create download button
        var downloadButton = document.createElement('button');
        downloadButton.id = 'downloadButton';
        downloadButton.type = 'button';
        downloadButton.className = 'pure-button-primary';
        downloadButton.textContent = '💾 Tải xuống QR Code';
        downloadButton.style.cssText = `
            margin-top: 15px;
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            font-weight: bold;
            padding: 12px 24px;
            font-size: 14px;
            border-radius: 25px;
            transition: all 0.3s ease;
            cursor: pointer;
            box-shadow: 0 4px 15px rgba(40, 167, 69, 0.3);
            display: block;
            width: 100%;
            max-width: 280px;
            margin-left: auto;
            margin-right: auto;
        `;
        
        downloadButton.onclick = downloadQrCode;
        downloadButton.onmouseover = function() {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 6px 20px rgba(40, 167, 69, 0.4)';
        };
        downloadButton.onmouseout = function() {
            this.style.transform = 'translateY(0)';
            this.style.boxShadow = '0 4px 15px rgba(40, 167, 69, 0.3)';
        };
        
        // Add button after the QR code image
        var qrPreview = qrCodeImage.parentElement;
        qrPreview.appendChild(downloadButton);
    }

    function showNotification(message, type) {
        // Create notification element
        var notification = document.createElement('div');
        notification.className = 'notification ' + type;
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 20px;
            border-radius: 8px;
            color: white;
            font-weight: 500;
            z-index: 1000;
            animation: slideIn 0.3s ease;
            max-width: 300px;
        `;
        
        if (type === 'success') {
            notification.style.background = 'linear-gradient(135deg, #4CAF50, #45a049)';
        } else {
            notification.style.background = 'linear-gradient(135deg, #f44336, #d32f2f)';
        }
        
        // Add CSS animation
        var style = document.createElement('style');
        style.textContent = `
            @keyframes slideIn {
                from { transform: translateX(100%); opacity: 0; }
                to { transform: translateX(0); opacity: 1; }
            }
            @keyframes slideOut {
                from { transform: translateX(0); opacity: 1; }
                to { transform: translateX(100%); opacity: 0; }
            }
        `;
        document.head.appendChild(style);
        
        // Add to page
        document.body.appendChild(notification);
        
        // Remove after 3 seconds
        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => {
                document.body.removeChild(notification);
            }, 300);
        }, 3000);
    }

    function toggleLogoSizeVisibility() {
        if (qrTypeSelect.value === 'png-with-logo') {
            logoSizeContainer.style.display = 'block';
        } else {
            logoSizeContainer.style.display = 'none';
        }
    }

    function init() {
        // Get form elements
        textField = document.getElementById('text');
        qrCodeImage = document.getElementById('qrcode');
        borderField = document.getElementById('border');
        eccSelect = document.getElementById('ecc');
        qrTypeSelect = document.getElementById('qrType');
        logoSizeField = document.getElementById('logoSize');
        logoSizeContainer = document.getElementById('logoSizeContainer');

        // Add event listeners
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

        // Initialize logo size visibility
        toggleLogoSizeVisibility();
        
        // Initial QR code generation
        updateQrCode();
    }

    init();
})();
