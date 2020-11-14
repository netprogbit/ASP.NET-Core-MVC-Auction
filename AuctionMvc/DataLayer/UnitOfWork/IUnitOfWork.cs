using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace DataLayer.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<User> Users { get; }

        IDbContextTransaction BeginTransaction();
        void Dispose();
        void Dispose(bool disposing);
        Task SaveAsync();
    }
}