namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmtk
    {
        [Key]
        [StringLength(20)]
        public string Matk { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Maloaitk { get; set; }

        [StringLength(20)]
        public string Matkcha { get; set; }

        public int? Tkchitiet { get; set; }

        public int? Bactk { get; set; }

        public int? Tinhchat { get; set; }

        public int? Trangthai { get; set; }

        public bool? Trangthaisd { get; set; }

        [StringLength(200)]
        public string Tentk { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        public virtual TDS_Dmloaitk TDS_Dmloaitk { get; set; }
    }
}
