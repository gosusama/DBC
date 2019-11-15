namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXDmCTDinhmucct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Macongthuc { get; set; }

        public int? SLdinhmuc { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public int? Sothutu { get; set; }
    }
}
