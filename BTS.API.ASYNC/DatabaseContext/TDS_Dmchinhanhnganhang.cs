namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmchinhanhnganhang
    {
        [Key]
        [StringLength(20)]
        public string Machinhanh { get; set; }

        [StringLength(20)]
        public string Manganhang { get; set; }

        [StringLength(250)]
        public string Tenchinhanh { get; set; }

        [StringLength(250)]
        public string Diachi { get; set; }

        [StringLength(20)]
        public string Dienthoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public virtual TDS_Dmnganhang TDS_Dmnganhang { get; set; }
    }
}
