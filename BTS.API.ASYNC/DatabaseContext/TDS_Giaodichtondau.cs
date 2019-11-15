namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Giaodichtondau
    {
        [Key]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Makhohang { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        public DateTime? Ngaytao { get; set; }

        public int Trangthai { get; set; }

        public int Nam { get; set; }
    }
}
