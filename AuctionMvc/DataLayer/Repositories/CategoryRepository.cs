using DataLayer.Abstractions;
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
    /// Categories repository
    /// </summary>
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AuctionDbContext _db;

        public CategoryRepository(AuctionDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> FindAllAsync(string search)
        {
            return await _db.Categories.AsNoTracking().Where(c => c.Name.Contains(search)).ToListAsync();
        }

        public async Task<Category> FindAsync(long id)
        {
            return await _db.Categories.FindAsync(id);
        }

        public async Task<Category> FindAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _db.Categories.SingleOrDefaultAsync(predicate);
        }

        public async Task CreateAsync(Category category)
        {
            await _db.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            _db.Entry(category).State = EntityState.Modified;
        }

        public bool Delete(long id)
        {
            var category = _db.Categories.Find(id);

            if (category == null)
                return false;

            _db.Categories.Remove(category);
            return true;
        }
    }
}
