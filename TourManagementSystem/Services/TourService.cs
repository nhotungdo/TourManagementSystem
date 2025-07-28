using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class TourService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public TourService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<Tour> GetAllTours()
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.TourAttractions)
                    .ThenInclude(ta => ta.Attraction)
                .Include(t => t.TourPromotions)
                    .ThenInclude(tp => tp.Promotion)
                .Include(t => t.TourSchedules)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();
        }

        public List<Tour> GetActiveTours()
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.TourAttractions)
                    .ThenInclude(ta => ta.Attraction)
                .Include(t => t.TourPromotions)
                    .ThenInclude(tp => tp.Promotion)
                .Include(t => t.TourSchedules.Where(ts => ts.Status == "scheduled"))
                .Where(t => t.IsActive == true)
                .OrderBy(t => t.TourName)
                .ToList();
        }

        public Tour? GetTourById(int tourId)
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.TourAttractions)
                    .ThenInclude(ta => ta.Attraction)
                .Include(t => t.TourPromotions)
                    .ThenInclude(tp => tp.Promotion)
                .Include(t => t.TourSchedules)
                .Include(t => t.Reviews)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefault(t => t.TourId == tourId);
        }

        public bool CreateTour(Tour tour, int createdBy)
        {
            try
            {
                tour.CreatedBy = createdBy;
                tour.CreatedAt = DateTime.Now;
                tour.IsActive = true;
                _context.Tours.Add(tour);
                _context.SaveChanges();

                _activityLogService.LogActivity(createdBy, "CREATE", "Tour", tour.TourId,
                    $"Created tour: {tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateTour(Tour tour, int updatedBy)
        {
            try
            {
                tour.UpdatedAt = DateTime.Now;
                _context.Tours.Update(tour);
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "UPDATE", "Tour", tour.TourId,
                    $"Updated tour: {tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTour(int tourId, int deletedBy)
        {
            try
            {
                var tour = _context.Tours.Find(tourId);
                if (tour == null) return false;

                _context.Tours.Remove(tour);
                _context.SaveChanges();

                _activityLogService.LogActivity(deletedBy, "DELETE", "Tour", tourId,
                    $"Deleted tour: {tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ToggleTourStatus(int tourId, int updatedBy)
        {
            try
            {
                var tour = _context.Tours.Find(tourId);
                if (tour == null) return false;

                tour.IsActive = !tour.IsActive;
                tour.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                var action = tour.IsActive == true ? "ACTIVATED" : "DEACTIVATED";
                _activityLogService.LogActivity(updatedBy, action, "Tour", tourId,
                    $"{action} tour: {tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Tour> SearchTours(string searchTerm)
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Where(t => t.TourName.Contains(searchTerm) ||
                           t.Description.Contains(searchTerm) ||
                           t.Destination.Contains(searchTerm) ||
                           t.DepartureLocation.Contains(searchTerm))
                .OrderBy(t => t.TourName)
                .ToList();
        }

        public List<Tour> GetToursByDestination(string destination)
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Where(t => t.Destination.Contains(destination) && t.IsActive == true)
                .OrderBy(t => t.TourName)
                .ToList();
        }

        public List<Tour> GetToursByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Where(t => t.BasePrice >= minPrice && t.BasePrice <= maxPrice && t.IsActive == true)
                .OrderBy(t => t.BasePrice)
                .ToList();
        }

        public List<Tour> GetToursByDuration(int minDays, int maxDays)
        {
            return _context.Tours
                .Include(t => t.CreatedByNavigation)
                .Where(t => t.DurationDays >= minDays && t.DurationDays <= maxDays && t.IsActive == true)
                .OrderBy(t => t.DurationDays)
                .ToList();
        }
    }
}