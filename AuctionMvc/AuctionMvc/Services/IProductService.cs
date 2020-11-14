using AuctionMvc.Models;
using AuctionMvc.Paginator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public interface IProductService
    {
        Task AddAsync(ProductViewModel productVM);
        Task<ProductViewModel> CreateViewModelAsync(long id);
        Task DeleteAsync(ProductViewModel productVM);
        Task EditAsync(ProductViewModel productVM);
        Task<IEnumerable<CategoryViewModel>> GetCategoryViewModelsAsync();
        Task<PaginatedList<ProductViewModel>> GetProductsAsync(string search, int? pageNumber);
    }
}