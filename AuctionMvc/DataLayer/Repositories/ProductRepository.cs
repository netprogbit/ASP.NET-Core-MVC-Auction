using DataLayer.DbContexts;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    /// <summary>
    /// Products repository
    /// </summary>
    public class ProductRepository : IRepository<Product>
    {
        private readonly AuctionDbContext _db;

        public ProductRepository(AuctionDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Product>> FindAllAsync(string search)
        {
            return await _db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.Category.Name.Contains(search) || p.Name.Contains(search)).ToListAsync();
        }

        public async Task<Product> FindAsync(long id)
        {
            return await _db.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> FindAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _db.Products.Include(p => p.Category).SingleOrDefaultAsync(predicate);
        }

        public async Task CreateAsync(Product product)
        {
            await _db.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _db.Entry(product).State = EntityState.Modified;
        }

        public bool Delete(long id)
        {
            var product = _db.Products.Find(id);

            if (product == null)
                return false;

            _db.Products.Remove(product);
            return true;
        }
    }
}
