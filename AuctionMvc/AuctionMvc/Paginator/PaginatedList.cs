using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionMvc.Paginator
{
  /// <summary>
  /// Pagination features
  /// </summary>  
  public class PaginatedList<T> : List<T>
  {
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
      PageNumber = pageNumber;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);

      this.AddRange(items);
    }

    public bool HasPreviousPage
    {
      get => PageNumber > 1;
    }

    public bool HasNextPage
    {
      get => PageNumber < TotalPages;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
      var count = await source.CountAsync();
      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
      return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
  }
}
