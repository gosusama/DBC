namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmdonvitinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmdonvitinh()
        {
            TDS_Dmmathang = new HashSet<TDS_Dmmathang>();
        }

        [Key]
        [StringLength(20)]
        public string Madvtinh { get; set; }

        public DateTime? Ngaytao { get; set; }

        public DateTime? Ngayphatsinh { get; set; }

        [Required]
        [StringLength(50)]
        public string Donvile { get; set; }

        [Required]
        [StringLength(50)]
        public string Donvilon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmmathang> TDS_Dmmathang { get; set; }
    }
}
