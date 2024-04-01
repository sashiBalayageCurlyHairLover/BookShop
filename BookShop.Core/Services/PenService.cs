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
    public class PenService : IPenService
    {
        private readonly ApplicationDbContext _context;

        public PenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PenViewModel model)
        {
            var entity = new Pen()
            {
                Id = model.Id,
                InkCapacity = model.InkCapacity,
                Manufacturer = model.Manufacturer,
                OwnerId = model.OwnerId,
                PenColor = model.PenColor
            };

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Pens.FindAsync(id);

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(PenViewModel model)
        {
            var entity = new Pen()
            {
                Id = model.Id,
                Manufacturer = model.Manufacturer,
                OwnerId = model.OwnerId,
                PenColor = model.PenColor,
                InkCapacity = model.InkCapacity
            };

            _context.Pens.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PenViewModel> GetByIdAsync(int id)
        {
            return await _context.Pens.Where(p => p.Id == id).Select(p => new PenViewModel()
            {
                Id = p.Id,
                Manufacturer = p.Manufacturer,
                OwnerId = p.OwnerId,
                InkCapacity = p.InkCapacity,
                PenColor = p.PenColor
            }).FirstAsync();
        }
    }
}
