using AuctionMvc.Models;
using AuctionMvc.Paginator;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public interface IUserService
    {
        Task<UserViewModel> CreateViewModelAsync(long id);
        Task DeleteAsync(long id);
        Task EditAsync(UserViewModel userVM);
        Task<PaginatedList<UserViewModel>> GetCategoriesAsync(string search, int? pageNumber);
    }
}