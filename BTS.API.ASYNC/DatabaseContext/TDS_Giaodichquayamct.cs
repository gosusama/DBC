namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Giaodichquayamct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [Required]
        [StringLength(20)]
        public string Manganhhang { get; set; }

        [StringLength(20)]
        public string Manhomhang { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Makhohang { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string Mabohang { get; set; }

        public int? Soluong { get; set; }

        public decimal? Giabanlecovat { get; set; }

        public decimal? Giabanlechuavat { get; set; }

        public decimal? Tienhang { get; set; }

        public decimal? Tienvat { get; set; }

        public decimal? Thanhtien { get; set; }

        public decimal? Tyleck { get; set; }

        public decimal? Tienck { get; set; }

        public int? Sort { get; set; }

        public decimal? Giavon { get; set; }

        public decimal? Doanhthu { get; set; }

        public int? Trangthai { get; set; }

        public int? Soluongxuly { get; set; }

        [StringLength(20)]
        public string Makhoban { get; set; }

        [StringLength(500)]
        public string Magiaodichxuly { get; set; }

        public virtual TDS_Dmmathang TDS_Dmmathang { get; set; }
    }
}
