using BookShop.Core.Models;
using BookShop.Core.Services;
using BookShop.Data;
using BookShop.Infrastructure.Data.Models;
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

		[HttpGet]
		public async Task<IActionResult> Buy(int id)
		{
			var entity = await _context.Papers.FindAsync(id);

			if (entity == null)
			{
				return RedirectToAction(nameof(All));
			}

			if (entity.OwnerId == GetUserId())
			{
				return RedirectToAction(nameof(All));
			}

			var entityBuyer = await _context.PaperBuyers.FindAsync(GetUserId(), id);

			if (entityBuyer != null)
			{
				return RedirectToAction(nameof(All));
			}

			TempData["BuyPaperId"] = id;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Buy()
		{
			int id = (int)TempData["BuyPaperId"];

			var entity = await _context.Papers.FindAsync(id);

			if (entity == null)
			{
				return RedirectToAction(nameof(All));
			}

			if (entity.OwnerId == GetUserId())
			{
				return RedirectToAction(nameof(All));
			}

			if (await _context.PaperBuyers.FindAsync(GetUserId(), id) != null)
			{
				return RedirectToAction(nameof(All));
			}

			var paperBuyer = new PaperBuyer()
			{
				PaperId = id,
				BuyerId = GetUserId()
			};

			await _context.PaperBuyers.AddAsync(paperBuyer);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(All));
		}

		[HttpGet]
		public async Task<IActionResult> Sell(int id)
		{
			var entity = await _context.Papers.FindAsync(id);

			if (entity == null)
			{
				return RedirectToAction(nameof(All));
			}

			if (await _context.PaperBuyers.FindAsync(GetUserId(), id) == null)
			{
				return RedirectToAction(nameof(All));
			}

			TempData["SellPaperId"] = id;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Sell()
		{
			int id = (int)TempData["SellPaperId"];

			var entity = await _context.Papers.FindAsync(id);

			if (entity == null)
			{
				return RedirectToAction(nameof(All));
			}

			var paperBuyer = await _context.PaperBuyers.FindAsync(GetUserId(), id);

			if (paperBuyer == null)
			{
				return RedirectToAction(nameof(All));
			}

			_context.PaperBuyers.Remove(paperBuyer);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(All));
		}

		public async Task<IActionResult> BoughtPapers()
		{
			var model = await _context.PaperBuyers
				.Where(p => p.BuyerId == GetUserId())
				.ToListAsync();

			return View(model);
		}

		private string GetUserId()
		{
			return User.FindFirst(ClaimTypes.NameIdentifier).Value;
		}
	}
}
