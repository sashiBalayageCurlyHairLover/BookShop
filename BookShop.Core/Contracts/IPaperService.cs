using BookShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Contracts
{
    public interface IPaperService
    {
        Task AddAsync(PaperViewModel model);

        Task EditAsync(PaperViewModel model);

        Task DeleteAsync(int id);

        Task<PaperViewModel> GetByIdAsync(int id);
    }
}
