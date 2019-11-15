namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmnvkhachang
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ManhanvienKD { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Madonvi { get; set; }
    }
}
