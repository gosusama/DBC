namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmloaikhohang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmloaikhohang()
        {
            TDS_Dmkhohang = new HashSet<TDS_Dmkhohang>();
        }

        [Key]
        [StringLength(20)]
        public string Maloaikho { get; set; }

        [StringLength(150)]
        public string Tenloaikho { get; set; }

        [StringLength(250)]
        public string Ghichu { get; set; }

        public int? Sokhomo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmkhohang> TDS_Dmkhohang { get; set; }
    }
}
