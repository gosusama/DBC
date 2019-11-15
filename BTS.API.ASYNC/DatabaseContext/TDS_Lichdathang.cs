namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Lichdathang
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "date")]
        public DateTime Ngaydathang { get; set; }
    }
}
