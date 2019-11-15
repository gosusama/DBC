namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmtuyenduong
    {
        [Key]
        [StringLength(20)]
        public string Matuyen { get; set; }

        [Required]
        [StringLength(200)]
        public string Tentuyen { get; set; }

        [Required]
        [StringLength(200)]
        public string Ghichu { get; set; }
    }
}
