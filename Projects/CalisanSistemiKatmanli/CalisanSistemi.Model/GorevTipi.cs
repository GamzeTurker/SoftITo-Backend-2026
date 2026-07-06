using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CalisanSistemi.Model
{
    public class GorevTipi
    {
        [Key]
        public int GorevTipId { get; set; }
        [Required]
        [StringLength(200)]
        public string Ad { get; set; }
    }
}
