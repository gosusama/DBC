namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmkhachhang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmkhachhang()
        {
            TDS_Dathang = new HashSet<TDS_Dathang>();
            TDS_Taikhoanhachtoan = new HashSet<TDS_Taikhoanhachtoan>();
        }

        [Key]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        [Required]
        [StringLength(20)]
        public string Maloaikhach { get; set; }

        [StringLength(200)]
        public string Tenkhachhang { get; set; }

        [StringLength(200)]
        public string Diachi { get; set; }

        [StringLength(20)]
        public string Dienthoai { get; set; }

        [StringLength(20)]
        public string Dienthoai2 { get; set; }

        [StringLength(20)]
        public string Fax { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Masothue { get; set; }

        [StringLength(20)]
        public string Mahopdong { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        public bool? Trangthai { get; set; }

        public decimal? Congnomax { get; set; }

        [StringLength(20)]
        public string Matuyen { get; set; }

        [StringLength(20)]
        public string Manganhang { get; set; }

        [StringLength(20)]
        public string Machinhanh { get; set; }

        [StringLength(20)]
        public string Sotaikhoannganhang { get; set; }

        [StringLength(250)]
        public string Diachigiaohang { get; set; }

        [StringLength(50)]
        public string Nguoigiaodich { get; set; }

        public decimal? Diem { get; set; }

        public decimal? Doanhso { get; set; }

        [StringLength(20)]
        public string Mathe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dathang> TDS_Dathang { get; set; }

        public virtual TDS_Dmloaikhachhang TDS_Dmloaikhachhang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Taikhoanhachtoan> TDS_Taikhoanhachtoan { get; set; }
    }
}
