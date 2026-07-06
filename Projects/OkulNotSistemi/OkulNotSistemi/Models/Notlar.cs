using System;
using System.Collections.Generic;

namespace OkulNotSistemi.Models;

public partial class Notlar
{
    public int NotId { get; set; }

    public int? OgrenciId { get; set; }

    public int? DersId { get; set; }

    public decimal? Vize { get; set; }

    public decimal? Final { get; set; }

    public virtual Dersler? Ders { get; set; }

    public virtual Ogrenciler? Ogrenci { get; set; }
}
