(function () {
    var gradientForm;
    var gradientQrCodeImage;
    var currentBlob = null; // Store the current QR code blob

    function handleGradientFormSubmit(event) {
        event.preventDefault();
        
        var formData = new FormData(gradientForm);
        
        // Show loading state
        var submitButton = gradientForm.querySelector('button[type="submit"]');
        var originalText = submitButton.textContent;
        submitButton.textContent = '🔄 Đang tạo...';
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
            // Store the blob for download
            currentBlob = blob;
            
            // Create object URL and update image
            var imageUrl = URL.createObjectURL(blob);
            gradientQrCodeImage.src = imageUrl;
            
            // Clean up object URL after image loads
            gradientQrCodeImage.onload = function() {
                URL.revokeObjectURL(imageUrl);
            };
            
            // Show success message
            showNotification('✅ QR Code với khung gradient đã được tạo thành công!', 'success');
            
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
        a.download = 'gradient-qrcode.png';
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
        var qrPreview = gradientQrCodeImage.parentElement;
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

    function applyPreset(presetType) {
        var presets = {
            'google': {
                gradientStartColor: '#4285f4',
                gradientEndColor: '#34a853',
                foregroundColor: '#4285f4',
                logoBorderColor: '#4285f4',
                bottomText: 'Google Play Store',
                bottomTextColor: '#4285f4',
                bottomTextFontSize: 36
            },
            'apple': {
                gradientStartColor: '#000000',
                gradientEndColor: '#333333',
                foregroundColor: '#000000',
                logoBorderColor: '#000000',
                bottomText: 'Apple App Store',
                bottomTextColor: '#000000',
                bottomTextFontSize: 40
            },
            'facebook': {
                gradientStartColor: '#1877f2',
                gradientEndColor: '#42a5f5',
                foregroundColor: '#1877f2',
                logoBorderColor: '#1877f2',
                bottomText: 'Facebook',
                bottomTextColor: '#1877f2',
                bottomTextFontSize: 32
            },
            'instagram': {
                gradientStartColor: '#833ab4',
                gradientEndColor: '#fd1d1d',
                foregroundColor: '#833ab4',
                logoBorderColor: '#833ab4',
                bottomText: 'Instagram',
                bottomTextColor: '#833ab4',
                bottomTextFontSize: 34
            }
        };

        var preset = presets[presetType];
        if (preset) {
            // Apply preset values
            document.getElementById('gradientStartColor').value = preset.gradientStartColor;
            document.getElementById('gradientEndColor').value = preset.gradientEndColor;
            document.getElementById('gradientForegroundColor').value = preset.foregroundColor;
            document.getElementById('gradientLogoBorderColor').value = preset.logoBorderColor;
            document.getElementById('bottomText').value = preset.bottomText;
            document.getElementById('bottomTextColor').value = preset.bottomTextColor;
            document.getElementById('bottomTextFontSize').value = preset.bottomTextFontSize;
            
            // Show notification
            showNotification('🎨 Đã áp dụng mẫu ' + presetType + '!', 'success');
        }
    }

    function init() {
        // Get form elements
        gradientForm = document.getElementById('gradientForm');
        gradientQrCodeImage = document.getElementById('gradientQrcode');

        // Add event listener for form submission
        gradientForm.addEventListener('submit', handleGradientFormSubmit);
        
        // Make applyPreset function globally available
        window.applyPreset = applyPreset;
        
        // Show download button for initial QR code
        if (gradientQrCodeImage.complete) {
            showDownloadButton();
        } else {
            gradientQrCodeImage.onload = function() {
                showDownloadButton();
            };
        }
    }

    init();
})();
