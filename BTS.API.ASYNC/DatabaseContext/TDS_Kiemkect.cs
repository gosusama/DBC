namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Kiemkect
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [StringLength(20)]
        public string Manhomvtu { get; set; }

        [StringLength(20)]
        public string Makehang { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Mavtu { get; set; }

        public int? Soluongtonmay { get; set; }

        public int? Soluongkiemke { get; set; }

        public int? Soluongchenhlech { get; set; }

        public decimal? Tientonmay { get; set; }

        public decimal? Tienkiemke { get; set; }

        public decimal? Tienchenhlech { get; set; }

        public decimal? Giavon { get; set; }

        public int? Soluongnhap { get; set; }

        public int? Soluongxuat { get; set; }

        public virtual TDS_Kiemke TDS_Kiemke { get; set; }
    }
}
