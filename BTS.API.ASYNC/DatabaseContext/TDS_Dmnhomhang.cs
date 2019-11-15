namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmnhomhang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmnhomhang()
        {
            TDS_Dmmathang = new HashSet<TDS_Dmmathang>();
        }

        [Key]
        [StringLength(20)]
        public string Manhomhang { get; set; }

        [StringLength(150)]
        public string Tennhomhang { get; set; }

        [StringLength(20)]
        public string Manganh { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmmathang> TDS_Dmmathang { get; set; }

        public virtual TDS_Dmnganhhang TDS_Dmnganhhang { get; set; }
    }
}
