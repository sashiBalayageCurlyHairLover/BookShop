using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BookShop.Infrastructure.Data.DataConstants;

namespace BookShop.Infrastructure.Data.Models
{
	public class ContactMessage
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ContactMessageTitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(ContactMessageDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [ForeignKey(nameof(SenderId))]
        public IdentityUser Sender { get; set; } = null!;
    }
}
