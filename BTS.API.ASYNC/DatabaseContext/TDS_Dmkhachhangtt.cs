namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmkhachhangtt
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mact { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(50)]
        public string Tenct { get; set; }

        public int? Reset { get; set; }

        public decimal? Diemmin { get; set; }

        public decimal? Diemmax { get; set; }

        public decimal? Doanhsomin { get; set; }

        public decimal? Doanhsomax { get; set; }
    }
}
