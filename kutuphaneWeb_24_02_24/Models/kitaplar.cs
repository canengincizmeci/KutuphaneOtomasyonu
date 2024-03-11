using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kutuphaneWeb_24_02_24.Models
{
    public class kitaplar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string kitapAd { get; set; }
        public string kitapYazar { get; set; }
        public string sayfaSayisi { get; set; }
        public bool? aktiflik { get; set; }
        public bool? oduncDurum { get; set; }
        public virtual ICollection<islemler> islemler { get; set; }
    }
}