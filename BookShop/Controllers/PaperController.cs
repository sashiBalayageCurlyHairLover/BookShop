using BookShop.Core.Models;
using BookShop.Core.Services;
using BookShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookShop.Controllers
{
	[Authorize]
	public class PaperController : Controller
	{
		private readonly PaperService _service;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public PaperController(PaperService service, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_service = service;
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> All()
		{
			var model = await _context.Papers.Select(x => new PaperViewModel()
			{
				Id = x.Id,
				Color = x.Color,
				Manufacturer = x.Manufacturer,
				OwnerId = x.OwnerId,
				Size = x.Size
			}).ToListAsync();

			return View(model);
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(PaperViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction(nameof(All));
			}

			model.OwnerId = GetUserId();
			await _service.AddAsync(model);

			return RedirectToAction(nameof(All));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (await _context.Papers.FindAsync(id) == null)
			{
				return RedirectToAction(nameof(All));
			}

			var model = await _service.GetByIdAsync(id);

			if (GetUserId() != model.OwnerId)
			{
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

			TempData["EditPaperId"] = id;
			TempData["EditPaperOwnerId"] = model.OwnerId;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(PaperViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction(nameof(All));
			}

			model.Id = (int)TempData["EditPaperId"];
			model.OwnerId = TempData["EditPaperOwnerId"].ToString();

			await _service.EditAsync(model);

			return RedirectToAction(nameof(All));
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			if (await _context.Papers.FindAsync(id) == null)
			{
				return RedirectToAction(nameof(All));
			}

			var entity = await _context.Papers.FindAsync(id);

			if (GetUserId() != entity.OwnerId)
			{
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

			TempData["DeletePaperId"] = id;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Delete()
		{
			int id = (int)TempData["DeletePaperId"];

			if (GetUserId() != _context.Papers.FindAsync(id).Result.OwnerId)
			{
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

			await _service.DeleteAsync(id);

			return RedirectToAction(nameof(All));
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
		}
	}
}
