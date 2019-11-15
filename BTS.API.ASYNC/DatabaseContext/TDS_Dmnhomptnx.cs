namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmnhomptnx
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmnhomptnx()
        {
            TDS_Dmptnx = new HashSet<TDS_Dmptnx>();
        }

        [Key]
        [StringLength(20)]
        public string Manhomptnx { get; set; }

        [StringLength(200)]
        public string Tennhomptnx { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        public bool? Trangthai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmptnx> TDS_Dmptnx { get; set; }
    }
}
