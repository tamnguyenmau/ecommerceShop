using System;
using System.Collections.Generic;

namespace ECommerceShop.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public string? Thumb { get; set; }

    public bool Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Author { get; set; }

    public int? AccountId { get; set; }

    public string? Tags { get; set; }

    public int? CatId { get; set; }

    public bool IsHost { get; set; }

    public bool IsNewFeed { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }

    public int? Views { get; set; }

    public string? Alias { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Category? Cat { get; set; }
}
