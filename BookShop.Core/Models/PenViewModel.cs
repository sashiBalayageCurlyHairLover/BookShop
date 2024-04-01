using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookShop.Infrastructure.Data.DataConstants;

namespace BookShop.Core.Models
{
    public class PenViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(PenColorMaxLength, MinimumLength = PenColorMinLength)]
        public string PenColor { get; set; } = string.Empty;

        [Required]
        [StringLength(PenManufacturerMaxLength, MinimumLength = PenManufacturerMinLength)]
        public string Manufacturer { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [Range(PenMinInkCapacity, PenMaxInkCapacity)]
        public double InkCapacity { get; set; }
    }
}
