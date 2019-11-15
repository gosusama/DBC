namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmptnx
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Maptnx { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Required]
        [StringLength(20)]
        public string Manhomptnx { get; set; }

        [StringLength(250)]
        public string Tenptnx { get; set; }

        [StringLength(50)]
        public string Tinhchat { get; set; }

        [StringLength(10)]
        public string Kyhieu { get; set; }

        [StringLength(20)]
        public string Matkno { get; set; }

        [StringLength(20)]
        public string Matkco { get; set; }

        [StringLength(20)]
        public string Matknhapkhauno { get; set; }

        [StringLength(20)]
        public string Matknhapkhauco { get; set; }

        [StringLength(20)]
        public string Matkthuedacbietno { get; set; }

        [StringLength(20)]
        public string Matkthuedacbietco { get; set; }

        [StringLength(20)]
        public string Matkthuegtgtno { get; set; }

        [StringLength(20)]
        public string Matkthuegtgtco { get; set; }

        [StringLength(20)]
        public string Matkchietkhauno { get; set; }

        [StringLength(20)]
        public string Matkchietkhauco { get; set; }

        [StringLength(20)]
        public string Matkgiamgiano { get; set; }

        [StringLength(20)]
        public string Matkgiamgiaco { get; set; }

        [StringLength(20)]
        public string Matklephino { get; set; }

        [StringLength(20)]
        public string Matklephico { get; set; }

        [StringLength(20)]
        public string Matkchiphino { get; set; }

        [StringLength(20)]
        public string Matkchiphico { get; set; }

        [StringLength(20)]
        public string Matkchiphikhacno { get; set; }

        [StringLength(20)]
        public string Matkchiphikhacco { get; set; }

        [StringLength(20)]
        public string Matkkhuyenmaino { get; set; }

        [StringLength(20)]
        public string Matkkhuyenmaico { get; set; }

        [StringLength(20)]
        public string Matkthuekhautruno { get; set; }

        [StringLength(20)]
        public string Matkthuekhautruco { get; set; }

        [StringLength(20)]
        public string Matkbanamno { get; set; }

        [StringLength(20)]
        public string Matkbanamco { get; set; }

        [StringLength(20)]
        public string Matkchietkhausaubanhangno { get; set; }

        [StringLength(20)]
        public string Matkchietkhausaubanhangco { get; set; }

        [StringLength(20)]
        public string Matknodf { get; set; }

        [StringLength(20)]
        public string Matkcodf { get; set; }

        [StringLength(20)]
        public string Matknhapkhaunodf { get; set; }

        [StringLength(20)]
        public string Matknhapkhaucodf { get; set; }

        [StringLength(20)]
        public string Matkthuedacbietnodf { get; set; }

        [StringLength(20)]
        public string Matkthuedacbietcodf { get; set; }

        [StringLength(20)]
        public string Matkthuegtgtnodf { get; set; }

        [StringLength(20)]
        public string Matkthuegtgtcodf { get; set; }

        [StringLength(20)]
        public string Matkchietkhaunodf { get; set; }

        [StringLength(20)]
        public string Matkchietkhaucodf { get; set; }

        [StringLength(20)]
        public string Matkgiamgianodf { get; set; }

        [StringLength(20)]
        public string Matkgiamgiacodf { get; set; }

        [StringLength(20)]
        public string Matklephinodf { get; set; }

        [StringLength(20)]
        public string Matklephicodf { get; set; }

        [StringLength(20)]
        public string Matkchiphinodf { get; set; }

        [StringLength(20)]
        public string Matkchiphicodf { get; set; }

        [StringLength(20)]
        public string Matkchiphikhacnodf { get; set; }

        [StringLength(20)]
        public string Matkchiphikhaccodf { get; set; }

        [StringLength(20)]
        public string Matkkhuyenmainodf { get; set; }

        [StringLength(20)]
        public string Matkkhuyenmaicodf { get; set; }

        [StringLength(20)]
        public string Matkthuekhautrunodf { get; set; }

        [StringLength(20)]
        public string Matkthuekhautrucodf { get; set; }

        [StringLength(20)]
        public string Matkbanamnodf { get; set; }

        [StringLength(20)]
        public string Matkbanamcodf { get; set; }

        [StringLength(20)]
        public string Matkchietkhausaubanhangnodf { get; set; }

        [StringLength(20)]
        public string Matkchietkhausaubanhangcodf { get; set; }

        public DateTime? Ngaytao { get; set; }

        [StringLength(20)]
        public string Manguoitao { get; set; }

        [StringLength(50)]
        public string Cttienchietkhau { get; set; }

        [Required]
        [StringLength(50)]
        public string Ctthanhtien { get; set; }

        [Required]
        [StringLength(50)]
        public string Ctthucdoanhthu { get; set; }

        [StringLength(20)]
        public string Matkvtu { get; set; }

        [StringLength(20)]
        public string Matkgiavon { get; set; }

        [StringLength(20)]
        public string Matkvtudf { get; set; }

        [StringLength(20)]
        public string Matkgiavondf { get; set; }

        public virtual TDS_Dmdonvi TDS_Dmdonvi { get; set; }

        public virtual TDS_Dmnhomptnx TDS_Dmnhomptnx { get; set; }
    }
}
