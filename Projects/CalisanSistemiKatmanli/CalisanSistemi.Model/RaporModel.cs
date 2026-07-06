using System;
using System.Collections.Generic;

namespace CalisanSistemi.Model
{
    public class RaporModel
    {
        // Sistem Analitik Özellikleri
        public int ToplamPersonel { get; set; }
        public int ToplamDepartman { get; set; }
        public int ToplamGorev { get; set; }
        public int TamamlananGorevSayisi { get; set; }
        public int BekleyenGorevSayisi { get; set; }

        // Rapor Listeleri
        public List<Personel> PersonelListesi { get; set; } = new List<Personel>();
        public List<Gorev> GorevListesi { get; set; } = new List<Gorev>();
        public List<Departman> DepartmanListesi { get; set; } = new List<Departman>();
    }
}
