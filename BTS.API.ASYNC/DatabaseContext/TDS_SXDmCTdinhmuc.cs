namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXDmCTdinhmuc
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Macongthuc { get; set; }

        [StringLength(200)]
        public string Tencongthuc { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(20)]
        public string Nguoitao { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        public int? Trangthai { get; set; }

        [StringLength(20)]
        public string MasieuthiTP { get; set; }
    }
}
