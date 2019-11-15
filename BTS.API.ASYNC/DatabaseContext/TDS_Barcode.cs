namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Barcode
    {
        [Required]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Key]
        [StringLength(20)]
        public string Barcode { get; set; }
    }
}
