using BookShop.Core.Contracts;
using BookShop.Core.Models;
using BookShop.Data;
using BookShop.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Services
{
    public class PaperService : IPaperService
    {
        private readonly ApplicationDbContext _context;

        public PaperService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaperViewModel model)
        {
            var entity = new Paper()
            {
                Color = model.Color,
                Id = model.Id,
                Manufacturer = model.Manufacturer,
                OwnerId = model.OwnerId,
                Size = model.Size
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Papers.FindAsync(id);

            _context.Papers.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(PaperViewModel model)
        {
            var entity = new Paper()
            {
                Id = model.Id,
                Manufacturer = model.Manufacturer,
                OwnerId = model.OwnerId,
                Size = model.Size,
                Color = model.Color
            };

            _context.Papers.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PaperViewModel> GetByIdAsync(int id)
        {
            return await _context.Papers.Where(p => p.Id == id).Select(p => new PaperViewModel()
            {
                Manufacturer = p.Manufacturer,
                OwnerId = p.OwnerId,
                Size = p.Size,
                Color = p.Color,
                Id = p.Id
            }).FirstAsync();
        }
    }
}
