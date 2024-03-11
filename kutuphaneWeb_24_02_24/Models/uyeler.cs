using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kutuphaneWeb_24_02_24.Models
{
    public class uyeler
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string adSoyad { get; set; }
        public int? kitapSayisi { get; set; }
        public string mail { get; set; }
        public bool? aktiflik { get; set; }
        public virtual ICollection<islemler> islemler { get; set; }



    }
}