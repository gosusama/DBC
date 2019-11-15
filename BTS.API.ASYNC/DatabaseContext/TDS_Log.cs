namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Log
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Malog { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(200)]
        public string Nguoitao { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Ngaytao { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string Tenmay { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(200)]
        public string Tenform { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(50)]
        public string Trangthai { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(2000)]
        public string Noidung { get; set; }
    }
}
