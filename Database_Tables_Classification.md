# TOUR MANAGEMENT SYSTEM
## Phân Loại Các Bảng Database

---

## 📊 **PHÂN LOẠI THEO CHỨC NĂNG**

---

## 1. 🏢 **BẢNG QUẢN LÝ NGƯỜI DÙNG (USER MANAGEMENT)**

### 👥 **Users** - Bảng người dùng chính
**Chức năng:** Quản lý thông tin tất cả người dùng trong hệ thống
**Mục đích:**
- Lưu trữ thông tin cá nhân (username, email, full_name, phone, address)
- Phân quyền theo role (admin, staff, customer)
- Quản lý trạng thái hoạt động (is_active)
- Theo dõi thời gian tạo và cập nhật

**Các trường quan trọng:**
- `user_id` - Khóa chính
- `role` - Phân quyền: admin/staff/customer
- `is_active` - Trạng thái hoạt động
- `created_at`, `updated_at` - Timestamp

---

## 2. 🗺️ **BẢNG QUẢN LÝ TOUR (TOUR MANAGEMENT)**

### 🎯 **Tours** - Bảng tour du lịch chính
**Chức năng:** Lưu trữ thông tin cơ bản của các tour du lịch
**Mục đích:**
- Định nghĩa tour với thông tin cơ bản
- Quản lý giá cơ bản và thông tin địa điểm
- Theo dõi người tạo tour

**Các trường quan trọng:**
- `tour_id` - Khóa chính
- `tour_name`, `description` - Thông tin tour
- `duration_days`, `duration_nights` - Thời gian tour
- `departure_location`, `destination` - Địa điểm
- `base_price` - Giá cơ bản
- `created_by` - FK đến Users (người tạo)

### 📅 **TourSchedules** - Bảng lịch trình tour
**Chức năng:** Quản lý các lịch trình cụ thể của từng tour
**Mục đích:**
- Lên lịch các chuyến đi cụ thể
- Quản lý sức chứa và số lượng đặt chỗ
- Theo dõi trạng thái lịch trình

**Các trường quan trọng:**
- `schedule_id` - Khóa chính
- `tour_id` - FK đến Tours
- `departure_date`, `return_date` - Ngày đi/ về
- `max_capacity`, `current_bookings` - Sức chứa
- `guide_id` - FK đến Users (hướng dẫn viên)
- `status` - Trạng thái: scheduled/ongoing/completed/cancelled

### 🏞️ **Attractions** - Bảng điểm tham quan
**Chức năng:** Quản lý các điểm tham quan trong tour
**Mục đích:**
- Lưu trữ thông tin điểm tham quan
- Quản lý hình ảnh và mô tả

**Các trường quan trọng:**
- `attraction_id` - Khóa chính
- `attraction_name`, `location` - Tên và địa điểm
- `description`, `image_url` - Mô tả và hình ảnh

### 🔗 **TourAttractions** - Bảng liên kết tour-điểm tham quan
**Chức năng:** Quan hệ nhiều-nhiều giữa Tour và Attraction
**Mục đích:**
- Xác định điểm tham quan nào thuộc tour nào
- Sắp xếp thứ tự tham quan
- Mô tả chi tiết cho từng điểm

**Các trường quan trọng:**
- `tour_id`, `attraction_id` - Khóa chính kép
- `visit_day` - Ngày tham quan
- `visit_order` - Thứ tự tham quan
- `description` - Mô tả chi tiết

---

## 3. 📋 **BẢNG QUẢN LÝ ĐẶT CHỖ (BOOKING MANAGEMENT)**

### 🎫 **Bookings** - Bảng đặt chỗ chính
**Chức năng:** Quản lý các đặt chỗ tour của khách hàng
**Mục đích:**
- Lưu trữ thông tin đặt chỗ
- Theo dõi trạng thái đặt chỗ và thanh toán
- Quản lý số lượng người và giá tiền

**Các trường quan trọng:**
- `booking_id` - Khóa chính
- `customer_id` - FK đến Users (khách hàng)
- `schedule_id` - FK đến TourSchedules
- `num_adults`, `num_children` - Số lượng người
- `total_price`, `final_price` - Giá tiền
- `status` - Trạng thái: pending/confirmed/cancelled/completed
- `payment_status` - Trạng thái thanh toán: unpaid/partial/paid
- `promotion_id` - FK đến Promotions (khuyến mãi)
- `discount_amount` - Số tiền giảm giá

---

## 4. 💳 **BẢNG QUẢN LÝ THANH TOÁN (PAYMENT MANAGEMENT)**

### 💰 **Payments** - Bảng thanh toán
**Chức năng:** Quản lý các giao dịch thanh toán
**Mục đích:**
- Lưu trữ thông tin thanh toán
- Theo dõi trạng thái giao dịch
- Quản lý phương thức thanh toán

**Các trường quan trọng:**
- `payment_id` - Khóa chính
- `booking_id` - FK đến Bookings
- `amount` - Số tiền thanh toán
- `payment_method` - Phương thức: cash/credit_card/bank_transfer/e_wallet
- `transaction_id` - Mã giao dịch
- `status` - Trạng thái: pending/completed/failed/refunded
- `processed_by` - FK đến Users (người xử lý)

### 🧾 **Invoices** - Bảng hóa đơn
**Chức năng:** Quản lý hóa đơn cho các đặt chỗ
**Mục đích:**
- Tạo hóa đơn cho khách hàng
- Quản lý thuế và giảm giá
- Theo dõi trạng thái hóa đơn

**Các trường quan trọng:**
- `invoice_id` - Khóa chính
- `invoice_number` - Số hóa đơn
- `booking_id` - FK đến Bookings
- `total_amount`, `tax_amount`, `discount_amount`, `final_amount`
- `status` - Trạng thái: draft/issued/paid/cancelled
- `due_date` - Ngày hạn thanh toán
- `created_by` - FK đến Users (người tạo)

---

## 5. ⭐ **BẢNG ĐÁNH GIÁ VÀ PHẢN HỒI (REVIEW & FEEDBACK)**

### 📝 **Reviews** - Bảng đánh giá
**Chức năng:** Quản lý đánh giá của khách hàng
**Mục đích:**
- Lưu trữ đánh giá và bình luận
- Theo dõi rating từ 1-5 sao
- Liên kết với tour và booking cụ thể

**Các trường quan trọng:**
- `review_id` - Khóa chính
- `tour_id` - FK đến Tours
- `customer_id` - FK đến Users (khách hàng)
- `booking_id` - FK đến Bookings
- `rating` - Đánh giá 1-5 sao
- `comment` - Bình luận chi tiết

---

## 6. 🎁 **BẢNG QUẢN LÝ KHUYẾN MÃI (PROMOTION MANAGEMENT)**

### 🏷️ **Promotions** - Bảng khuyến mãi
**Chức năng:** Quản lý các chương trình khuyến mãi
**Mục đích:**
- Tạo và quản lý khuyến mãi
- Thiết lập thời gian hiệu lực
- Quản lý mã khuyến mãi và giới hạn sử dụng

**Các trường quan trọng:**
- `promotion_id` - Khóa chính
- `promotion_name`, `description` - Tên và mô tả
- `discount_percentage` - Phần trăm giảm giá
- `start_date`, `end_date` - Thời gian hiệu lực
- `promotion_code` - Mã khuyến mãi
- `max_usage`, `current_usage` - Giới hạn sử dụng
- `min_booking_amount` - Giá trị đặt chỗ tối thiểu
- `is_active` - Trạng thái hoạt động

### 🔗 **TourPromotions** - Bảng liên kết tour-khuyến mãi
**Chức năng:** Quan hệ nhiều-nhiều giữa Tour và Promotion
**Mục đích:**
- Áp dụng khuyến mãi cho tour cụ thể
- Quản lý trạng thái áp dụng

**Các trường quan trọng:**
- `tour_promotion_id` - Khóa chính
- `tour_id` - FK đến Tours
- `promotion_id` - FK đến Promotions
- `is_active` - Trạng thái áp dụng

---

## 7. 🔔 **BẢNG THÔNG BÁO (NOTIFICATION SYSTEM)**

### 📢 **Notifications** - Bảng thông báo
**Chức năng:** Quản lý hệ thống thông báo
**Mục đích:**
- Gửi thông báo cho người dùng
- Phân loại theo loại và đối tượng nhận
- Theo dõi trạng thái đọc

**Các trường quan trọng:**
- `notification_id` - Khóa chính
- `title`, `message` - Tiêu đề và nội dung
- `type` - Loại: info/warning/error/success
- `user_id` - FK đến Users (người nhận cụ thể)
- `target_role` - Đối tượng: admin/staff/customer/all
- `is_read` - Trạng thái đã đọc
- `created_by` - FK đến Users (người tạo)

---

## 8. 📊 **BẢNG LOG VÀ THEO DÕI (LOGGING & MONITORING)**

### 📝 **ActivityLogs** - Bảng log hoạt động
**Chức năng:** Ghi lại tất cả hoạt động trong hệ thống
**Mục đích:**
- Audit trail cho bảo mật
- Theo dõi hoạt động người dùng
- Debug và troubleshooting

**Các trường quan trọng:**
- `log_id` - Khóa chính
- `user_id` - FK đến Users (người thực hiện)
- `action` - Hành động thực hiện
- `entity_type`, `entity_id` - Đối tượng tác động
- `description` - Mô tả chi tiết
- `ip_address`, `user_agent` - Thông tin client
- `log_level` - Mức độ: info/warning/error/debug

---

## 9. ⚙️ **BẢNG CẤU HÌNH HỆ THỐNG (SYSTEM CONFIGURATION)**

### 🔧 **SystemConfigs** - Bảng cấu hình hệ thống
**Chức năng:** Lưu trữ các cấu hình hệ thống
**Mục đích:**
- Quản lý thông tin công ty
- Cấu hình email, thanh toán
- Thiết lập các tham số hệ thống

**Các trường quan trọng:**
- `config_id` - Khóa chính
- `config_key`, `config_value` - Khóa và giá trị cấu hình
- `description` - Mô tả cấu hình
- `category` - Phân loại: company/email/payment/system
- `is_active` - Trạng thái hoạt động
- `updated_by` - FK đến Users (người cập nhật)

---

## 📋 **TÓM TẮT PHÂN LOẠI**

### 🎯 **Bảng Core (Cốt lõi)**
1. **Users** - Quản lý người dùng
2. **Tours** - Quản lý tour
3. **Bookings** - Quản lý đặt chỗ
4. **Payments** - Quản lý thanh toán

### 🔗 **Bảng Relationship (Liên kết)**
1. **TourSchedules** - Lịch trình tour
2. **TourAttractions** - Tour-Điểm tham quan
3. **TourPromotions** - Tour-Khuyến mãi

### 🏞️ **Bảng Master Data (Dữ liệu gốc)**
1. **Attractions** - Điểm tham quan
2. **Promotions** - Khuyến mãi

### 📄 **Bảng Transaction (Giao dịch)**
1. **Invoices** - Hóa đơn
2. **Reviews** - Đánh giá

### 🔔 **Bảng System (Hệ thống)**
1. **Notifications** - Thông báo
2. **ActivityLogs** - Log hoạt động
3. **SystemConfigs** - Cấu hình

---

## 🎯 **MỤC ĐÍCH SỬ DỤNG CHÍNH**

### 💼 **Quản lý nghiệp vụ**
- Tours, TourSchedules, Bookings, Payments, Invoices

### 👥 **Quản lý người dùng**
- Users, ActivityLogs

### 🎁 **Marketing và khuyến mãi**
- Promotions, TourPromotions, Notifications

### 📊 **Báo cáo và phân tích**
- ActivityLogs, Reviews, SystemConfigs

### 🏞️ **Nội dung và thông tin**
- Attractions, TourAttractions

---

*Tài liệu này giúp hiểu rõ chức năng và mục đích của từng bảng trong database, hỗ trợ việc thiết kế, phát triển và bảo trì hệ thống.* 