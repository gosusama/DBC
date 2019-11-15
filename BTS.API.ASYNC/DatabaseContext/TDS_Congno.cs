namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Congno
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Sochungtu { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(10)]
        public string Loaigiaodich { get; set; }

        [StringLength(20)]
        public string Manhacungcap { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Manhanvien { get; set; }

        [StringLength(20)]
        public string Sochungtugoc { get; set; }

        public decimal? Sotienphatsinhno { get; set; }

        public decimal? Sotienphatsinhco { get; set; }

        [StringLength(300)]
        public string Ghichu { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Nguoitao { get; set; }

        public int? Trangthai { get; set; }
    }
}
