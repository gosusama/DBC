namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dathangct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        public int? Soluong { get; set; }

        public decimal? Dongia { get; set; }

        public decimal? Tienhang { get; set; }

        public decimal? Tienvat { get; set; }

        public decimal? Thanhtien { get; set; }

        public decimal? Giabanlecovat { get; set; }

        public decimal? Giabanbuoncovat { get; set; }

        public int? Soluongthung { get; set; }

        public int? Toncuoikysl { get; set; }

        public decimal? Toncuoikygt { get; set; }

        public int? Tondaukysl { get; set; }

        public decimal? Tondaukygt { get; set; }

        public int? Nhaptksl { get; set; }

        public decimal? Nhaptkgt { get; set; }

        public int? Xuattksl { get; set; }

        public decimal? Xuattkgt { get; set; }

        public int? Soluongmax { get; set; }

        public int? Soluongmin { get; set; }

        public virtual TDS_Dathang TDS_Dathang { get; set; }

        public virtual TDS_Dmmathang TDS_Dmmathang { get; set; }
    }
}
