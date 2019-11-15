namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmquayhang
    {
        [Key]
        [StringLength(20)]
        public string Maquay { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(100)]
        public string Tenquay { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [StringLength(50)]
        public string Manguoitao { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        public int? Trangthai { get; set; }

        [StringLength(50)]
        public string Magiaodich { get; set; }

        public decimal? Tongtien { get; set; }

        [StringLength(200)]
        public string Icon { get; set; }
    }
}
