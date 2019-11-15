namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Sodutaikhoan
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        public DateTime Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        [StringLength(250)]
        public string Ghichu { get; set; }

        public int Trangthai { get; set; }

        public int Nam { get; set; }
    }
}
