using System;
using System.Collections.Generic;

namespace ECommerceShop.Models;

public partial class Page
{
    public int PageId { get; set; }

    public string? PageName { get; set; }

    public string? Content { get; set; }

    public string? Thumb { get; set; }

    public bool Status { get; set; }

    public string? Title { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }

    public string? Alias { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? Position { get; set; }
}
