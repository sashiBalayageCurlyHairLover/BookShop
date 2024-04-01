using BookShop.Core.Contracts;
using BookShop.Core.Models;
using BookShop.Data;
using BookShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookViewModel model)
        {
            var entity = new Book()
            {
                AuthorId = model.AuthorId,
                Description = model.Description,
                Id = model.Id,
                PublishDate = model.PublishDate,
                Title = model.Title
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Books.FindAsync(id);

            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(BookViewModel model)
        {
            var entity = new Book()
            {
                AuthorId = model.AuthorId,
                Description = model.Description,
                Id = model.Id,
                PublishDate = model.PublishDate,
                Title = model.Title
            };

            _context.Books.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BookViewModel> GetByIdAsync(int id)
        {
            return await _context.Books.Where(b => b.Id == id).Select(b => new BookViewModel()
            {
                AuthorId = b.AuthorId,
                Description = b.Description,
                Id = b.Id,
                PublishDate = b.PublishDate,
                Title = b.Title
            }).FirstAsync();
        }
    }
}
