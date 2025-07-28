# Test Login Functionality

## 🧪 Test Cases cho LoginWindow.xaml

### Test Case 1: Admin Login
**Input:**
- Username: `admin`
- Password: `123456`

**Expected Output:**
- ✅ Đăng nhập thành công
- ✅ Chuyển đến `AdminMainWindow`
- ✅ Hiển thị "Welcome, Admin System"
- ✅ Có menu: Dashboard, User Management, Tour Management, etc.

### Test Case 2: Staff Login
**Input:**
- Username: `staff1`
- Password: `123456`

**Expected Output:**
- ✅ Đăng nhập thành công
- ✅ Chuyển đến `StaffMainWindow`
- ✅ Hiển thị "Welcome, Nguyễn Văn A"
- ✅ Có menu: Dashboard, Tour Management, Booking Management, etc.

### Test Case 3: Customer Login
**Input:**
- Username: `customer1`
- Password: `123456`

**Expected Output:**
- ✅ Đăng nhập thành công
- ✅ Chuyển đến `CustomerMainWindow`
- ✅ Hiển thị "Welcome, Trần Thị B"
- ✅ Có menu: Dashboard, Browse Tours, My Bookings, etc.

### Test Case 4: Invalid Credentials
**Input:**
- Username: `invalid`
- Password: `wrong`

**Expected Output:**
- ❌ Đăng nhập thất bại
- ❌ Hiển thị message "Invalid username or password"
- ❌ Ở lại LoginWindow

### Test Case 5: Empty Fields
**Input:**
- Username: `""`
- Password: `""`

**Expected Output:**
- ❌ Button "Sign In" bị disable
- ❌ Không thể submit form

## 🔍 Debug Information

### Log Files
Khi đăng nhập thành công, hệ thống sẽ log:
```
User 'admin' logged in successfully
User 'staff1' logged in successfully
User 'customer1' logged in successfully
```

### Database Queries
Hệ thống sẽ thực hiện query:
```sql
SELECT * FROM Users 
WHERE Username = 'admin' 
AND Password = '123456' 
AND IsActive = 1
```

### Navigation Flow
1. `LoginWindow` → User enters credentials
2. `LoginViewModel` → Validates credentials
3. `AuthService` → Checks database
4. `LoginWindow.OnLoginSuccessful` → Navigates based on role
5. `AdminMainWindow/StaffMainWindow/CustomerMainWindow` → Shows appropriate interface

## 🐛 Common Issues & Solutions

### Issue 1: "Invalid username or password"
**Cause:** Database connection issue or wrong credentials
**Solution:** 
- Check database connection string
- Verify database exists with Demo6-TourManagement.sql
- Check if user exists in database

### Issue 2: "Unknown user role"
**Cause:** Role field in database is null or invalid
**Solution:**
- Check Users table in database
- Ensure role is one of: 'admin', 'staff', 'customer'

### Issue 3: Navigation not working
**Cause:** MainWindow classes not found
**Solution:**
- Check if AdminMainWindow, StaffMainWindow, CustomerMainWindow exist
- Ensure all XAML files are properly compiled

### Issue 4: XAML errors
**Cause:** Missing converters or resources
**Solution:**
- Check if InverseBooleanConverter exists
- Ensure all XAML syntax is correct

## 📋 Test Checklist

- [ ] Database connection working
- [ ] Users table has test data
- [ ] LoginWindow loads without errors
- [ ] Username field accepts input
- [ ] Password field accepts input
- [ ] Sign In button enables/disables correctly
- [ ] Admin login navigates to AdminMainWindow
- [ ] Staff login navigates to StaffMainWindow
- [ ] Customer login navigates to CustomerMainWindow
- [ ] Invalid credentials show error message
- [ ] Activity logs are created for successful logins
- [ ] Logout functionality works
- [ ] Forgot password link works

## 🚀 Quick Test Commands

```bash
# Build project
dotnet build

# Run application
dotnet run --project TourManagementSystem

# Clean and rebuild if needed
dotnet clean
dotnet build
``` 