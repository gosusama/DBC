namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmbohang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmbohang()
        {
            TDS_Dmbohangct = new HashSet<TDS_Dmbohangct>();
        }

        [Key]
        [StringLength(20)]
        public string Mabohang { get; set; }

        [Required]
        [StringLength(200)]
        public string Tenbo { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        public int? Trangthai { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmbohangct> TDS_Dmbohangct { get; set; }
    }
}
