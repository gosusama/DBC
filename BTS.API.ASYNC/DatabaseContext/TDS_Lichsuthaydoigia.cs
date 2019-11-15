namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Lichsuthaydoigia
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public decimal? Gianhapcu { get; set; }

        public decimal? Giabanlecu { get; set; }

        public decimal? Giabanbuoncu { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngaytao { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime Ngayphatsinh { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string Formthaydoi { get; set; }
    }
}
