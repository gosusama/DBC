using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_NGUOIDUNG_QUYEN")]
    public class AU_NGUOIDUNG_QUYEN : DataInfoEntity
    {

        [StringLength(50)]
        [Required]
        public string USERNAME { get; set; }

        [StringLength(50)]
        [Required]
        public string MACHUCNANG { get; set; }

        [Required]
        public bool XEM { get; set; }

        [Required]
        public bool THEM { get; set; }

        [Required]
        public bool SUA { get; set; }

        [Required]
        public bool XOA { get; set; }

        [Required]
        public bool DUYET { get; set; }

        [Required]
        public bool GIAMUA { get; set; }

        [Required]
        public bool GIABAN { get; set; }

        [Required]
        public bool GIAVON { get; set; }

        [Required]
        public bool TYLELAI { get; set; }

        [Required]
        public bool BANCHIETKHAU { get; set; }

        [Required]
        public bool BANBUON { get; set; }

        [Required]
        public bool BANTRALAI { get; set; }

        public int TRANGTHAI { get; set; }
    }
}
