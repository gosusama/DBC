namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmgiagiaodich
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        public decimal? Giabanlecovat { get; set; }

        public decimal? Giabanbuoncovat { get; set; }

        public decimal? Giabanlechuavat { get; set; }

        public decimal? Giabanbuonchuavat { get; set; }

        public decimal? Tylelaibuon { get; set; }

        public decimal? Tylelaile { get; set; }

        public int? Vat { get; set; }
    }
}
