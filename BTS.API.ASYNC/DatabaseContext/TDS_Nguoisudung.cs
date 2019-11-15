namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Nguoisudung
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Manhomquyen { get; set; }

        [Required]
        [StringLength(50)]
        public string Tendangnhap { get; set; }

        [Required]
        [StringLength(150)]
        public string Hovaten { get; set; }

        public int? Trangthai { get; set; }

        [Required]
        [StringLength(150)]
        public string Matkhau { get; set; }

        [StringLength(20)]
        public string Code1 { get; set; }

        [StringLength(20)]
        public string Code2 { get; set; }

        [StringLength(20)]
        public string Code3 { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Maphanhe { get; set; }
    }
}
