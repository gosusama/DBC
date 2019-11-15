namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmvuviec
    {
        [Key]
        [StringLength(20)]
        public string Mavuviec { get; set; }

        [StringLength(250)]
        public string Tenvuviec { get; set; }

        [StringLength(20)]
        public string Manhomvuviec { get; set; }

        public int? Trangthai { get; set; }

        public int? Trangthaisudung { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }
    }
}
