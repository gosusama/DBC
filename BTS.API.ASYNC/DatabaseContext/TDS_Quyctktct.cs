namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Quyctktct
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mactktpk { get; set; }

        [StringLength(500)]
        public string Ghichu { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Matkno { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Matkco { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal Sotien { get; set; }

        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Mavuviec { get; set; }

        [StringLength(20)]
        public string Manhanviencongno { get; set; }

        [StringLength(20)]
        public string Manhanviengiaohang { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal Tienvat { get; set; }

        [StringLength(20)]
        public string Mavat { get; set; }

        [StringLength(20)]
        public string Makmchiphi { get; set; }
    }
}
