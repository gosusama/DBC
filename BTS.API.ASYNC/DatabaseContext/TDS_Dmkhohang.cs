namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmkhohang
    {
        [Key]
        [StringLength(20)]
        public string Makhohang { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Maloaikho { get; set; }

        [Required]
        [StringLength(100)]
        public string Tenkhohang { get; set; }

        [StringLength(200)]
        public string Diachi { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        public virtual TDS_Dmloaikhohang TDS_Dmloaikhohang { get; set; }
    }
}
