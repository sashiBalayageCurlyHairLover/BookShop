using BookShop.Core.Models;
using BookShop.Core.Services;
using BookShop.Data;
using BookShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookShop.Controllers
{
    [Authorize]
    public class PenController : Controller
    {
        private readonly PenService _service;
        private readonly ApplicationDbContext _context;

        public PenController(PenService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }
        
        public async Task<IActionResult> All()
        {
            var model = await _context.Pens.Select(x => new PenViewModel()
            { 
                Id = x.Id,
                Manufacturer = x.Manufacturer,
                OwnerId = x.OwnerId,
                InkCapacity = x.InkCapacity,
                PenColor = x.PenColor
            })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PenViewModel model)
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
            var entity = await _context.Pens.FindAsync(id);
            
            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (entity.OwnerId != GetUserId())
            {
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

            TempData["EditPenOwnerId"] = entity.OwnerId;
            TempData["EditPenId"] = id;

            var model = new PenViewModel()
            {
                Id = entity.Id,
                InkCapacity = entity.InkCapacity,
                PenColor = entity.PenColor,
                Manufacturer = entity.Manufacturer,
                OwnerId = entity.OwnerId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(All));
            }

            string id = TempData["EditPenOwnerId"].ToString();

            if (id != GetUserId())
            {
                return RedirectToAction(nameof(All));
            }

            model.OwnerId = id;
            model.Id = (int)TempData["EditPenId"];
            await _service.EditAsync(model);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _service.GetByIdAsync(id);

            if (viewModel == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (viewModel.OwnerId != GetUserId())
            {
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

            TempData["DeletePenId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            int id = (int)TempData["DeletePenId"];

            var viewModel = await _service.GetByIdAsync(id);

            if (viewModel == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (viewModel.OwnerId != GetUserId())
            {
				if (!User.IsInRole("Administrator"))
				{
					return RedirectToAction(nameof(All));
				}
			}

            foreach (var penBuyer in await _context.PenBuyers.Where(pb => pb.PenId == id).ToListAsync())
            {
                _context.PenBuyers.Remove(penBuyer);
            }

            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Buy(int id)
        {
            var entity = await _context.Pens.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (entity.OwnerId == GetUserId())
            {
                return RedirectToAction(nameof(All));
            }

            var entityBuyer = await _context.PenBuyers.FindAsync(GetUserId(), id);

            if (entityBuyer != null)
            {
                return RedirectToAction(nameof(All));
            }

            TempData["BuyPenId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buy()
        {
            int id = (int)TempData["BuyPenId"];

            var entity = await _context.Pens.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (entity.OwnerId == GetUserId())
            {
                return RedirectToAction(nameof(All));
            }

            if (await _context.PenBuyers.FindAsync(GetUserId(), id) != null)
            {
                return RedirectToAction(nameof(All));
            }

            var penBuyer = new PenBuyer()
            {
                PenId = id,
                BuyerId = GetUserId()
            };

            await _context.PenBuyers.AddAsync(penBuyer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Sell(int id)
        {
            var entity = await _context.Pens.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (await _context.PenBuyers.FindAsync(GetUserId(), id) == null)
            {
                return RedirectToAction(nameof(All));
            }

            TempData["SellPenId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sell()
        {
            int id = (int)TempData["SellPenId"];

            var entity = await _context.Pens.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            var penBuyer = await _context.PenBuyers.FindAsync(GetUserId(), id);

            if (penBuyer == null)
            {
                return RedirectToAction(nameof(All));
            }

            _context.PenBuyers.Remove(penBuyer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> BoughtPens()
        {
            var model = await _context.PenBuyers
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
