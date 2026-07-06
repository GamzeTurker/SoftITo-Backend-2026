using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CalisanSistemi.Model
{
    public class Gorev
    {
        [Key]
        public int GorevId { get; set; }

      
        public int GorevTipId{ get; set; }

        public bool TamamlandiMi { get; set; }

        public DateTime? SonTarih { get; set; }

        public int PersonelNo { get; set; }

        [ForeignKey("PersonelNo")]
        public Personel Personel { get; set; }

        [ForeignKey("GorevTipId")]
        public GorevTipi GorevTipi { get; set; }
    }
}
