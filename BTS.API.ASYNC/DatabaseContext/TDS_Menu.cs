namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Menu
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Menuid { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(150)]
        public string Menuname { get; set; }

        [StringLength(20)]
        public string Menucha { get; set; }

        public int? Thutu { get; set; }

        [StringLength(30)]
        public string Formname { get; set; }

        public int? Loaimenu { get; set; }

        [Required]
        [StringLength(20)]
        public string Maphanhe { get; set; }

        [StringLength(20)]
        public string Thamso { get; set; }

        public int? Cap { get; set; }
    }
}
