using AuctionMvc.Models;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    public interface IAuthService
    {
        Task<AccessToken> Authenticate(string email, string password);
        Task<bool> RegisterAsync(RegisterViewModel userVM);
    }
}