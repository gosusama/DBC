namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmmotasanphamchitiet
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Mamota { get; set; }

        [StringLength(500)]
        public string Giatri { get; set; }
    }
}
