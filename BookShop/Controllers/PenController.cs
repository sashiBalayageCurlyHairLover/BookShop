using BookShop.Core.Models;
using BookShop.Core.Services;
using BookShop.Data;
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
                return RedirectToAction(nameof(All));
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
                return RedirectToAction(nameof(All));
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
                return RedirectToAction(nameof(All));
            }

            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
