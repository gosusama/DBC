namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Giaodich
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Giaodich()
        {
            TDS_Giaodichct = new HashSet<TDS_Giaodichct>();
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

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public int? Trangthai { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Sochungtugoc { get; set; }

        public DateTime? Ngaychungtugoc { get; set; }

        [StringLength(30)]
        public string Kemtheo { get; set; }

        public int? Songaythanhtoan { get; set; }

        public decimal? Tiendathanhtoan { get; set; }

        [StringLength(20)]
        public string Mahopdong { get; set; }

        public DateTime? Ngaythanhtoan { get; set; }

        public DateTime? Ngayhoadon { get; set; }

        [StringLength(20)]
        public string Sohoadon { get; set; }

        [StringLength(20)]
        public string Kyhieuhoadon { get; set; }

        [StringLength(20)]
        public string Magiaodichphu { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(250)]
        public string Diachigiaohang { get; set; }

        [StringLength(20)]
        public string Manhanviencongno { get; set; }

        [StringLength(20)]
        public string Manhanviendathang { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichct> TDS_Giaodichct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Taikhoanhachtoan> TDS_Taikhoanhachtoan { get; set; }
    }
}
