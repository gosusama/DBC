using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_THAMSOHETHONG")]
    public class AU_THAMSOHETHONG : DataInfoEntity
    {
        [Column("MA_THAMSO")]
        [StringLength(50)]
        [Description("Mã tham số hệ thống")]
        public string MaThamSo { get; set; }

        [Column("TEN_THAMSO")]
        [StringLength(500)]
        [Description("Tên tham số hệ thống")]
        public string TenThamSo { get; set; }

        [Column("GIATRI_THAMSO")]
        [Description("Giá trị tham số hệ thống")]
        public int GiaTriThamSo { get; set; }

        [Column("KIEUDULIEU")]
        [Description("Kiểu dữ liệu tham số 0: number ; 1: character")]
        public int KieuDuLieu { get; set; }

        [Column("IS_EDIT")]
        [Description("Có được phép sửa tham số không")]
        public int IsEdit { get; set; }
    }
}
