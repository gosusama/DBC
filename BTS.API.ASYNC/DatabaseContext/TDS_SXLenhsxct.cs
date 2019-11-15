namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXLenhsxct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        public int Soluong { get; set; }

        public decimal Dongia { get; set; }

        public decimal Tienhang { get; set; }

        public decimal? Tienvat { get; set; }

        public decimal? Thanhtien { get; set; }

        [Required]
        [StringLength(20)]
        public string Mamaysanxuat { get; set; }

        [StringLength(20)]
        public string Magiacong { get; set; }

        public int? Trangthai { get; set; }

        [StringLength(20)]
        public string Masovitinh { get; set; }

        [StringLength(20)]
        public string Macongthuc { get; set; }

        public int? Soluongthucte { get; set; }
    }
}
