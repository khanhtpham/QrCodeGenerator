(function () {
    var vcardForm;
    var vcardQrCodeImage;
    var currentBlob = null;

    function constructVCard() {
        var firstName = document.getElementById('firstName').value.trim();
        var lastName = document.getElementById('lastName').value.trim();
        var organization = document.getElementById('organization').value.trim();
        var jobTitle = document.getElementById('jobTitle').value.trim();
        var phone = document.getElementById('phone').value.trim();
        var email = document.getElementById('email').value.trim();

        var vcard = [
            'BEGIN:VCARD',
            'VERSION:3.0',
            'N:' + lastName + ';' + firstName,
            'FN:' + firstName + ' ' + lastName,
            organization ? 'ORG:' + organization : '',
            jobTitle ? 'TITLE:' + jobTitle : '',
            phone ? 'TEL:' + phone : '',
            email ? 'EMAIL:' + email : '',
            'END:VCARD'
        ].filter(line => line !== '').join('\n');

        return vcard;
    }

    function handleVCardFormSubmit(event) {
        event.preventDefault();

        var vcardText = constructVCard();
        var ecc = document.getElementById('customEcc').value;
        var foregroundColor = document.getElementById('foregroundColor').value;
        var backgroundColor = document.getElementById('backgroundColor').value;
        var borderWidth = document.getElementById('customBorder').value;
        var logoSize = document.getElementById('customLogoSize').value;
        var logoFile = document.getElementById('logoFile').files[0];
        var logoBorderColor = document.getElementById('logoBorderColor').value;
        var logoBackgroundColor = document.getElementById('logoBackgroundColor').value;

        var formData = new FormData();
        formData.append('Text', vcardText);
        formData.append('Ecc', ecc);
        formData.append('ForegroundColor', foregroundColor);
        formData.append('BackgroundColor', backgroundColor);
        formData.append('BorderWidth', borderWidth);
        formData.append('LogoSize', logoSize);
        if (logoFile) {
            formData.append('LogoFile', logoFile);
        }
        formData.append('LogoBorderColor', logoBorderColor);
        formData.append('LogoBackgroundColor', logoBackgroundColor);

        // Show loading state
        var submitButton = vcardForm.querySelector('button[type="submit"]');
        var originalText = submitButton.textContent;
        submitButton.textContent = '🔄 Đang tạo...';
        submitButton.disabled = true;

        fetch('qrcode/custom', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.blob();
            })
            .then(blob => {
                currentBlob = blob;
                var imageUrl = URL.createObjectURL(blob);
                vcardQrCodeImage.src = imageUrl;
                vcardQrCodeImage.onload = () => URL.revokeObjectURL(imageUrl);
                showNotification('✅ QR Code vCard đã được tạo!', 'success');
                showDownloadButton();
            })
            .catch(error => {
                console.error('Error:', error);
                showNotification('❌ Lỗi: ' + error.message, 'error');
            })
            .finally(() => {
                submitButton.textContent = originalText;
                submitButton.disabled = false;
            });
    }

    function downloadQrCode() {
        if (!currentBlob) return;
        var url = URL.createObjectURL(currentBlob);
        var a = document.createElement('a');
        a.href = url;
        a.download = 'vcard-qrcode.png';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }

    function showDownloadButton() {
        var existingButton = document.getElementById('downloadButton');
        if (existingButton) existingButton.remove();

        var downloadButton = document.createElement('button');
        downloadButton.id = 'downloadButton';
        downloadButton.type = 'button';
        downloadButton.className = 'pure-button-primary';
        downloadButton.textContent = '💾 Tải xuống QR Code';
        downloadButton.style.cssText = `
            margin-top: 15px; background: linear-gradient(135deg, #28a745, #20c997);
            border: none; color: white; font-weight: bold; padding: 12px 24px;
            font-size: 14px; border-radius: 25px; cursor: pointer; display: block;
            width: 100%; max-width: 280px; margin-left: auto; margin-right: auto;
        `;
        downloadButton.onclick = downloadQrCode;
        vcardQrCodeImage.parentElement.appendChild(downloadButton);
    }

    function showNotification(message, type) {
        var notification = document.createElement('div');
        notification.className = 'notification ' + type;
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed; top: 20px; right: 20px; padding: 15px 20px;
            border-radius: 8px; color: white; z-index: 1000;
        `;
        notification.style.background = type === 'success' ? '#4CAF50' : '#f44336';
        document.body.appendChild(notification);
        setTimeout(() => document.body.removeChild(notification), 3000);
    }

    function init() {
        vcardForm = document.getElementById('vcardForm');
        vcardQrCodeImage = document.getElementById('vcardQrcode');
        vcardForm.addEventListener('submit', handleVCardFormSubmit);
    }

    init();
})();
