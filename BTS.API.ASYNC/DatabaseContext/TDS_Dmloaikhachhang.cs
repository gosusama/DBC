namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmloaikhachhang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmloaikhachhang()
        {
            TDS_Dmkhachhang = new HashSet<TDS_Dmkhachhang>();
        }

        [Key]
        [StringLength(20)]
        public string Maloaikhach { get; set; }

        [StringLength(50)]
        public string Tenloaikhach { get; set; }

        [StringLength(200)]
        public string Ghichu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmkhachhang> TDS_Dmkhachhang { get; set; }
    }
}
