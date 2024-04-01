using BookShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Contracts
{
    public interface IBookService
    {
        Task AddAsync(BookViewModel model);

        Task EditAsync(BookViewModel model);

        Task DeleteAsync(int id);

        Task<BookViewModel> GetByIdAsync(int id);
    }
}
