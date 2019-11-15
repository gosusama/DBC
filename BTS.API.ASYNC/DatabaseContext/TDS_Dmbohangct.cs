namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmbohangct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mabohang { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        public int? Soluong { get; set; }

        public decimal? Tylechietkhaule { get; set; }

        public decimal? Tylechietkhaubuon { get; set; }

        public int? Trangthai { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        public decimal? Tongtienbanbuon { get; set; }

        public decimal? Tongtienbanle { get; set; }

        public virtual TDS_Dmbohang TDS_Dmbohang { get; set; }

        public virtual TDS_Dmmathang TDS_Dmmathang { get; set; }
    }
}
