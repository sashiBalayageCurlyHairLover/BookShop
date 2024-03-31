using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookShop.Infrastructure.Data.DataConstants;

namespace BookShop.Infrastructure.Data.Models
{
    public class Pen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(PenColorMaxLength)]
        public string PenColor { get; set; } = string.Empty;

        [Required]
        [StringLength(PenManufacturerMaxLength)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey(nameof(OwnerId))]
        public IdentityUser Owner { get; set; } = null!;

        [Required]
        [Range(PenMinInkCapacity, PenMaxInkCapacity)]
        public double InkCapacity { get; set; }
    }
}
