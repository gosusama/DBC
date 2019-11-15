namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmmathang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TDS_Dmmathang()
        {
            TDS_Dathangct = new HashSet<TDS_Dathangct>();
            TDS_Dmbohangct = new HashSet<TDS_Dmbohangct>();
            TDS_Dmgiaban = new HashSet<TDS_Dmgiaban>();
            TDS_Giaodichct = new HashSet<TDS_Giaodichct>();
            TDS_Giaodichquayamct = new HashSet<TDS_Giaodichquayamct>();
            TDS_Giaodichquayct = new HashSet<TDS_Giaodichquayct>();
        }

        [Key]
        [StringLength(20)]
        public string Masieuthi { get; set; }

        [Required]
        [StringLength(20)]
        public string Manganh { get; set; }

        [Required]
        [StringLength(20)]
        public string Manhomhang { get; set; }

        [Required]
        [StringLength(20)]
        public string Makhachhang { get; set; }

        [StringLength(20)]
        public string Makehang { get; set; }

        [Required]
        [StringLength(20)]
        public string Madvtinh { get; set; }

        [StringLength(300)]
        public string Tendaydu { get; set; }

        [StringLength(100)]
        public string Tenviettat { get; set; }

        [StringLength(30)]
        public string Mahangcuancc { get; set; }

        [StringLength(20)]
        public string Mavatmua { get; set; }

        [StringLength(20)]
        public string Mavatban { get; set; }

        public decimal? Trietkhauncc { get; set; }

        public bool? Isshowweb { get; set; }

        public int? Trangthai { get; set; }

        public int? Trangthaikd { get; set; }
        public int? StateSync { get; set; } 

        public DateTime Ngaytao { get; set; }

        public DateTime Ngayphatsinh { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        [StringLength(20)]
        public string Itemcode { get; set; }

        public int Quycach { get; set; }

        [StringLength(2000)]
        public string Barcode { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dathangct> TDS_Dathangct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmbohangct> TDS_Dmbohangct { get; set; }

        public virtual TDS_Dmdonvitinh TDS_Dmdonvitinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Dmgiaban> TDS_Dmgiaban { get; set; }

        public virtual TDS_Dmnganhhang TDS_Dmnganhhang { get; set; }

        public virtual TDS_Dmnhomhang TDS_Dmnhomhang { get; set; }

        public virtual TDS_Dmvat TDS_Dmvat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichct> TDS_Giaodichct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichquayamct> TDS_Giaodichquayamct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDS_Giaodichquayct> TDS_Giaodichquayct { get; set; }
    }
}
