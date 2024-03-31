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
    public class Paper
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(PaperSizeMaxLength)]
        public string Size { get; set; } = string.Empty;

        [Required]
        [StringLength(PaperColorMaxLength)]
        public string Color { get; set; } = string.Empty;

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey(nameof(OwnerId))]
        public IdentityUser Owner { get; set; } = null!;

        [Required]
        [StringLength(PaperManufacturerMaxLength)]
        public string Manufacturer { get; set; } = string.Empty;
    }
}
