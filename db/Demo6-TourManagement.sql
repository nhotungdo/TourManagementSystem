CREATE TABLE Users (
    user_id INT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    address NVARCHAR(255),
    role VARCHAR(20) NOT NULL CHECK (role IN ('admin', 'staff', 'customer')),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    is_active BIT DEFAULT 1
);

CREATE TABLE Tours (
    tour_id INT PRIMARY KEY,
    tour_name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    duration_days INT NOT NULL,
    duration_nights INT NOT NULL,
    departure_location NVARCHAR(100) NOT NULL,
    destination NVARCHAR(100) NOT NULL,
    base_price DECIMAL(12,2) NOT NULL,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    is_active BIT DEFAULT 1,
    FOREIGN KEY (created_by) REFERENCES Users(user_id)
);

CREATE TABLE TourSchedules (
    schedule_id INT PRIMARY KEY,
    tour_id INT NOT NULL,
    departure_date DATE NOT NULL,
    return_date DATE NOT NULL,
    max_capacity INT NOT NULL,
    current_bookings INT DEFAULT 0,
    price_adjustment DECIMAL(12,2) DEFAULT 0.00,
    guide_id INT,
    status VARCHAR(20) DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'ongoing', 'completed', 'cancelled')),
    FOREIGN KEY (tour_id) REFERENCES Tours(tour_id),
    FOREIGN KEY (guide_id) REFERENCES Users(user_id),
    CHECK (return_date > departure_date),
    CHECK (current_bookings <= max_capacity)
);

CREATE TABLE Bookings (
    booking_id INT PRIMARY KEY,
    customer_id INT NOT NULL,
    schedule_id INT NOT NULL,
    booking_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    num_adults INT NOT NULL,
    num_children INT DEFAULT 0,
    total_price DECIMAL(12,2) NOT NULL,
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'cancelled', 'completed')),
    payment_status VARCHAR(20) DEFAULT 'unpaid' CHECK (payment_status IN ('unpaid', 'partial', 'paid')),
    notes NVARCHAR(MAX),
    FOREIGN KEY (customer_id) REFERENCES Users(user_id),
    FOREIGN KEY (schedule_id) REFERENCES TourSchedules(schedule_id)
);

CREATE TABLE Payments (
    payment_id INT PRIMARY KEY,
    booking_id INT NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    payment_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    payment_method VARCHAR(20) NOT NULL CHECK (payment_method IN ('cash', 'credit_card', 'bank_transfer', 'e_wallet')),
    transaction_id VARCHAR(100),
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'completed', 'failed', 'refunded')),
    processed_by INT,
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id),
    FOREIGN KEY (processed_by) REFERENCES Users(user_id)
);

CREATE TABLE Reviews (
    review_id INT PRIMARY KEY,
    tour_id INT NOT NULL,
    customer_id INT NOT NULL,
    booking_id INT NOT NULL,
    rating TINYINT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(MAX),
    review_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (tour_id) REFERENCES Tours(tour_id),
    FOREIGN KEY (customer_id) REFERENCES Users(user_id),
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id)
);

CREATE TABLE Attractions (
    attraction_id INT PRIMARY KEY,
    attraction_name NVARCHAR(255) NOT NULL,
    location NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    image_url VARCHAR(255)
);

CREATE TABLE TourAttractions (
    tour_id INT NOT NULL,
    attraction_id INT NOT NULL,
    visit_day INT NOT NULL,
    visit_order INT NOT NULL,
    description NVARCHAR(MAX),
    PRIMARY KEY (tour_id, attraction_id, visit_day),
    FOREIGN KEY (tour_id) REFERENCES Tours(tour_id),
    FOREIGN KEY (attraction_id) REFERENCES Attractions(attraction_id)
);

CREATE TABLE Invoices (
    invoice_id INT PRIMARY KEY,
    invoice_number VARCHAR(20) UNIQUE NOT NULL,
    booking_id INT NOT NULL,
    issue_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    due_date DATETIME NOT NULL,
    total_amount DECIMAL(12,2) NOT NULL,
    tax_amount DECIMAL(12,2) DEFAULT 0.00,
    discount_amount DECIMAL(12,2) DEFAULT 0.00,
    final_amount DECIMAL(12,2) NOT NULL,
    status VARCHAR(20) DEFAULT 'draft' CHECK (status IN ('draft', 'issued', 'paid', 'cancelled')),
    payment_terms NVARCHAR(MAX),
    notes NVARCHAR(MAX),
    created_by INT NOT NULL,
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id),
    FOREIGN KEY (created_by) REFERENCES Users(user_id)
);

-- Insert Users
INSERT INTO Users (user_id, username, password, email, full_name, phone, address, role, created_at, updated_at, is_active)
VALUES 
(1, 'admin', '123456', 'admin@travel.com', N'Admin System', '0901234567', N'12 Lý Thường Kiệt, Quận Hoàn Kiếm, Hà Nội', 'admin', GETDATE(), GETDATE(), 1),
(2, 'staff1', '123456', 'staff1@travel.com', N'Nguyễn Văn A', '0912345678', N'45 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh', 'staff', GETDATE(), GETDATE(), 1),
(3, 'customer1', '123456', 'customer1@gmail.com', N'Trần Thị B', '0987654321', N'78 Phan Chu Trinh, Quận Hải Châu, Đà Nẵng', 'customer', GETDATE(), GETDATE(), 1),
(4, 'customer2', '123456', 'customer2@gmail.com', N'Lê Văn C', '0978123456', N'32 Trần Phú, TP. Nha Trang, Khánh Hòa', 'customer', GETDATE(), GETDATE(), 1),
(5, 'guide1', '123456', 'guide1@travel.com', N'Phạm Thị D', '0965432187', N'65 Lê Lợi, TP. Huế, Thừa Thiên Huế', 'staff', GETDATE(), GETDATE(), 1);

-- Insert Tours
INSERT INTO Tours (tour_id, tour_name, description, duration_days, duration_nights, departure_location, destination, base_price, created_by, created_at, updated_at, is_active)
VALUES
(1, N'Đà Lạt Mộng Mơ', N'Tour khám phá Đà Lạt 3 ngày 2 đêm', 3, 2, N'TP.HCM', N'Đà Lạt', 2500000, 1, GETDATE(), GETDATE(), 1),
(2, N'Nha Trang Biển Xanh', N'Tour nghỉ dưỡng Nha Trang 4 ngày 3 đêm', 4, 3, N'Hà Nội', N'Nha Trang', 3500000, 1, GETDATE(), GETDATE(), 1),
(3, N'Miền Tây Sông Nước', N'Tour trải nghiệm miền Tây 2 ngày 1 đêm', 2, 1, N'TP.HCM', N'Cần Thơ', 1500000, 2, GETDATE(), GETDATE(), 1);

-- Insert Attractions
INSERT INTO Attractions (attraction_id, attraction_name, location, description, image_url)
VALUES
(1, N'Thung lũng Tình Yêu', N'Đà Lạt', N'Địa điểm du lịch nổi tiếng với phong cảnh lãng mạn', 'https://www.vietnamairlines.com/~/media/SEO-images/2025%20SEO/Traffic%20TV/thung-lung-tinh-yeu/thumb-thung-lung-tinh-yeu.jpg'),
(2, N'Hồ Xuân Hương', N'Đà Lạt', N'Hồ nước đẹp ở trung tâm thành phố', 'https://khamphadalat.vn/wp-content/uploads/2020/02/hxh-scaled.jpg'),
(3, N'Vinpearl Land', N'Nha Trang', N'Khu vui chơi giải trí trên đảo', 'https://res.klook.com/image/upload/q_85/c_fill,w_750/v1614915280/blog/elmonpuvzvrpmagnu9kv.jpg');

-- Insert Tour Attractions
INSERT INTO TourAttractions (tour_id, attraction_id, visit_day, visit_order, description)
VALUES
(1, 1, 1, 1, N'Tham quan buổi sáng'),
(1, 2, 1, 2, N'Tham quan buổi chiều'),
(2, 3, 2, 1, N'Tham quan Vinpearl Land');

-- Insert Tour Schedules
INSERT INTO TourSchedules (schedule_id, tour_id, departure_date, return_date, max_capacity, guide_id, status)
VALUES
(1, 1, '2024-12-15', '2024-12-17', 20, 5, 'scheduled'),
(2, 1, '2024-12-22', '2024-12-24', 20, 5, 'scheduled'),
(3, 2, '2024-12-20', '2024-12-23', 15, NULL, 'scheduled'),
(4, 3, '2024-12-10', '2024-12-11', 30, NULL, 'scheduled');

-- Insert Bookings
INSERT INTO Bookings (booking_id, customer_id, schedule_id, num_adults, num_children, total_price, status, payment_status, booking_date)
VALUES
(1, 3, 1, 2, 1, 7500000, 'confirmed', 'paid', GETDATE()),
(2, 4, 2, 1, 0, 2500000, 'pending', 'unpaid', GETDATE()),
(3, 3, 3, 2, 2, 10500000, 'confirmed', 'partial', GETDATE());

-- Insert Invoices
INSERT INTO Invoices (invoice_id, invoice_number, booking_id, due_date, total_amount, final_amount, status, created_by, issue_date)
VALUES
(1, 'INV-2024-001', 1, DATEADD(day, 7, GETDATE()), 7500000, 7500000, 'paid', 2, GETDATE()),
(2, 'INV-2024-002', 2, DATEADD(day, 7, GETDATE()), 2500000, 2500000, 'draft', 2, GETDATE()),
(3, 'INV-2024-003', 3, DATEADD(day, 7, GETDATE()), 10500000, 10500000, 'issued', 2, GETDATE());

-- Insert Payments
INSERT INTO Payments (payment_id, booking_id, amount, payment_method, transaction_id, status, processed_by, payment_date)
VALUES
(1, 1, 7500000, 'bank_transfer', 'TRX123456', 'completed', 2, GETDATE()),
(2, 3, 5000000, 'credit_card', 'TRX789012', 'completed', 2, GETDATE());

-- Insert Reviews
INSERT INTO Reviews (review_id, tour_id, customer_id, booking_id, rating, comment, review_date)
VALUES
(1, 1, 3, 1, 5, N'Tour tuyệt vời, hướng dẫn viên nhiệt tình', GETDATE()),
(2, 3, 3, 3, 4, N'Trải nghiệm thú vị, đồ ăn ngon', GETDATE());





-- Promotions table
CREATE TABLE Promotions (
    promotion_id INT IDENTITY(1,1) PRIMARY KEY,
    promotion_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(500) NULL,
    discount_percentage DECIMAL(5,2) NOT NULL,
    discount_amount DECIMAL(12,2) NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
    promotion_code NVARCHAR(20) NOT NULL UNIQUE,
    max_usage INT NULL,
    current_usage INT DEFAULT 0,
    min_booking_amount DECIMAL(12,2) NULL,
    is_active BIT DEFAULT 1,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (created_by) REFERENCES Users(user_id)
);

-- Tour Promotions (many-to-many relationship)
CREATE TABLE TourPromotions (
    tour_promotion_id INT IDENTITY(1,1) PRIMARY KEY,
    tour_id INT NOT NULL,
    promotion_id INT NOT NULL,
    is_active BIT DEFAULT 1,
    applied_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (tour_id) REFERENCES Tours(tour_id),
    FOREIGN KEY (promotion_id) REFERENCES Promotions(promotion_id)
);

-- Notifications table
CREATE TABLE Notifications (
    notification_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(200) NOT NULL,
    message NVARCHAR(1000) NOT NULL,
    type NVARCHAR(20) NOT NULL CHECK (type IN ('info', 'warning', 'error', 'success')),
    user_id INT NULL,
    target_role NVARCHAR(20) NOT NULL CHECK (target_role IN ('admin', 'staff', 'customer', 'all')),
    is_read BIT DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    read_at DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (created_by) REFERENCES Users(user_id)
);

-- Activity Logs table
CREATE TABLE ActivityLogs (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    action NVARCHAR(100) NOT NULL,
    entity_type NVARCHAR(50) NOT NULL,
    entity_id INT NULL,
    description NVARCHAR(500) NULL,
    ip_address NVARCHAR(45) NULL,
    user_agent NVARCHAR(500) NULL,
    log_level NVARCHAR(20) NOT NULL CHECK (log_level IN ('info', 'warning', 'error', 'debug')),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

-- System Configs table
CREATE TABLE SystemConfigs (
    config_id INT IDENTITY(1,1) PRIMARY KEY,
    config_key NVARCHAR(100) NOT NULL UNIQUE,
    config_value NVARCHAR(1000) NOT NULL,
    description NVARCHAR(200) NULL,
    category NVARCHAR(50) NOT NULL CHECK (category IN ('company', 'email', 'payment', 'system')),
    is_active BIT DEFAULT 1,
    updated_by INT NULL,
    updated_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (updated_by) REFERENCES Users(user_id)
);

-- Add new columns to existing tables

-- Add promotion fields to Bookings table
ALTER TABLE Bookings ADD promotion_id INT NULL;
ALTER TABLE Bookings ADD discount_amount DECIMAL(12,2) DEFAULT 0;
ALTER TABLE Bookings ADD final_price DECIMAL(12,2) DEFAULT 0;
ALTER TABLE Bookings ADD CONSTRAINT FK_Bookings_Promotions 
    FOREIGN KEY (promotion_id) REFERENCES Promotions(promotion_id);

-- Insert sample data for new features

-- Sample Promotions
INSERT INTO Promotions (promotion_name, description, discount_percentage, start_date, end_date, promotion_code, max_usage, min_booking_amount, created_by)
VALUES 
(N'Khuyến mãi mùa hè', N'Giảm giá đặc biệt cho các tour mùa hè', 15.00, GETDATE(), DATEADD(month, 3, GETDATE()), 'SUMMER2024', 50, 1000000, 1),
(N'Khuyến mãi sinh nhật', N'Giảm giá sinh nhật cho khách hàng', 20.00, GETDATE(), DATEADD(year, 1, GETDATE()), 'BIRTHDAY', 100, 500000, 1),
(N'Khuyến mãi nhóm', N'Giảm giá khi đặt tour theo nhóm từ 5 người trở lên', 10.00, GETDATE(), DATEADD(month, 6, GETDATE()), 'GROUP2024', 30, 2000000, 1);

-- Sample System Configs
INSERT INTO SystemConfigs (config_key, config_value, description, category, updated_by)
VALUES 
('company.name', N'Tour Management System', N'Tên công ty', 'company', 1),
('company.address', N'123 Đường ABC, Quận 1, TP.HCM', N'Địa chỉ công ty', 'company', 1),
('company.phone', '0901234567', N'Số điện thoại công ty', 'company', 1),
('company.email', 'info@tourmanagement.com', N'Email công ty', 'company', 1),
('email.smtp.server', 'smtp.gmail.com', N'SMTP Server', 'email', 1),
('email.smtp.port', '587', N'SMTP Port', 'email', 1),
('payment.gateway', 'manual', N'Cổng thanh toán', 'payment', 1),
('system.maxbookings', '10', N'Số lượng đặt tour tối đa mỗi khách hàng', 'system', 1);

-- Sample Notifications
INSERT INTO Notifications (title, message, type, target_role, created_by)
VALUES 
(N'Chào mừng đến với hệ thống', N'Chào mừng bạn đến với Tour Management System!', 'info', 'all', 1),
(N'Khuyến mãi mới', N'Khuyến mãi mùa hè với giảm giá 15% đã được áp dụng.', 'info', 'customer', 1),
(N'Bảo trì hệ thống', N'Hệ thống sẽ bảo trì vào ngày mai từ 2:00 - 4:00 sáng.', 'warning', 'all', 1);

-- Sample Activity Logs
INSERT INTO ActivityLogs (user_id, action, entity_type, entity_id, description, log_level)
VALUES 
(1, 'LOGIN', 'User', 1, 'Admin logged in', 'info'),
(2, 'CREATE', 'Tour', 1, 'Staff created new tour', 'info'),
(3, 'BOOKING', 'Booking', 1, 'Customer created booking', 'info');

-- Apply some promotions to tours
INSERT INTO TourPromotions (tour_id, promotion_id)
VALUES 
(1, 1), -- Apply summer promotion to Da Lat tour
(2, 1), -- Apply summer promotion to Nha Trang tour
(3, 3); -- Apply group promotion to Mekong tour





-- Create indexes for better performance
CREATE INDEX IX_Promotions_Code ON Promotions(promotion_code);
CREATE INDEX IX_Promotions_Active ON Promotions(is_active, start_date, end_date);
CREATE INDEX IX_Notifications_User ON Notifications(user_id, is_read);
CREATE INDEX IX_Notifications_Role ON Notifications(target_role, is_active);
CREATE INDEX IX_ActivityLogs_User ON ActivityLogs(user_id, created_at);
CREATE INDEX IX_ActivityLogs_Entity ON ActivityLogs(entity_type, entity_id);
CREATE INDEX IX_SystemConfigs_Key ON SystemConfigs(config_key, is_active);

-- Add constraints
ALTER TABLE Promotions ADD CONSTRAINT CK_Promotions_Dates CHECK (end_date > start_date);
ALTER TABLE Promotions ADD CONSTRAINT CK_Promotions_Usage CHECK (current_usage <= ISNULL(max_usage, current_usage));
ALTER TABLE Promotions ADD CONSTRAINT CK_Promotions_Discount CHECK (discount_percentage >= 0 AND discount_percentage <= 100);

