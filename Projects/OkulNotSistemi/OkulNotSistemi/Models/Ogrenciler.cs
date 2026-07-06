using System;
using System.Collections.Generic;

namespace OkulNotSistemi.Models;

public partial class Ogrenciler
{
    public int OgrenciId { get; set; }

    public string? OgrenciAd { get; set; }

    public string? OgrenciSoyad { get; set; }

    public string? Numara { get; set; }

    public string? Sinif { get; set; }

    public virtual ICollection<Notlar> Notlars { get; set; } = new List<Notlar>();
}
