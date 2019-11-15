namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmmaxmindieuchuyen
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Makhohang { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        public int Tonmax { get; set; }

        public int Tonmin { get; set; }

        public int Trangthaikinhdoanh { get; set; }

        public int Chanthung { get; set; }
    }
}
