namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmchungtu
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mactu { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(250)]
        public string Tenctu { get; set; }

        [Required]
        [StringLength(20)]
        public string Matkno { get; set; }

        [Required]
        [StringLength(20)]
        public string Matkco { get; set; }

        public bool? Trangthai { get; set; }

        [Required]
        [StringLength(20)]
        public string Matknodf { get; set; }

        [Required]
        [StringLength(20)]
        public string Matkcodf { get; set; }

        [Required]
        [StringLength(20)]
        public string Kyhieuct { get; set; }

        [Required]
        [StringLength(20)]
        public string Maloaictu { get; set; }

        public DateTime? Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        public virtual TDS_Dmloaichungtu TDS_Dmloaichungtu { get; set; }
    }
}
