namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmgiaban
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public decimal? Giabanlecovat { get; set; }

        public decimal? Giabanbuoncovat { get; set; }

        public decimal? Giabanlechuavat { get; set; }

        public decimal? Giabanbuonchuavat { get; set; }

        public decimal? Giamuacovat { get; set; }

        public decimal? Giamuachuavat { get; set; }

        public decimal? Tylelaibuon { get; set; }

        public decimal? Tylelaile { get; set; }
        public virtual TDS_Dmmathang TDS_Dmmathang { get; set; }
    }
}
