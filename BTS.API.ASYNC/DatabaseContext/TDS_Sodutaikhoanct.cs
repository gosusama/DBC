namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Sodutaikhoanct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Matk { get; set; }

        public decimal? Sotienduno { get; set; }

        public decimal? Sotienduco { get; set; }
    }
}
