using System;
using System.Collections.Generic;

namespace OkulNotSistemi.Models;

public partial class Dersler
{
    public int DersId { get; set; }

    public string? DersAd { get; set; }

    public virtual ICollection<Notlar> Notlars { get; set; } = new List<Notlar>();
}
