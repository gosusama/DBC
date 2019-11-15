namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmloaichungtu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmloaichungtu()
        {
            TDS_Dmchungtu = new HashSet<TDS_Dmchungtu>();
        }

        [Key]
        [StringLength(20)]
        public string Maloaictu { get; set; }

        [StringLength(150)]
        public string Tenloaictu { get; set; }

        public int? Trangthai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmchungtu> TDS_Dmchungtu { get; set; }
    }
}
