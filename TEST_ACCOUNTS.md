# Test Accounts - Tour Management System

## Tài khoản mẫu từ database Demo6-TourManagement.sql

### 👑 Admin Account
- **Username:** admin
- **Password:** 123456
- **Role:** admin
- **Expected Navigation:** AdminMainWindow
- **Features:** Quản lý toàn bộ hệ thống, dashboard, user management, reports

### 👨‍💼 Staff Account
- **Username:** staff1
- **Password:** 123456
- **Role:** staff
- **Expected Navigation:** StaffMainWindow
- **Features:** Quản lý tour, booking, payment, invoice

### 👨‍💼 Guide Account
- **Username:** guide1
- **Password:** 123456
- **Role:** staff
- **Expected Navigation:** StaffMainWindow
- **Features:** Quản lý tour, booking, payment, invoice

### 👤 Customer Account 1
- **Username:** customer1
- **Password:** 123456
- **Role:** customer
- **Expected Navigation:** CustomerMainWindow
- **Features:** Browse tours, my bookings, payments, reviews

### 👤 Customer Account 2
- **Username:** customer2
- **Password:** 123456
- **Role:** customer
- **Expected Navigation:** CustomerMainWindow
- **Features:** Browse tours, my bookings, payments, reviews

## Cách test:

1. **Chạy ứng dụng:**
   ```bash
   dotnet run --project TourManagementSystem
   ```

2. **Đăng nhập với từng tài khoản:**
   - Nhập username và password
   - Nhấn "Sign In"
   - Kiểm tra xem có chuyển đến đúng giao diện không

3. **Kiểm tra chức năng:**
   - **Admin:** Dashboard, User Management, Reports
   - **Staff:** Tour Management, Booking Management, Payment Management
   - **Customer:** Browse Tours, My Bookings, My Payments

## Expected Behavior:

✅ **Admin Login:**
- Chuyển đến AdminMainWindow
- Hiển thị "Welcome, Admin System"
- Có menu: Dashboard, User Management, Tour Management, etc.

✅ **Staff Login:**
- Chuyển đến StaffMainWindow
- Hiển thị "Welcome, Nguyễn Văn A" hoặc "Welcome, Phạm Thị D"
- Có menu: Dashboard, Tour Management, Booking Management, etc.

✅ **Customer Login:**
- Chuyển đến CustomerMainWindow
- Hiển thị "Welcome, Trần Thị B" hoặc "Welcome, Lê Văn C"
- Có menu: Dashboard, Browse Tours, My Bookings, etc.

## Troubleshooting:

❌ **Nếu đăng nhập thất bại:**
1. Kiểm tra database connection
2. Đảm bảo database đã được tạo với script Demo6-TourManagement.sql
3. Kiểm tra connection string trong appsettings.json

❌ **Nếu không chuyển đến đúng giao diện:**
1. Kiểm tra role trong database có đúng không
2. Kiểm tra console logs để xem lỗi
3. Đảm bảo tất cả MainWindow classes đã được build

❌ **Nếu có lỗi XAML:**
1. Chạy `dotnet clean` và `dotnet build`
2. Kiểm tra tất cả XAML files có syntax đúng không 