namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Giaodichquay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Giaodichquay()
        {
            TDS_Giaodichquayct = new HashSet<TDS_Giaodichquayct>();
            TDS_Taikhoanhachtoan = new HashSet<TDS_Taikhoanhachtoan>();
        }

        [Key]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Maptnx { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        [StringLength(250)]
        public string Ghichu { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        public int? Trangthai { get; set; }

        [Required]
        [StringLength(20)]
        public string Maquay { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Manhanviencongno { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichquayct> TDS_Giaodichquayct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Taikhoanhachtoan> TDS_Taikhoanhachtoan { get; set; }
    }
}
