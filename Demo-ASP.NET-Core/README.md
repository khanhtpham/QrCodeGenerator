# QR Code Generator Demo

Ứng dụng demo tạo QR code với ASP.NET Core, hỗ trợ tạo QR code với logo upload và tùy chỉnh màu sắc.

## Tính năng

### QR Code Cơ bản
- Tạo QR code dạng SVG (không có logo)
- Tạo QR code dạng PNG (không có logo)  
- Tạo QR code dạng PNG với logo mặc định ở giữa
- Điều chỉnh mức độ sửa lỗi (Error Correction)
- Điều chỉnh độ rộng viền (Border Width)
- Điều chỉnh kích thước logo (10% - 30% của QR code)

### QR Code Tùy chỉnh (Mới)
- **Upload logo thực tế**: Hỗ trợ upload file ảnh (PNG, JPG, GIF, etc.)
- **Tùy chỉnh màu sắc QR code**: Màu foreground và background
- **Tùy chỉnh màu sắc logo**: Màu viền và background của logo
- **Logo được cắt tròn**: Logo được hiển thị trong hình tròn với background tùy chỉnh
- **Tất cả tính năng cơ bản**: Error correction, border width, logo size

## Kiến trúc

Dự án được thiết kế theo mô hình **Domain-Driven Design (DDD)** và tuân thủ các nguyên tắc của **ABP Framework**:

### Layers
- **Controller Layer**: `QrCodeController` - Xử lý HTTP requests
- **Service Layer**: `QrCodeService` - Chứa business logic
- **DTO Layer**: 
  - `QrCodeRequestDto` - DTO cơ bản
  - `QrCodeWithLogoRequestDto` - DTO với logo và màu sắc
- **Utils Layer**: `ColorUtils` - Xử lý chuyển đổi màu sắc
- **Domain Layer**: Sử dụng thư viện `Net.Codecrete.QrCodeGenerator`

### Dependencies
- `Net.Codecrete.QrCodeGenerator` (v2.0.7) - Thư viện tạo QR code
- `SkiaSharp` (v2.88.6) - Xử lý hình ảnh và vẽ logo

## API Endpoints

### 1. Tạo QR code SVG
```
GET /qrcode/svg?text={text}&ecc={0-3}&border={0-999999}
```

### 2. Tạo QR code PNG
```
GET /qrcode/png?text={text}&ecc={0-3}&border={0-999999}
```

### 3. Tạo QR code PNG với logo mặc định
```
GET /qrcode/png-with-logo?text={text}&ecc={0-3}&border={0-999999}&logoSize={10-30}
```

### 4. Tạo QR code PNG với logo tùy chỉnh (Mới)
```
POST /qrcode/custom
Content-Type: multipart/form-data

Form data:
- Text: string (required)
- Ecc: int (0-3)
- BorderWidth: int (0-999999)
- LogoSize: int (10-30)
- LogoFile: file (optional)
- ForegroundColor: string (hex format: #RRGGBB)
- BackgroundColor: string (hex format: #RRGGBB)
- LogoBorderColor: string (hex format: #RRGGBB)
- LogoBackgroundColor: string (hex format: #RRGGBB)
```

### Parameters
- `text`: Nội dung cần mã hóa (bắt buộc)
- `ecc`: Mức độ sửa lỗi (0: Low, 1: Medium, 2: Quartile, 3: High)
- `border`: Độ rộng viền (số module)
- `logoSize`: Kích thước logo (% của QR code)
- `LogoFile`: File ảnh logo (hỗ trợ PNG, JPG, GIF, etc.)
- `ForegroundColor`: Màu các chấm QR code (hex format)
- `BackgroundColor`: Màu nền QR code (hex format)
- `LogoBorderColor`: Màu viền logo (hex format)
- `LogoBackgroundColor`: Màu nền logo (hex format)

## Cách sử dụng

1. Chạy ứng dụng:
```bash
dotnet run
```

2. Mở trình duyệt và truy cập: `http://localhost:5000`

3. **QR Code Cơ bản**:
   - Nhập text cần tạo QR code
   - Chọn loại QR code (SVG, PNG, PNG với logo)
   - Điều chỉnh các thông số cơ bản

4. **QR Code Tùy chỉnh**:
   - Nhập text cần tạo QR code
   - Upload logo (tùy chọn)
   - Tùy chỉnh màu sắc QR code và logo
   - Điều chỉnh các thông số khác
   - Nhấn "Tạo QR Code Tùy chỉnh"

## Lưu ý kỹ thuật

### Logo Upload
- Hỗ trợ các định dạng: PNG, JPG, JPEG, GIF, BMP, WebP
- Giới hạn kích thước file: 10MB
- Logo được tự động resize và cắt tròn
- Nếu không upload logo, sẽ hiển thị hình tròn với màu tùy chỉnh

### Màu sắc
- Sử dụng định dạng hex: #RRGGBB (ví dụ: #FF0000 cho màu đỏ)
- Màu mặc định: QR đen trên nền trắng
- Logo background mặc định: trắng với viền đen

### Error Correction
- Khuyến nghị dùng Medium (1) hoặc High (3) khi có logo
- Error correction cao hơn giúp QR code vẫn đọc được khi có logo che phủ
- Kích thước logo được giới hạn 10-30% để đảm bảo QR code vẫn có thể quét được

## Mở rộng

Để mở rộng thêm tính năng:
1. Thêm validation cho định dạng và kích thước ảnh
2. Thêm tùy chọn vị trí logo (không chỉ ở giữa)
3. Thêm hiệu ứng cho logo (shadow, gradient, etc.)
4. Thêm tùy chọn định dạng output (JPG, WebP, etc.)
5. Thêm watermark hoặc branding
6. Thêm batch processing cho nhiều QR code
