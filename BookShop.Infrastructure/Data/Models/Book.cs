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
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(BookTitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(BookDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string AuthorId { get; set; } = string.Empty;

        [ForeignKey(nameof(AuthorId))]
        public IdentityUser Author { get; set; } = null!;

        [Required]
        public DateTime PublishDate { get; set; }
    }
}
