namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXLenhsx
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
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

        [Required]
        [StringLength(20)]
        public string Manvsanxuat { get; set; }

        public DateTime? Ngaynhandonhang { get; set; }

        public DateTime? Ngaygiaohang { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonhang { get; set; }

        public int? Kieumau { get; set; }

        [StringLength(200)]
        public string Ca { get; set; }
    }
}
