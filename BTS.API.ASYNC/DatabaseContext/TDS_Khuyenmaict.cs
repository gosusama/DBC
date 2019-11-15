namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Khuyenmaict
    {
        [Key]
        public int Itemid { get; set; }

        [StringLength(20)]
        public string Machuongtrinh { get; set; }

        [StringLength(20)]
        public string Masieuthiban { get; set; }

        [StringLength(20)]
        public string Masieuthikm { get; set; }

        [StringLength(20)]
        public string Mabohang { get; set; }

        public int? Soluongban { get; set; }

        public int? Soluongkm { get; set; }

        [StringLength(20)]
        public string Makhohangban { get; set; }

        [StringLength(20)]
        public string Makhohangkm { get; set; }

        public int? Soluongmaxinbin { get; set; }

        public decimal? Giatrikmmax { get; set; }

        public decimal? Giatrikmmin { get; set; }

        public int? Tilechietkhau { get; set; }

        public decimal? Tienchietkhau { get; set; }

        [StringLength(20)]
        public string Manganhhang { get; set; }

        [StringLength(20)]
        public string Manhomhang { get; set; }

        public virtual TDS_Khuyenmai TDS_Khuyenmai { get; set; }
    }
}
