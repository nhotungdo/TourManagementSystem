# Tour Management System

Hệ thống quản lý tour du lịch với các chức năng quản lý người dùng, tour, đặt chỗ, thanh toán và báo cáo.

## 🚀 Tính năng chính

### 👥 Quản lý người dùng
- Đăng nhập/Đăng xuất
- Phân quyền theo role (Admin, Staff, Customer)
- Quản lý thông tin người dùng
- Đặt lại mật khẩu

### 🗺️ Quản lý Tour
- Tạo và quản lý tour du lịch
- Quản lý lịch trình tour
- Quản lý điểm tham quan
- Thiết lập giá và khuyến mãi

### 📋 Quản lý đặt chỗ
- Đặt tour cho khách hàng
- Quản lý trạng thái đặt chỗ
- Xử lý thanh toán
- Tạo hóa đơn

### 💳 Quản lý thanh toán
- Xử lý các phương thức thanh toán
- Quản lý giao dịch
- Báo cáo doanh thu

### 📊 Báo cáo và thống kê
- Dashboard tổng quan
- Báo cáo doanh thu
- Thống kê đặt chỗ
- Log hoạt động

## 🔧 Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server hoặc SQL Server Express
- Visual Studio 2022 hoặc VS Code

## 👤 Tài khoản mẫu

### Admin
- **Username:** admin
- **Password:** 123456
- **Role:** Admin
- **Quyền:** Quản lý toàn bộ hệ thống

### Staff
- **Username:** staff1
- **Password:** 123456
- **Role:** Staff
- **Quyền:** Quản lý tour, đặt chỗ, thanh toán

### Customer
- **Username:** customer1
- **Password:** 123456
- **Role:** Customer
- **Quyền:** Xem tour, đặt chỗ, thanh toán

## 🏗️ Kiến trúc hệ thống

### Models
- `User` - Quản lý người dùng
- `Tour` - Thông tin tour
- `TourSchedule` - Lịch trình tour
- `Booking` - Đặt chỗ
- `Payment` - Thanh toán
- `Review` - Đánh giá
- `Promotion` - Khuyến mãi
- `ActivityLog` - Log hoạt động

### Services
- `AuthService` - Xác thực và phân quyền
- `UserService` - Quản lý người dùng
- `TourService` - Quản lý tour
- `BookingService` - Quản lý đặt chỗ
- `PaymentService` - Quản lý thanh toán
- `ActivityLogService` - Ghi log hoạt động

### Views
- `LoginWindow` - Màn hình đăng nhập
- `AdminMainWindow` - Giao diện Admin
- `StaffMainWindow` - Giao diện Staff
- `CustomerMainWindow` - Giao diện Customer

## 🔐 Bảo mật

- Mật khẩu được hash bằng SHA256
- Phân quyền theo role
- Log hoạt động người dùng
- Validation dữ liệu đầu vào

## 📝 Ghi chú

- Hệ thống hỗ trợ cả mật khẩu plain text (từ database cũ) và hashed password
- Tự động chuyển đổi mật khẩu plain text sang hashed khi đăng nhập
- Các chức năng nâng cao sẽ được implement trong các phiên bản tiếp theo

## 🐛 Xử lý lỗi

Nếu gặp lỗi khi chạy ứng dụng:

1. Kiểm tra connection string trong `appsettings.json`
2. Đảm bảo database đã được tạo và có dữ liệu
3. Kiểm tra .NET 8.0 SDK đã được cài đặt
4. Chạy `dotnet clean` và `dotnet build` lại

