namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Quyctkt
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mactktpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Mactu { get; set; }

        [Required]
        [StringLength(20)]
        public string Soctkt { get; set; }

        public DateTime Ngayctkt { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        [StringLength(200)]
        public string Kemtheo { get; set; }

        [StringLength(1000)]
        public string Ghichu { get; set; }

        public int Trangthai { get; set; }

        [Required]
        [StringLength(4000)]
        public string Magiaodichpk { get; set; }

        public DateTime Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public DateTime? Ngayhoadon { get; set; }

        [StringLength(20)]
        public string Sohoadon { get; set; }

        [StringLength(20)]
        public string Kyhieuhoadon { get; set; }
    }
}
