namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Nhomquyen
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Manhomquyen { get; set; }

        [Required]
        [StringLength(150)]
        public string Tennhomquyen { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime Ngayphatsinh { get; set; }
    }
}
