using BookShop.Core.Models;
using BookShop.Data;
using BookShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookShop.Controllers
{
	[Authorize]
	public class ContactMessageController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ContactMessageController(ApplicationDbContext context)
		{
			_context = context;
		}

		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> All()
		{
			var model = await _context.ContactMessages.Select(x => new ContactMessageViewModel()
			{
				Description = x.Description,
				Id = x.Id,
				SenderId = x.SenderId,
				Title = x.Title
			}).ToListAsync();

			foreach (var item in model)
			{
				var senderName = _context.Users.Find(item.SenderId).UserName;

				item.SenderName = senderName;
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(ContactMessageViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("All", "Book");
			}

			var entity = new ContactMessage()
			{
				Description = model.Description,
				Id = model.Id,
				Title = model.Title
			};

			entity.SenderId = GetUserId();

            await _context.AddAsync(entity);
			await _context.SaveChangesAsync();

			return RedirectToAction("All", "Book");
		}

		[Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _context.ContactMessages.FindAsync(id) == null)
            {
                return RedirectToAction(nameof(All));
            }

            var entity = await _context.ContactMessages.FindAsync(id);

            if (GetUserId() != entity.SenderId)
            {
                if (!User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(All));
                }
            }

            TempData["DeleteContactMessageId"] = id;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            int id = (int)TempData["DeleteContactMessageId"];

			var entity = await _context.ContactMessages.FindAsync(id);

			if (entity == null)
			{
				return RedirectToAction(nameof(All));
			}

			if (GetUserId() != entity.SenderId)
            {
                if (!User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(All));
                }
            }

            _context.ContactMessages.Remove(entity);
			await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier).Value;
		}
	}
}
