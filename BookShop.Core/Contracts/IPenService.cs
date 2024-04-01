using BookShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Contracts
{
    public interface IPenService
    {
        Task AddAsync(PenViewModel model);

        Task DeleteAsync(int id);

        Task EditAsync(PenViewModel model);

        Task<PenViewModel> GetByIdAsync(int id);
    }
}
