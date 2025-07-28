# TOUR MANAGEMENT SYSTEM
## So Sánh Admin vs Staff

---

## 🎯 **TỔNG QUAN VỀ PHÂN QUYỀN**

### 👑 **Admin (Quản trị viên)**
- **Quyền cao nhất** trong hệ thống
- **Toàn quyền** quản lý và kiểm soát
- **Quản lý người dùng** và phân quyền
- **Cấu hình hệ thống** toàn cục

### 👨‍💼 **Staff (Nhân viên)**
- **Quyền trung bình** trong hệ thống
- **Quản lý nghiệp vụ** hàng ngày
- **Không thể** quản lý người dùng
- **Không thể** cấu hình hệ thống

---

## 📋 **SO SÁNH CHI TIẾT THEO CHỨC NĂNG**

---

## 1. 🗺️ **QUẢN LÝ TOUR**

### ✅ **Cả Admin và Staff đều có thể:**
- Tạo, sửa, xóa tour du lịch
- Quản lý lịch trình tour
- Quản lý điểm tham quan
- Thiết lập giá tour

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Tạo tour mới | ✅ | ✅ |
| Sửa tour | ✅ | ✅ |
| Xóa tour | ✅ | ✅ |
| Xem tất cả tour | ✅ | ✅ |
| Quản lý lịch trình | ✅ | ✅ |

---

## 2. 📋 **QUẢN LÝ ĐẶT CHỖ**

### ✅ **Cả Admin và Staff đều có thể:**
- Xem danh sách đặt chỗ
- Xử lý đặt chỗ mới
- Cập nhật trạng thái đặt chỗ
- Quản lý thanh toán

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Xem tất cả đặt chỗ | ✅ | ✅ |
| Tạo đặt chỗ mới | ✅ | ✅ |
| Cập nhật trạng thái | ✅ | ✅ |
| Xử lý thanh toán | ✅ | ✅ |
| Hủy đặt chỗ | ✅ | ✅ |

---

## 3. 💳 **QUẢN LÝ THANH TOÁN**

### ✅ **Cả Admin và Staff đều có thể:**
- Xử lý thanh toán
- Tạo hóa đơn
- Quản lý giao dịch
- Xử lý hoàn tiền

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Xử lý thanh toán | ✅ | ✅ |
| Tạo hóa đơn | ✅ | ✅ |
| Xem báo cáo thanh toán | ✅ | ✅ |
| Xử lý hoàn tiền | ✅ | ✅ |

---

## 4. 👥 **QUẢN LÝ NGƯỜI DÙNG**

### ❌ **Staff KHÔNG THỂ:**
- Quản lý tài khoản người dùng
- Tạo tài khoản mới
- Phân quyền người dùng
- Xóa tài khoản

### ✅ **Chỉ Admin có thể:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Xem danh sách người dùng | ✅ | ❌ |
| Tạo tài khoản mới | ✅ | ❌ |
| Sửa thông tin người dùng | ✅ | ❌ |
| Phân quyền (role) | ✅ | ❌ |
| Kích hoạt/Vô hiệu hóa tài khoản | ✅ | ❌ |
| Xóa tài khoản | ✅ | ❌ |
| Reset mật khẩu | ✅ | ❌ |

---

## 5. ⚙️ **CẤU HÌNH HỆ THỐNG**

### ❌ **Staff KHÔNG THỂ:**
- Thay đổi cấu hình hệ thống
- Quản lý thông tin công ty
- Thiết lập tham số hệ thống

### ✅ **Chỉ Admin có thể:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Cấu hình thông tin công ty | ✅ | ❌ |
| Thiết lập email server | ✅ | ❌ |
| Cấu hình thanh toán | ✅ | ❌ |
| Thiết lập tham số hệ thống | ✅ | ❌ |
| Quản lý cấu hình chung | ✅ | ❌ |

---

## 6. 📊 **BÁO CÁO VÀ THỐNG KÊ**

### ✅ **Cả Admin và Staff đều có thể:**
- Xem báo cáo doanh thu
- Thống kê đặt chỗ
- Báo cáo tour performance
- Export dữ liệu

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Báo cáo tổng quan | ✅ | ✅ |
| Thống kê doanh thu | ✅ | ✅ |
| Báo cáo người dùng | ✅ | ❌ |
| Báo cáo hệ thống | ✅ | ❌ |
| Export to PDF/Excel | ✅ | ✅ |

---

## 7. 🔔 **THÔNG BÁO VÀ LOG**

### ✅ **Cả Admin và Staff đều có thể:**
- Xem thông báo
- Xem log hoạt động cá nhân

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Xem thông báo | ✅ | ✅ |
| Tạo thông báo mới | ✅ | ❌ |
| Xem log hoạt động | ✅ | ✅ |
| Xem log toàn hệ thống | ✅ | ❌ |
| Xem log người dùng khác | ✅ | ❌ |

---

## 8. 🎁 **QUẢN LÝ KHUYẾN MÃI**

### ✅ **Cả Admin và Staff đều có thể:**
- Xem danh sách khuyến mãi
- Áp dụng khuyến mãi cho tour

### 🔍 **Khác biệt:**
| Chức năng | Admin | Staff |
|-----------|-------|-------|
| Tạo khuyến mãi mới | ✅ | ❌ |
| Sửa khuyến mãi | ✅ | ❌ |
| Xóa khuyến mãi | ✅ | ❌ |
| Áp dụng khuyến mãi | ✅ | ✅ |
| Xem báo cáo khuyến mãi | ✅ | ✅ |

---

## 🎯 **TÓM TẮT QUYỀN HẠN**

### 👑 **ADMIN - Toàn quyền**
```
✅ Quản lý tour và lịch trình
✅ Quản lý đặt chỗ và thanh toán
✅ Quản lý người dùng và phân quyền
✅ Cấu hình hệ thống toàn cục
✅ Xem tất cả báo cáo và log
✅ Quản lý khuyến mãi
✅ Tạo thông báo hệ thống
```

### 👨‍💼 **STAFF - Quyền nghiệp vụ**
```
✅ Quản lý tour và lịch trình
✅ Quản lý đặt chỗ và thanh toán
❌ Quản lý người dùng
❌ Cấu hình hệ thống
✅ Xem báo cáo nghiệp vụ
❌ Quản lý khuyến mãi
❌ Tạo thông báo hệ thống
```

---

## 🔐 **PHÂN QUYỀN TRONG CODE**

### 📝 **AuthService.cs**
```csharp
public bool IsAdmin(int userId)
{
    var user = _context.Users.Find(userId);
    return user?.Role?.ToLower() == "admin";
}

public bool IsStaff(int userId)
{
    var user = _context.Users.Find(userId);
    return user?.Role?.ToLower() == "staff" || user?.Role?.ToLower() == "admin";
}
```

### 🎯 **Logic phân quyền:**
- **Admin** có tất cả quyền của Staff
- **Staff** chỉ có quyền nghiệp vụ cơ bản
- **Customer** chỉ có quyền đặt chỗ và xem tour

---

## 🎨 **GIAO DIỆN KHÁC BIỆT**

### 👑 **AdminMainWindow**
- **Menu đầy đủ** với tất cả chức năng
- **User Management** button
- **System Config** button
- **Activity Logs** button

### 👨‍💼 **StaffMainWindow**
- **Menu giới hạn** chỉ chức năng nghiệp vụ
- **Không có** User Management
- **Không có** System Config
- **Không có** Activity Logs toàn hệ thống

---

## 💡 **KHUYẾN NGHỊ SỬ DỤNG**

### 👑 **Khi nào dùng Admin:**
- **Quản lý hệ thống** toàn cục
- **Tạo tài khoản** cho nhân viên mới
- **Cấu hình** thông số hệ thống
- **Xem báo cáo** tổng quan
- **Xử lý sự cố** hệ thống

### 👨‍💼 **Khi nào dùng Staff:**
- **Xử lý đặt chỗ** hàng ngày
- **Quản lý tour** và lịch trình
- **Xử lý thanh toán** cho khách
- **Báo cáo** nghiệp vụ
- **Hỗ trợ khách hàng**

---

*Tài liệu này giúp hiểu rõ sự khác biệt giữa Admin và Staff, hỗ trợ việc phân quyền và sử dụng hệ thống hiệu quả.* 