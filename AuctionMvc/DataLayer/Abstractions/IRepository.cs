using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Abstractions
{
  internal interface IRepository<T> where T : class
  {
    IQueryable<T> GetAll();
    Task<T> GetAsync(long id);
    Task CreateAsync(T item);
    void Update(T item);
    bool Delete(long id);
  }
}
