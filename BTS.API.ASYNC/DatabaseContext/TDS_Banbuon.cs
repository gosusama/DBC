namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Banbuon
    {
        [Key]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        public decimal? Tiendathanhtoan { get; set; }

        public decimal? Sotienno { get; set; }

        public decimal? Tongtien { get; set; }
    }
}
