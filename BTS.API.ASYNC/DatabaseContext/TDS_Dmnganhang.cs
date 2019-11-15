namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmnganhang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmnganhang()
        {
            TDS_Dmchinhanhnganhang = new HashSet<TDS_Dmchinhanhnganhang>();
        }

        [Key]
        [StringLength(20)]
        public string Manganhang { get; set; }

        [StringLength(250)]
        public string Tennganhang { get; set; }

        [StringLength(250)]
        public string Diachi { get; set; }

        [StringLength(20)]
        public string Dienthoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Tenviettat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmchinhanhnganhang> TDS_Dmchinhanhnganhang { get; set; }
    }
}
