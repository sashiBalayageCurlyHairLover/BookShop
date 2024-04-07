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
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BookService _bookService;

        public BookController(ApplicationDbContext context, BookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        public async Task<IActionResult> All()
        {
            var model = await _context.Books.Select(b => new BookViewModel()
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                PublishDate = b.PublishDate,
                AuthorId = b.AuthorId
            }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Add() 
        { 
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Add(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(All));
            }

            model.AuthorId = GetUserId();
            await _bookService.AddAsync(model);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (await _context.Books.FindAsync(id) == null)
            {
                return RedirectToAction(nameof(All));
            }

            var model = await _bookService.GetByIdAsync(id);

            if (GetUserId() != model.AuthorId)
            {
                if (!User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(All));
                }
            }

            TempData["EditBookId"] = id;
            TempData["EditBookAuthorId"] = model.AuthorId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(All));
            }

            model.Id = (int)TempData["EditBookId"];
            model.AuthorId = TempData["EditBookAuthorId"].ToString();

            await _bookService.EditAsync(model);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _context.Books.FindAsync(id) == null)
            {
                return RedirectToAction(nameof(All));
            }

            var entity = await _context.Books.FindAsync(id);

            if (GetUserId() != entity.AuthorId)
            {
                if (!User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(All));
                }
            }

            TempData["DeleteBookId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            int id = (int)TempData["DeleteBookId"];

            if (GetUserId() != _context.Books.FindAsync(id).Result.AuthorId)
            {
                if (!User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(All));
                }
            }

            await _bookService.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Buy(int id)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (entity.AuthorId == GetUserId())
            {
                return RedirectToAction(nameof(All));
            }

            var entityBuyer = await _context.BookBuyers.FindAsync(GetUserId(), id);

            if (entityBuyer != null)
            {
                return RedirectToAction(nameof(All));
            }

            TempData["BuyBookId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buy()
        {
            int id = (int)TempData["BuyBookId"];

            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (entity.AuthorId == GetUserId())
            {
                return RedirectToAction(nameof(All));
            }

            if (await _context.BookBuyers.FindAsync(GetUserId(), id) != null)
            {
                return RedirectToAction(nameof(All));
            }

            var bookBuyer = new BookBuyer()
            {
                BookId = id,
                BuyerId = GetUserId()
            };

            await _context.BookBuyers.AddAsync(bookBuyer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Sell(int id)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            if (await _context.BookBuyers.FindAsync(GetUserId(), id) == null)
            {
                return RedirectToAction(nameof(All));
            }

            TempData["SellBookId"] = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sell()
        {
            int id = (int)TempData["SellBookId"];

            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(All));
            }

            var bookBuyer = await _context.BookBuyers.FindAsync(GetUserId(), id);
            
            if (bookBuyer == null)
            {
                return RedirectToAction(nameof(All));
            }

            _context.BookBuyers.Remove(bookBuyer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> BoughtBooks()
        {
            var model = await _context.BookBuyers
                .Where(b => b.BuyerId == GetUserId())
                .ToListAsync();

            return View(model);
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
