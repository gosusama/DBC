namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Ctugoc
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Matkno { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Matkco { get; set; }

        public decimal? Sotien { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [StringLength(20)]
        public string Manvcongno { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Makhohang { get; set; }

        [StringLength(500)]
        public string Ghichu { get; set; }
    }
}
