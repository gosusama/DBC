namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_SXDmNangSuatNV
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(200)]
        public string Makhachhang { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(100)]
        public string Camay { get; set; }

        public int? Chotsohang { get; set; }

        public DateTime? Ngaychot { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }
    }
}
