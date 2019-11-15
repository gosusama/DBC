namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmhopdong
    {
        [Key]
        [StringLength(20)]
        public string Mahopdong { get; set; }

        [StringLength(50)]
        public string Sohopdong { get; set; }

        [StringLength(200)]
        public string Tenhopdong { get; set; }

        public DateTime? Ngayky { get; set; }

        public DateTime? Ngayhethan { get; set; }

        public bool? Trangthai { get; set; }

        [StringLength(500)]
        public string Tomtathopdong { get; set; }

        [StringLength(500)]
        public string Ghichu { get; set; }

        [Required]
        [StringLength(20)]
        public string Makhachhang { get; set; }
    }
}
