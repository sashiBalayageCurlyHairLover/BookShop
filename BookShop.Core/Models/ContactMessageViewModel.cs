using System.ComponentModel.DataAnnotations;
using static BookShop.Infrastructure.Data.DataConstants;

namespace BookShop.Core.Models
{
	public class ContactMessageViewModel
	{
        public int Id { get; set; }

        [Required]
		[StringLength(ContactMessageTitleMaxLength, MinimumLength = ContactMessageTitleMinLength)]
		public string Title { get; set; } = string.Empty;

		[Required]
		[StringLength(ContactMessageDescriptionMaxLength, MinimumLength = ContactMessageDescriptionMinLength)]
		public string Description { get; set; } = string.Empty;

		[Required]
		[StringLength(ContactMessageSenderNameMaxLength,  MinimumLength = ContactMessageSenderNameMinLength)]
		public string SenderName { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;
	}
}
