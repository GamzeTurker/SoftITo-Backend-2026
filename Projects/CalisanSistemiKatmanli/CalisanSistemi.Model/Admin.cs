using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CalisanSistemi.Model
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string KullaniciAdi { get; set; }
        [Required]
        public string Sifre { get; set; }
        public string Email { get; set; }
    }
}
