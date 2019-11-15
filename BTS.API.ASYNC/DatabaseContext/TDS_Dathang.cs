namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dathang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dathang()
        {
            TDS_Dathangct = new HashSet<TDS_Dathangct>();
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

        [StringLength(200)]
        public string Ghichu { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public int? Trangthai { get; set; }

        [Required]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(500)]
        public string Manhacungcap { get; set; }

        [StringLength(200)]
        public string Manganh { get; set; }

        [StringLength(200)]
        public string Tenkhachhang { get; set; }

        [StringLength(500)]
        public string Manhom { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        public virtual TDS_Dmkhachhang TDS_Dmkhachhang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dathangct> TDS_Dathangct { get; set; }
    }
}
