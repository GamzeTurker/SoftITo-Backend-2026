using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CalisanSistemi.Model
{
    public class Departman
    {
        [Key]
        public int DepartmanNo { get; set; }
        public string DepartmanAdi { get; set; }
        public int CalisanSayisi { get; set; }
        public string Aciklama { get; set; }
    }
}
