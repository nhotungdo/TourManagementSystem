using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Scripts
{
    public static class SeedData
    {
        public static void Initialize(TourManagementContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Users.Any())
            {
                return; // Data already seeded
            }

            // Create default users
            CreateDefaultUsers(context);

            // Create sample tours
            CreateSampleTours(context);

            // Create sample attractions
            CreateSampleAttractions(context);

            // Create sample promotions
            CreateSamplePromotions(context);

            // Create sample schedules
            CreateSampleSchedules(context);

            context.SaveChanges();
        }

        private static void CreateDefaultUsers(TourManagementContext context)
        {
            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    Password = "123456",
                    Email = "admin@toursystem.com",
                    FullName = "System Administrator",
                    Phone = "0123456789",
                    Address = "123 Admin Street, City",
                    Role = "admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "staff",
                    Password = "123456" ,
                    Email = "staff@toursystem.com",
                    FullName = "Tour Staff",
                    Phone = "0987654321",
                    Address = "456 Staff Avenue, City",
                    Role = "staff",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "customer",
                    Password = "123456",
                    Email = "customer@example.com",
                    FullName = "John Customer",
                    Phone = "0555666777",
                    Address = "789 Customer Road, City",
                    Role = "customer",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
        }

        private static void CreateSampleTours(TourManagementContext context)
        {
            var adminUser = context.Users.First(u => u.Username == "admin");

            var tours = new[]
            {
                new Tour
                {
                    TourName = "Hà Nội - Sapa - Fansipan",
                    Description = "Khám phá vẻ đẹp núi rừng Tây Bắc, chinh phục đỉnh Fansipan",
                    DurationDays = 4,
                    DurationNights = 3,
                    DepartureLocation = "Hà Nội",
                    Destination = "Sapa, Lào Cai",
                    BasePrice = 2500000,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Tour
                {
                    TourName = "Hà Nội - Hạ Long - Cát Bà",
                    Description = "Du thuyền vịnh Hạ Long, khám phá đảo Cát Bà",
                    DurationDays = 3,
                    DurationNights = 2,
                    DepartureLocation = "Hà Nội",
                    Destination = "Hạ Long, Quảng Ninh",
                    BasePrice = 1800000,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Tour
                {
                    TourName = "TP.HCM - Phú Quốc",
                    Description = "Nghỉ dưỡng tại đảo ngọc Phú Quốc",
                    DurationDays = 5,
                    DurationNights = 4,
                    DepartureLocation = "TP.HCM",
                    Destination = "Phú Quốc, Kiên Giang",
                    BasePrice = 3500000,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Tour
                {
                    TourName = "Hà Nội - Ninh Bình - Tam Cốc",
                    Description = "Khám phá vùng đất cố đô, thuyền thưởng ngoạn Tam Cốc",
                    DurationDays = 2,
                    DurationNights = 1,
                    DepartureLocation = "Hà Nội",
                    Destination = "Ninh Bình",
                    BasePrice = 1200000,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            };

            context.Tours.AddRange(tours);
        }

        private static void CreateSampleAttractions(TourManagementContext context)
        {
            var attractions = new[]
            {
                new Attraction
                {
                    AttractionName = "Đỉnh Fansipan",
                    Description = "Nóc nhà Đông Dương, độ cao 3.143m"
                },
                new Attraction
                {
                    AttractionName = "Vịnh Hạ Long",
                    Description = "Di sản thiên nhiên thế giới với hàng nghìn đảo đá vôi"
                },
                new Attraction
                {
                    AttractionName = "Bãi Sao Phú Quốc",
                    Description = "Bãi biển đẹp nhất Phú Quốc với cát trắng mịn"
                },
                new Attraction
                {
                    AttractionName = "Tam Cốc - Bích Động",
                    Description = "Hang động đẹp với thuyền thưởng ngoạn"
                },
                new Attraction
                {
                    AttractionName = "Chùa Bái Đính",
                    Description = "Quần thể chùa lớn nhất Việt Nam"
                }
            };

            context.Attractions.AddRange(attractions);
        }

        private static void CreateSamplePromotions(TourManagementContext context)
        {
            var adminUser = context.Users.First(u => u.Username == "admin");

            var promotions = new[]
            {
                new Promotion
                {
                    PromotionName = "Khuyến mãi mùa hè",
                    Description = "Giảm giá cho các tour mùa hè",
                    PromotionCode = "SUMMER2024",
                    DiscountPercentage = 15,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(3),
                    MaxUsage = 100,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Promotion
                {
                    PromotionName = "Khuyến mãi sinh nhật",
                    Description = "Giảm giá đặc biệt cho khách hàng trong tháng sinh nhật",
                    PromotionCode = "BIRTHDAY",
                    DiscountPercentage = 20,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    MaxUsage = 50,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            };

            context.Promotions.AddRange(promotions);
        }

        private static void CreateSampleSchedules(TourManagementContext context)
        {
            var staffUser = context.Users.First(u => u.Username == "staff");
            var tours = context.Tours.ToList();

            var schedules = new[]
            {
                new TourSchedule
                {
                    TourId = tours[0].TourId, // Sapa tour
                    DepartureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                    ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                    MaxCapacity = 20,
                    CurrentBookings = 0,
                    Status = "scheduled",
                    GuideId = staffUser.UserId
                },
                new TourSchedule
                {
                    TourId = tours[1].TourId, // Hạ Long tour
                    DepartureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                    ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(16)),
                    MaxCapacity = 15,
                    CurrentBookings = 0,
                    Status = "scheduled",
                    GuideId = staffUser.UserId
                },
                new TourSchedule
                {
                    TourId = tours[2].TourId, // Phú Quốc tour
                    DepartureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(21)),
                    ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(25)),
                    MaxCapacity = 12,
                    CurrentBookings = 0,
                    Status = "scheduled",
                    GuideId = staffUser.UserId
                }
            };

            context.TourSchedules.AddRange(schedules);
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}