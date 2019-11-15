namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmmotasanpham
    {
        [Key]
        [StringLength(20)]
        public string Mamota { get; set; }

        [Required]
        [StringLength(200)]
        public string Tenmota { get; set; }

        public int? Thutu { get; set; }
    }
}
