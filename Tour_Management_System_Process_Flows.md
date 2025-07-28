# TOUR MANAGEMENT SYSTEM
## Các Luồng Xử Lý Chính

---

## 1. LUỒNG KHỞI ĐỘNG HỆ THỐNG

### 🔄 Quy trình khởi động
```
1. App.xaml.cs OnStartup()
   ├── Khởi tạo Database Context
   ├── EnsureCreated() - Tạo database nếu chưa có
   ├── SeedData.Initialize() - Nạp dữ liệu mẫu
   ├── Hiển thị LoginWindow
   └── Set MainWindow = LoginWindow
```

### ⚙️ Các bước chi tiết
1. **Database Initialization**
   - Kiểm tra kết nối database
   - Tạo database schema nếu chưa tồn tại
   - Nạp dữ liệu mẫu (users, tours, schedules)

2. **UI Initialization**
   - Hiển thị màn hình đăng nhập
   - Thiết lập ShutdownMode
   - Xử lý lỗi khởi tạo

---

## 2. LUỒNG XÁC THỰC VÀ ĐĂNG NHẬP

### 🔐 Quy trình đăng nhập
```
1. User nhập Username/Password
2. LoginViewModel.ValidateInput()
3. AuthService.AuthenticateUser()
   ├── HashPassword() - Mã hóa mật khẩu
   ├── Tìm user trong database
   ├── Kiểm tra mật khẩu (hashed/plain text)
   └── Cập nhật mật khẩu sang hashed nếu cần
4. OnLoginSuccessful() - Chuyển hướng theo role
   ├── Admin → AdminMainWindow
   ├── Staff → StaffMainWindow
   └── Customer → CustomerMainWindow
5. ActivityLogService.LogActivity() - Ghi log
```

### 🛡️ Bảo mật
- **Password Hashing:** SHA256
- **Role-based Access:** Admin/Staff/Customer
- **Session Management:** User context trong window
- **Activity Logging:** Ghi lại mọi hoạt động

---

## 3. LUỒNG QUẢN LÝ TOUR

### 🗺️ Quy trình tạo tour
```
1. Admin/Staff mở TourManagementPage
2. Click "Add Tour" → AddTourWindow
3. Nhập thông tin tour:
   ├── TourName, Description
   ├── DurationDays, BasePrice
   ├── Attractions (nhiều điểm tham quan)
   └── Promotions (khuyến mãi)
4. TourService.CreateTour()
   ├── Validation dữ liệu
   ├── Lưu vào database
   └── ActivityLogService.LogActivity()
5. Refresh TourManagementPage
```

### 📅 Quy trình quản lý lịch trình
```
1. Mở ScheduleManagementPage
2. Chọn tour → AddScheduleWindow
3. Nhập thông tin lịch trình:
   ├── DepartureDate, ReturnDate
   ├── MaxCapacity, CurrentCapacity
   ├── Status (active/inactive)
   └── Notes
4. TourScheduleService.CreateSchedule()
5. Cập nhật danh sách lịch trình
```

---

## 4. LUỒNG ĐẶT CHỖ (BOOKING)

### 📋 Quy trình đặt chỗ của Customer
```
1. Customer mở BrowseToursPage
2. Chọn tour → BookTourPage
3. Chọn lịch trình phù hợp
4. Nhập thông tin đặt chỗ:
   ├── NumAdults, NumChildren
   ├── SpecialRequests
   └── Contact Information
5. BookingService.CreateBooking()
   ├── Validate schedule availability
   ├── CalculateTotalPrice()
   ├── ApplyPromotion() nếu có
   ├── Set status = "pending"
   └── CreateInvoice()
6. Chuyển đến PaymentPage
```

### 💰 Quy trình thanh toán
```
1. Customer chọn phương thức thanh toán
2. PaymentService.CreatePayment()
   ├── Validate booking
   ├── Set payment status = "pending"
   └── Log activity
3. Staff/Admin xử lý thanh toán:
   ├── PaymentService.UpdatePaymentStatus()
   ├── BookingService.UpdateBookingStatus()
   └── InvoiceService.GenerateInvoice()
4. Gửi email xác nhận (nếu có)
```

---

## 5. LUỒNG QUẢN LÝ THANH TOÁN

### 💳 Quy trình xử lý thanh toán
```
1. Staff mở PaymentManagementPage
2. Chọn booking cần xử lý
3. ProcessPaymentWindow:
   ├── Hiển thị thông tin booking
   ├── Nhập amount, payment method
   └── Xác nhận thanh toán
4. PaymentService.ProcessPayment()
   ├── UpdatePaymentStatus("completed")
   ├── UpdateBookingPaymentStatus("paid")
   ├── GenerateInvoice()
   └── LogActivity()
5. Refresh danh sách thanh toán
```

### 🔄 Quy trình hoàn tiền
```
1. Staff chọn payment cần hoàn tiền
2. PaymentService.ProcessRefund()
   ├── Validate payment status
   ├── Calculate refund amount
   ├── Create refund record
   ├── Update booking status
   └── Log activity
3. Gửi thông báo cho customer
```

---

## 6. LUỒNG BÁO CÁO VÀ THỐNG KÊ

### 📊 Quy trình tạo báo cáo
```
1. Admin/Staff mở ReportsPage
2. Chọn loại báo cáo:
   ├── Revenue Report
   ├── Booking Statistics
   ├── Tour Performance
   └── Customer Analytics
3. Chọn thời gian (date range)
4. GenerateReport()
   ├── Query database
   ├── Calculate statistics
   ├── Format data
   └── Display in DataGrid
5. Export to PDF/Excel (nếu cần)
```

### 📈 Dashboard tổng quan
```
1. LoadDashboard() khi mở window
2. Fetch real-time data:
   ├── TotalRevenue()
   ├── ActiveBookings()
   ├── PendingPayments()
   └── RecentActivities()
3. Update UI với live data
4. Auto-refresh mỗi 5 phút
```

---

## 7. LUỒNG QUẢN LÝ NGƯỜI DÙNG

### 👥 Quy trình tạo user mới
```
1. Admin mở UserManagementPage
2. Click "Add User" → AddUserWindow
3. Nhập thông tin user:
   ├── Username, Email, FullName
   ├── Role (Admin/Staff/Customer)
   ├── Password (tạm thời)
   └── IsActive status
4. UserService.CreateUser()
   ├── Validate unique username/email
   ├── HashPassword()
   ├── Set default values
   └── LogActivity()
5. Gửi email thông báo (nếu có)
```

### 🔄 Quy trình cập nhật profile
```
1. User mở AccountManagementPage
2. Edit thông tin cá nhân
3. UserService.UpdateUser()
   ├── Validate changes
   ├── Update database
   └── LogActivity()
4. Refresh user interface
```

---

## 8. LUỒNG XỬ LÝ LỖI VÀ LOGGING

### 🛡️ Error Handling
```
1. Try-Catch blocks trong tất cả services
2. User-friendly error messages
3. Log errors vào ActivityLog
4. Graceful degradation
5. Database connection retry logic
```

### 📝 Activity Logging
```
1. ActivityLogService.LogActivity()
   ├── UserId, Action, EntityType
   ├── EntityId, Description
   ├── Timestamp
   └── IP Address (nếu có)
2. Log tất cả operations:
   ├── Login/Logout
   ├── CRUD operations
   ├── Status changes
   └── System events
```

---

## 9. LUỒNG NAVIGATION VÀ UI

### 🧭 Navigation Flow
```
1. MainWindow (Admin/Staff/Customer)
   ├── Navigation buttons
   ├── Frame.Navigate() to Pages
   └── SetActiveButton() highlighting
2. Page Navigation:
   ├── Load data từ Services
   ├── Bind to DataContext
   ├── Handle user interactions
   └── Update UI accordingly
3. Window Management:
   ├── Modal dialogs cho forms
   ├── Non-modal cho reports
   └── Proper window lifecycle
```

### 🎨 UI Updates
```
1. Data Binding với ObservableCollection
2. INotifyPropertyChanged implementation
3. Async operations với loading indicators
4. Validation feedback
5. Responsive design với Grid/StackPanel
```

---

## 10. LUỒNG ĐĂNG XUẤT VÀ CLEANUP

### 🚪 Quy trình đăng xuất
```
1. User click Logout button
2. LogoutButton_Click()
   ├── ActivityLogService.LogActivity()
   ├── Clear user session
   ├── Close current window
   └── Show LoginWindow
3. Reset application state
4. Cleanup resources
```

### 🧹 Resource Management
```
1. Dispose database contexts
2. Close file handles
3. Clear cached data
4. Reset static variables
5. Proper window disposal
```

---

## TÓM TẮT CÁC LUỒNG CHÍNH

### 🔄 Core Workflows
1. **Authentication Flow** - Đăng nhập/Đăng xuất
2. **Tour Management Flow** - Tạo/Quản lý tour
3. **Booking Flow** - Đặt chỗ và thanh toán
4. **Payment Processing Flow** - Xử lý thanh toán
5. **Reporting Flow** - Báo cáo và thống kê
6. **User Management Flow** - Quản lý người dùng

### 🏗️ Architecture Patterns
- **MVVM Pattern** - Separation of concerns
- **Service Layer** - Business logic encapsulation
- **Repository Pattern** - Data access abstraction
- **Observer Pattern** - Event handling
- **Factory Pattern** - Object creation

### 🔒 Security Measures
- **Password Hashing** - SHA256 encryption
- **Role-based Access** - Authorization control
- **Input Validation** - Data sanitization
- **Activity Logging** - Audit trail
- **Session Management** - User context

---

*Tài liệu này mô tả các luồng xử lý chính trong hệ thống Tour Management System, giúp hiểu rõ cách hệ thống hoạt động và tương tác giữa các thành phần.* 