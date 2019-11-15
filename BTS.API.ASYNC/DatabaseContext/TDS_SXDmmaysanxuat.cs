namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXDmmaysanxuat
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mamaysanxuat { get; set; }

        [Required]
        [StringLength(50)]
        public string Tenmaysanxuat { get; set; }

        public int Trangthai { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        public int Hesosanxuat { get; set; }
    }
}
