namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Kiemke
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Kiemke()
        {
            TDS_Kiemkect = new HashSet<TDS_Kiemkect>();
        }

        [Key]
        [StringLength(20)]
        public string Magiaodichpk { get; set; }

        [Required]
        [StringLength(20)]
        public string Maptnx { get; set; }

        [Required]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Makhohang { get; set; }

        [StringLength(200)]
        public string Manganhhang { get; set; }

        [StringLength(1000)]
        public string Manhomhang { get; set; }

        [StringLength(1000)]
        public string Makehang { get; set; }

        [StringLength(1000)]
        public string Mavtu { get; set; }

        public int Trangthai { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        public DateTime? Ngaytao { get; set; }

        [Required]
        [StringLength(20)]
        public string Manguoitao { get; set; }

        [StringLength(20)]
        public string Mavuviecthua { get; set; }

        [StringLength(20)]
        public string Mavuviecthieu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Kiemkect> TDS_Kiemkect { get; set; }
    }
}
