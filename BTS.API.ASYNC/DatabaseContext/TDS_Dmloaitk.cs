namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmloaitk
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmloaitk()
        {
            TDS_Dmtk = new HashSet<TDS_Dmtk>();
        }

        [Key]
        [StringLength(20)]
        public string Maloaitk { get; set; }

        [StringLength(200)]
        public string Tenloaitk { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmtk> TDS_Dmtk { get; set; }
    }
}
