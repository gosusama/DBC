namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmtaikhoanketchuyenct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Matkno { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Matkco { get; set; }

        [Required]
        [StringLength(5)]
        public string Tinhchat { get; set; }

        public int Trangthaisudung { get; set; }

        public int Thutu { get; set; }

        [StringLength(250)]
        public string Ghichu { get; set; }
    }
}
