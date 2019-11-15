namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Khuyenmai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Khuyenmai()
        {
            TDS_Khuyenmaict = new HashSet<TDS_Khuyenmaict>();
        }

        [Key]
        [StringLength(20)]
        public string Machuongtrinh { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public int Loaiapdung { get; set; }

        public int Maloaichuongtrinh { get; set; }

        [StringLength(250)]
        public string Tenchuongtrinh { get; set; }

        public int? Trangthai { get; set; }

        public int? Trangthaikm { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        public DateTime? Ngaybatdau { get; set; }

        public DateTime? Ngayketthuc { get; set; }

        public int? Giobatdau { get; set; }

        public int? Gioketthuc { get; set; }

        public int? Phutbatdau { get; set; }

        public int? Phutkethuc { get; set; }

        public DateTime? Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [StringLength(20)]
        public string Mact { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Khuyenmaict> TDS_Khuyenmaict { get; set; }
    }
}
