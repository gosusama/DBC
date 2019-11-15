namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmkehang
    {
        [Key]
        [StringLength(20)]
        public string Makehang { get; set; }

        [StringLength(200)]
        public string Tenkehang { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }
    }
}
