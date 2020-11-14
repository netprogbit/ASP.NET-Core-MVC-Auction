using AuctionMvc.Models;
using AuctionMvc.Paginator;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public interface ICategoryService
    {
        Task AddAsync(CategoryViewModel categoryVM);
        Task<CategoryViewModel> CreateViewModelAsync(long id);
        Task EditAsync(CategoryViewModel categoryVM);
        Task<PaginatedList<CategoryViewModel>> GetCategoriesAsync(string search, int? pageNumber);
    }
}