using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_VATTU_GIACA")]
    public class MdMerchandisePrice : DataInfoEntity
    {
        [Column("MAVATTU")]
        [StringLength(50)]
        [Required]
        public string MaVatTu { get; set; }
        [Column("MADONVI")]
        [StringLength(50)]
        [Required]
        public string MaDonVi { get; set; }
        [Column("GIAVON")]
        public decimal GiaVon { get; set; }
        [Column("GIAMUAVAT")]
        public decimal GiaMuaVat { get; set; }
        [Column("GIAMUA")]
        public decimal GiaMua { get; set; }
        [Column("GIABANLE")]
        public decimal GiaBanLe { get; set; }
        [Column("GIABANBUON")]
        public decimal GiaBanBuon { get; set; }
        [Column("TY_LELAI_BUON")]
        public decimal TyLeLaiBuon { get; set; }
        [Column("TY_LELAI_LE")]
        public decimal TyLeLaiLe { get; set; }
        [Column("MAVATVAO")]
        [StringLength(50)]
        public string MaVatVao { get; set; }

        [Column("TYLE_VAT_VAO")]
        public decimal TyLeVatVao { get; set; }
        [Column("MAVATRA")]
        [StringLength(50)]
        public string MaVatRa { get; set; }
        [Column("TYLE_VAT_RA")]
        public decimal TyLeVatRa { get; set; }
        [Column("GIA_BANLE_VAT")]
        public decimal GiaBanLeVat { get; set; }
        [Column("GIA_BANBUON_VAT")]
        public decimal GiaBanBuonVat { get; set; }
        [Column("SOTONMAX")]
        public decimal SoTonMax { get; set; }
        [Column("SOTONMIN")]
        public decimal SoTonMin { get; set; }
    }
}
