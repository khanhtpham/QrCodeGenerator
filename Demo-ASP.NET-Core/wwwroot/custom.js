(function () {
    var customForm;
    var customQrCodeImage;
    var currentBlob = null; // Store the current QR code blob

    function handleCustomFormSubmit(event) {
        event.preventDefault();
        
        var formData = new FormData(customForm);
        
        // Show loading state
        var submitButton = customForm.querySelector('button[type="submit"]');
        var originalText = submitButton.textContent;
        submitButton.textContent = '🔄 Đang tạo...';
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
            // Store the blob for download
            currentBlob = blob;
            
            // Create object URL and update image
            var imageUrl = URL.createObjectURL(blob);
            customQrCodeImage.src = imageUrl;
            
            // Clean up object URL after image loads
            customQrCodeImage.onload = function() {
                URL.revokeObjectURL(imageUrl);
            };
            
            // Show success message
            showNotification('✅ QR Code đã được tạo thành công!', 'success');
            
            // Show download button
            showDownloadButton();
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('❌ Có lỗi xảy ra khi tạo QR code: ' + error.message, 'error');
        })
        .finally(() => {
            // Restore button state
            submitButton.textContent = originalText;
            submitButton.disabled = false;
        });
    }

    function downloadQrCode() {
        if (!currentBlob) {
            showNotification('❌ Chưa có QR code để tải xuống!', 'error');
            return;
        }
        
        // Create download link
        var url = URL.createObjectURL(currentBlob);
        var a = document.createElement('a');
        a.href = url;
        a.download = 'custom-qrcode.png';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        
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
        var qrPreview = customQrCodeImage.parentElement;
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

    function init() {
        // Get form elements
        customForm = document.getElementById('customForm');
        customQrCodeImage = document.getElementById('customQrcode');

        // Add event listener for form submission
        customForm.addEventListener('submit', handleCustomFormSubmit);
        
        // Show download button for initial QR code
        if (customQrCodeImage.complete) {
            showDownloadButton();
        } else {
            customQrCodeImage.onload = function() {
                showDownloadButton();
            };
        }
    }

    init();
})();
