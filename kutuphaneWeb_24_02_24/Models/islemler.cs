using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kutuphaneWeb_24_02_24.Models
{
    public class islemler
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int islemID { get; set; }


        public int? uyeID { get; set; }
        [ForeignKey("id")]
        public virtual uyeler uyeler { get; set; }

        public int? kitapID { get; set; }
        [ForeignKey("id")]
        public virtual kitaplar kitaplar { get; set; }
        public bool? islemTur { get; set; }
        public DateTime? islemTarihi { get; set; }
    }
}