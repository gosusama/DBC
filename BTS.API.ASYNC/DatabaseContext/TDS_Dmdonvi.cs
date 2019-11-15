namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmdonvi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmdonvi()
        {
            TDS_Dathang = new HashSet<TDS_Dathang>();
            TDS_Dmchungtu = new HashSet<TDS_Dmchungtu>();
            TDS_Dmptnx = new HashSet<TDS_Dmptnx>();
            TDS_Giaodich = new HashSet<TDS_Giaodich>();
            TDS_Giaodichquay = new HashSet<TDS_Giaodichquay>();
            TDS_Taikhoanhachtoan = new HashSet<TDS_Taikhoanhachtoan>();
        }

        [Key]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [StringLength(20)]
        public string Madonvicha { get; set; }

        [Required]
        [StringLength(200)]
        public string Tendonvi { get; set; }

        [StringLength(20)]
        public string Dienthoai { get; set; }

        [StringLength(20)]
        public string Dienthoai2 { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(200)]
        public string Diachi { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Masothue { get; set; }

        public DateTime? Ngayhieuluc { get; set; }

        public int? Chedoketoan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dathang> TDS_Dathang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmchungtu> TDS_Dmchungtu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmptnx> TDS_Dmptnx { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodich> TDS_Giaodich { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichquay> TDS_Giaodichquay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Taikhoanhachtoan> TDS_Taikhoanhachtoan { get; set; }
    }
}
