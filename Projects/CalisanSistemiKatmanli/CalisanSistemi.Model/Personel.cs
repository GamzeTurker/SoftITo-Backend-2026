using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text;

namespace CalisanSistemi.Model
{
    public class Personel
    {
        [Key]
        public int PersonelNo { get; set; }
        [Required]
        public string PersonelAdSoyad { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
       
        public decimal Maas { get; set; }

        [Required]
        [DisplayName("Yaş")]
        public int Yas { get; set; }
        [Required]
        [StringLength(30)]
        public string Meslek { get; set; }
        public int DepartmanNo { get; set; }
        [ForeignKey("DepartmanNo")]
        public Departman Departman { get; set; }
    }
}
