using System;
using System.Collections.Generic;

namespace ECommerceShop.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? CatName { get; set; }

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public int? Levels { get; set; }

    public int? Position { get; set; }

    public bool? Status { get; set; }

    public string? Cover { get; set; }

    public string? Thumd { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public string? MetaDesk { get; set; }

    public string? MetaKey { get; set; }

    public string? SchemaMakup { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
