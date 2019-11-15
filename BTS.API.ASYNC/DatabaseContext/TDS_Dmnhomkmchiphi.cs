namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmnhomkmchiphi
    {
        [Key]
        [StringLength(20)]
        public string Manhomkmchiphi { get; set; }

        [Required]
        [StringLength(250)]
        public string Tennhomkmchiphi { get; set; }

        public int Trangthai { get; set; }

        public DateTime Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public int Trangthaisudung { get; set; }
    }
}
