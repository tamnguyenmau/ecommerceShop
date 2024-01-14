﻿using System;
using System.Collections.Generic;

namespace ECommerceShop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? ShortDesc { get; set; }

    public string? Content { get; set; }

    public int? CatId { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    public string? Thumb { get; set; }

    public string? Video { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public bool Bestsellers { get; set; }

    public bool HomeFlag { get; set; }

    public bool Status { get; set; }

    public string? Tags { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }

    public int? UnitslnStock { get; set; }

    public virtual ICollection<AttributesPrice> AttributesPrices { get; set; } = new List<AttributesPrice>();

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
