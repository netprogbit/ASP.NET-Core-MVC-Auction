using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
  public class Category
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public List<Product> Products { get; set; }
    public string Name { get; set; }
    public string ImageFileName { get; set; }
  }
}
