# Tour Management System - Project Report

## Report 1: Project Proposal Report (Initiation Phase)

### Project Title
**Tour Management System (TMS) – Desktop Application with WPF**

### Team Members and Roles
| No. | Student ID | Student Name | Role |
|-----|------------|--------------|------|
| 1 | HE187130 | Xuân Trường | Developer |
| 2 | HE187130 | Xuân Trường | Database Designer |
| 3 | HE187130 | Xuân Trường | UI/UX Designer |

### Objectives and Expected Outcomes

**Mục tiêu:**
Xây dựng một ứng dụng quản lý tour du lịch chạy trên nền tảng desktop (WPF) cho phép thực hiện các tác vụ như:
- Quản lý thông tin người dùng (Admin, Staff, Customer)
- Quản lý tour du lịch và lịch trình
- Quản lý đặt chỗ và thanh toán
- Quản lý điểm tham quan và khuyến mãi
- Phân quyền người dùng theo role
- Xem báo cáo và thống kê hoạt động

**Kết quả mong đợi:**
- Một ứng dụng WPF hoàn chỉnh có thể sử dụng cho công ty du lịch vừa và nhỏ
- Cơ sở dữ liệu SQL Server được thiết kế và tích hợp đầy đủ
- Hệ thống đăng nhập phân quyền, hoạt động ổn định
- Giao diện hiện đại, dễ sử dụng
- Hệ thống báo cáo và thống kê chi tiết

### Scope and Technical Requirements

**Phạm vi bao gồm:**
- Xây dựng giao diện người dùng bằng WPF
- Xử lý dữ liệu bằng C# và Entity Framework Core
- Kết nối cơ sở dữ liệu SQL Server
- Chức năng: Đăng nhập, Quản lý Tour, Đặt chỗ, Thanh toán, Báo cáo, Quản lý người dùng

**Không bao gồm:**
- Triển khai Web hoặc Mobile app
- Tích hợp thanh toán online thực tế
- Hệ thống chat hoặc email marketing
- Tích hợp với hệ thống bên ngoài

**Yêu cầu kỹ thuật:**
- Visual Studio 2022+
- .NET 8.0
- SQL Server
- Entity Framework Core 9.0.7
- Connection string: "DBDefault"

### Initial Implementation Plan

| Giai đoạn | Thời gian | Nội dung |
|-----------|-----------|----------|
| Giai đoạn 1 | Tuần 1–2 | Phân tích yêu cầu, thiết kế database |
| Giai đoạn 2 | Tuần 3–4 | Thiết kế UI/UX, tạo models và context |
| Giai đoạn 3 | Tuần 5–6 | Lập trình chức năng cốt lõi (Auth, Tour, Booking) |
| Giai đoạn 4 | Tuần 7–8 | Tích hợp, kiểm thử, tinh chỉnh UI |
| Giai đoạn 5 | Tuần 9 | Viết báo cáo và demo nghiệm thu |

### Resources and Tools
- **Ngôn ngữ:** C#
- **UI Framework:** WPF (.xaml)
- **ORM:** Entity Framework Core 9.0.7
- **CSDL:** SQL Server
- **IDE:** Visual Studio 2022
- **Quản lý mã nguồn:** Git
- **Architecture:** MVVM Pattern

### Risk Assessment

| Rủi ro | Mức độ | Biện pháp |
|--------|--------|-----------|
| Không đủ thời gian triển khai | Trung bình | Chia giai đoạn nhỏ, ưu tiên chức năng cốt lõi |
| Thiếu kinh nghiệm với EF Core | Trung bình | Tìm tài liệu, chia nhỏ chức năng |
| Xung đột phân công công việc nhóm | Thấp | Phân rõ trách nhiệm, cập nhật qua Git |
| Vấn đề kết nối database | Trung bình | Test connection sớm, có backup plan |

---

## Report 2: System Analysis & Architectural Design Report (Design Phase)

### System Overview
Hệ thống Tour Management System là một ứng dụng desktop sử dụng WPF, phục vụ quản lý tour du lịch cho công ty du lịch vừa và nhỏ. Các chức năng chính bao gồm:
- Đăng nhập với phân quyền (Admin/Staff/Customer)
- Quản lý tour du lịch và lịch trình
- Quản lý đặt chỗ và thanh toán
- Quản lý điểm tham quan và khuyến mãi
- Xem báo cáo và thống kê

**Người dùng chính:**
- **Admin:** Toàn quyền quản lý hệ thống
- **Staff:** Quản lý tour, đặt chỗ, thanh toán
- **Customer:** Xem tour, đặt chỗ, thanh toán

**Nền tảng:**
- Ứng dụng WPF chạy trên Windows
- Kết nối cơ sở dữ liệu SQL Server

### UML Diagrams

**Use Case Diagram:**
```
Customer --> Browse Tours
Customer --> Book Tour
Customer --> View Bookings
Customer --> Make Payment
Staff --> Manage Tours
Staff --> Manage Bookings
Staff --> Process Payments
Admin --> Manage Users
Admin --> View Reports
Admin --> System Configuration
```

**Class Diagram (Tóm tắt):**
- `User`: UserId, Username, Password, Email, FullName, Role
- `Tour`: TourId, TourName, Description, DurationDays, BasePrice
- `TourSchedule`: ScheduleId, TourId, DepartureDate, MaxCapacity
- `Booking`: BookingId, CustomerId, ScheduleId, TotalPrice, Status
- `Payment`: PaymentId, BookingId, Amount, PaymentMethod, Status

### Database Design

**ERD (Tóm tắt):**
```
USER ||--o{ TOUR : creates
TOUR ||--o{ TOUR_SCHEDULE : has
TOUR_SCHEDULE ||--o{ BOOKING : receives
BOOKING ||--o{ PAYMENT : generates
USER ||--o{ BOOKING : makes
TOUR ||--o{ REVIEW : receives
```

**Database Schema (rút gọn):**
- `Users`: user_id, username, password, email, full_name, role
- `Tours`: tour_id, tour_name, description, duration_days, base_price
- `TourSchedules`: schedule_id, tour_id, departure_date, max_capacity
- `Bookings`: booking_id, customer_id, schedule_id, total_price, status
- `Payments`: payment_id, booking_id, amount, payment_method, status

### User Interface (UI) Mockups
**Các màn hình chính:**
- `LoginWindow.xaml`: Đăng nhập
- `AdminMainWindow.xaml`: Giao diện Admin
- `StaffMainWindow.xaml`: Giao diện Staff
- `CustomerMainWindow.xaml`: Giao diện Customer
- `TourManagementPage.xaml`: Quản lý tour
- `BookingManagementPage.xaml`: Quản lý đặt chỗ
- `PaymentManagementPage.xaml`: Quản lý thanh toán
- `ReportsPage.xaml`: Báo cáo và thống kê

**Rationale UI:**
- Sử dụng Material Design principles
- Giao diện responsive với navigation tabs
- Color scheme nhất quán (Primary: Blue, Secondary: Orange)
- Icons trực quan cho từng chức năng

### Team Contributions
| Thành viên | Công việc phụ trách |
|------------|---------------------|
| Xuân Trường | Thiết kế UI (XAML), điều hướng |
| Xuân Trường | Kết nối DB, EF Core, xử lý logic |
| Xuân Trường | Quản lý đăng nhập, phân quyền |
| Xuân Trường | Module tour, booking, payment |

---

## Report 3: Core Feature Development Report (Development Phase)

### Development Progress Overview
Đến thời điểm hiện tại, nhóm đã hoàn thành phần lớn các chức năng cốt lõi của hệ thống, bao gồm:
- Đăng nhập và phân quyền người dùng (Admin/Staff/Customer)
- Quản lý thông tin người dùng
- Quản lý tour du lịch và lịch trình
- Quản lý đặt chỗ và thanh toán
- Quản lý điểm tham quan và khuyến mãi
- Thống kê và báo cáo chi tiết
- Giao diện người dùng đã được thiết kế bằng WPF với các cửa sổ chức năng riêng biệt

### Implemented Features

| Tính năng | Mô tả | File chính |
|-----------|-------|------------|
| Authentication | Xác thực người dùng (Admin/Staff/Customer) | `LoginWindow.xaml`, `AuthService.cs` |
| User Management | Quản lý người dùng, phân quyền | `UserManagementPage.xaml` |
| Tour Management | Thêm, sửa, xóa tour du lịch | `TourManagementPage.xaml` |
| Schedule Management | Quản lý lịch trình tour | `ScheduleManagementPage.xaml` |
| Booking Management | Quản lý đặt chỗ | `BookingManagementPage.xaml` |
| Payment Management | Xử lý thanh toán | `PaymentManagementPage.xaml` |
| Attraction Management | Quản lý điểm tham quan | `AttractionManagementPage.xaml` |
| Promotion Management | Quản lý khuyến mãi | `PromotionManagementPage.xaml` |
| Reports & Statistics | Báo cáo và thống kê | `ReportsPage.xaml` |
| Activity Log | Ghi log hoạt động | `ActivityLogPage.xaml` |

### Technical Implementation

**🔷 OOP Principles**
- Mỗi model class đại diện cho một entity trong database
- Services layer tách biệt business logic khỏi UI
- Dependency injection pattern được sử dụng

**🔷 Entity Framework Core**
- Database context: `TourManagementContext.cs`
- Connection string: "DBDefault" trong `appsettings.json`
- Code-first approach với migrations

**🔷 WPF for UI**
- MVVM pattern implementation
- Data binding với ObservableCollection
- Custom converters cho UI logic
- Responsive design với Grid và StackPanel

**🔷 Security Features**
- Password hashing với SHA256
- Role-based authorization
- Activity logging cho audit trail
- Input validation và sanitization

### Challenges and Solutions

| Thách thức | Cách khắc phục |
|------------|----------------|
| Kết nối cơ sở dữ liệu thất bại ban đầu | Sử dụng TrustServerCertificate=true, kiểm tra connection string |
| Thiết kế giao diện WPF phức tạp | Chia nhỏ thành các Page riêng biệt, sử dụng Navigation |
| Quản lý state giữa các window | Sử dụng static properties và events |
| Xử lý async operations | Implement async/await pattern cho database operations |
| Validation dữ liệu | Tạo custom validation attributes và error handling |

### Code Quality and Documentation
- **Naming convention:** PascalCase cho properties, camelCase cho variables
- **File organization:** Models, Services, Views, Converters được tách riêng
- **Comments:** XML documentation cho public methods
- **Error handling:** Try-catch blocks với meaningful error messages
- **Code reusability:** Shared services và utilities

### Testing Activities
**Kiểm thử thủ công:**
- ✅ Đăng nhập với các role khác nhau
- ✅ CRUD operations cho Tour, User, Booking
- ✅ Payment processing workflow
- ✅ Report generation
- ✅ Navigation between pages

**Chưa hoàn thành:**
- Unit tests tự động
- Integration tests
- Performance testing
- UI automation tests

### Performance Optimization
- Lazy loading cho large datasets
- Pagination cho DataGrid controls
- Caching cho frequently accessed data
- Optimized database queries với Include()

### Next Steps
1. **Tối ưu giao diện người dùng:**
   - Thêm animations và transitions
   - Implement dark mode theme
   - Responsive design improvements

2. **Bổ sung chức năng nâng cao:**
   - Export reports to PDF/Excel
   - Email notifications
   - Advanced search và filtering
   - Bulk operations

3. **Testing và Quality Assurance:**
   - Unit tests với MSTest hoặc NUnit
   - Integration tests
   - Code coverage analysis

4. **Deployment và Documentation:**
   - Setup installer package
   - User manual và admin guide
   - API documentation (nếu có)

### Git Commit History
- **Initial commit:** Project structure setup
- **Database models:** Entity Framework models và context
- **Authentication system:** Login và authorization
- **Core features:** Tour, Booking, Payment management
- **UI improvements:** WPF interface enhancements
- **Bug fixes:** Various bug fixes và optimizations

### Technical Debt và Improvements
- **Refactor AuthService:** Implement interface cho testability
- **Add logging framework:** Replace console logging với proper logging
- **Implement caching:** Add Redis hoặc in-memory caching
- **Code documentation:** Add more comprehensive XML comments
- **Error handling:** Implement global exception handling

---

## Kết luận

Dự án Tour Management System đã được phát triển thành công với đầy đủ các chức năng cốt lõi theo yêu cầu ban đầu. Hệ thống sử dụng công nghệ hiện đại (.NET 8, WPF, Entity Framework Core) và tuân thủ các nguyên tắc thiết kế tốt. Giao diện người dùng thân thiện và dễ sử dụng, phù hợp cho các công ty du lịch vừa và nhỏ.

**Điểm mạnh:**
- Kiến trúc rõ ràng, dễ bảo trì
- Giao diện người dùng hiện đại
- Bảo mật tốt với password hashing và role-based access
- Database design chuẩn với proper relationships

**Cải tiến trong tương lai:**
- Thêm unit tests và integration tests
- Implement real-time notifications
- Add mobile app companion
- Integrate với third-party payment gateways
- Implement advanced analytics và reporting

Dự án đã sẵn sàng cho deployment và sử dụng trong môi trường production với các cải tiến nhỏ về performance và testing. 