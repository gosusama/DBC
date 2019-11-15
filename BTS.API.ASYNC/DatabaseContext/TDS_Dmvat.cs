namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmvat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmvat()
        {
            TDS_Dmmathang = new HashSet<TDS_Dmmathang>();
        }

        [Key]
        [StringLength(20)]
        public string Mavat { get; set; }

        [StringLength(100)]
        public string Tenvat { get; set; }

        public int? Vat { get; set; }

        [StringLength(100)]
        public string Congthuc { get; set; }

        public int? Loaivat { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        public int? Khongchiuthue { get; set; }

        [StringLength(250)]
        public string Doanhso { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmmathang> TDS_Dmmathang { get; set; }
    }
}
