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
    /// Users repository
    /// </summary>
    public class UserRepository : IRepository<User>
    {
        private readonly AuctionDbContext _db;

        public UserRepository(AuctionDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> FindAllAsync(string search)
        {
            return await _db.Users.AsNoTracking().Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search)).ToListAsync();
        }

        public async Task<User> FindAsync(long id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _db.Users.SingleOrDefaultAsync(predicate);
        }

        public async Task CreateAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _db.Entry(user).State = EntityState.Modified;
            _db.Entry(user).Property(x => x.Password).IsModified = false;
        }

        public bool Delete(long id)
        {
            var user = _db.Users.Find(id);

            if (user == null)
                return false;

            _db.Users.Remove(user);
            return true;
        }
    }
}
