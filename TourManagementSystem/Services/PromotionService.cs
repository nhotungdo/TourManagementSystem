using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class PromotionService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public PromotionService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<Promotion> GetAllPromotions()
        {
            return _context.Promotions
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.TourPromotions)
                    .ThenInclude(tp => tp.Tour)
                .OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue)
                .ToList();
        }

        public List<Promotion> GetActivePromotions()
        {
            var now = DateTime.Now;
            return _context.Promotions
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.TourPromotions)
                    .ThenInclude(tp => tp.Tour)
                .Where(p => p.IsActive == true &&
                           p.StartDate <= now &&
                           p.EndDate >= now)
                .OrderBy(p => p.EndDate)
                .ToList();
        }

        public Promotion? GetPromotionById(int promotionId)
        {
            return _context.Promotions
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.TourPromotions)
                    .ThenInclude(tp => tp.Tour)
                .FirstOrDefault(p => p.PromotionId == promotionId);
        }

        public bool CreatePromotion(Promotion promotion, int createdBy)
        {
            try
            {
                promotion.CreatedBy = createdBy;
                promotion.CreatedAt = DateTime.Now;
                promotion.IsActive = true;
                _context.Promotions.Add(promotion);
                _context.SaveChanges();

                _activityLogService.LogActivity(createdBy, "CREATE", "Promotion", promotion.PromotionId,
                    $"Created promotion: {promotion.PromotionName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePromotion(Promotion promotion, int updatedBy)
        {
            try
            {
                promotion.UpdatedAt = DateTime.Now;
                _context.Promotions.Update(promotion);
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "UPDATE", "Promotion", promotion.PromotionId,
                    $"Updated promotion: {promotion.PromotionName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeletePromotion(int promotionId, int deletedBy)
        {
            try
            {
                var promotion = _context.Promotions.Find(promotionId);
                if (promotion == null) return false;

                _context.Promotions.Remove(promotion);
                _context.SaveChanges();

                _activityLogService.LogActivity(deletedBy, "DELETE", "Promotion", promotionId,
                    $"Deleted promotion: {promotion.PromotionName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TogglePromotionStatus(int promotionId, int updatedBy)
        {
            try
            {
                var promotion = _context.Promotions.Find(promotionId);
                if (promotion == null) return false;

                promotion.IsActive = !(promotion.IsActive ?? false);
                promotion.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                var action = promotion.IsActive == true ? "ACTIVATED" : "DEACTIVATED";
                _activityLogService.LogActivity(updatedBy, action, "Promotion", promotionId,
                    $"{action} promotion: {promotion.PromotionName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AssignPromotionToTour(int promotionId, int tourId, int assignedBy)
        {
            try
            {
                var existingAssignment = _context.TourPromotions
                    .FirstOrDefault(tp => tp.PromotionId == promotionId && tp.TourId == tourId);

                if (existingAssignment != null) return false; // Already assigned

                var tourPromotion = new TourPromotion
                {
                    PromotionId = promotionId,
                    TourId = tourId
                };

                _context.TourPromotions.Add(tourPromotion);
                _context.SaveChanges();

                _activityLogService.LogActivity(assignedBy, "ASSIGN", "TourPromotion", tourPromotion.TourPromotionId,
                    $"Assigned promotion {promotionId} to tour {tourId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemovePromotionFromTour(int promotionId, int tourId, int removedBy)
        {
            try
            {
                var tourPromotion = _context.TourPromotions
                    .FirstOrDefault(tp => tp.PromotionId == promotionId && tp.TourId == tourId);

                if (tourPromotion == null) return false;

                _context.TourPromotions.Remove(tourPromotion);
                _context.SaveChanges();

                _activityLogService.LogActivity(removedBy, "REMOVE", "TourPromotion", tourPromotion.TourPromotionId,
                    $"Removed promotion {promotionId} from tour {tourId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Promotion> GetPromotionsByTour(int tourId)
        {
            return _context.TourPromotions
                .Include(tp => tp.Promotion)
                    .ThenInclude(p => p.CreatedByNavigation)
                .Where(tp => tp.TourId == tourId && tp.Promotion.IsActive == true)
                .Select(tp => tp.Promotion)
                .ToList();
        }

        public List<Promotion> SearchPromotions(string searchTerm)
        {
            return _context.Promotions
                .Include(p => p.CreatedByNavigation)
                .Where(p => p.PromotionName.Contains(searchTerm) ||
                           p.Description.Contains(searchTerm) ||
                           p.PromotionCode.Contains(searchTerm))
                .OrderBy(p => p.PromotionName)
                .ToList();
        }

        public Promotion? GetPromotionByCode(string promotionCode)
        {
            var now = DateTime.Now;
            return _context.Promotions
                .FirstOrDefault(p => p.PromotionCode == promotionCode &&
                                   p.IsActive == true &&
                                   p.StartDate <= now &&
                                   p.EndDate >= now);
        }

        public bool ValidatePromotionCode(string promotionCode, int tourId)
        {
            var promotion = GetPromotionByCode(promotionCode);
            if (promotion == null) return false;

            // Check if promotion is assigned to the tour
            var tourPromotion = _context.TourPromotions
                .FirstOrDefault(tp => tp.PromotionId == promotion.PromotionId && tp.TourId == tourId);

            return tourPromotion != null;
        }
    }
}