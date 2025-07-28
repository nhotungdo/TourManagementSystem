using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class TourPromotion
{
    public int TourPromotionId { get; set; }

    public int TourId { get; set; }

    public int PromotionId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? AppliedAt { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
