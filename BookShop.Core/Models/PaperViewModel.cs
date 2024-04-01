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
    public class PaperViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(PaperSizeMaxLength, MinimumLength = PaperSizeMinLength)]
        public string Size { get; set; } = string.Empty;

        [Required]
        [StringLength(PaperColorMaxLength, MinimumLength = PaperColorMinLength)]
        public string Color { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [StringLength(PaperManufacturerMaxLength, MinimumLength = PaperManufacturerMinLength)]
        public string Manufacturer { get; set; } = string.Empty;
    }
}
