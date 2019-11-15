namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Taikhoanhachtoan
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Matkno { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string Matkco { get; set; }

        public decimal? Sotien { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        public virtual TDS_Dmkhachhang TDS_Dmkhachhang { get; set; }

        public virtual TDS_Giaodich TDS_Giaodich { get; set; }

        public virtual TDS_Giaodichquay TDS_Giaodichquay { get; set; }
    }
}
