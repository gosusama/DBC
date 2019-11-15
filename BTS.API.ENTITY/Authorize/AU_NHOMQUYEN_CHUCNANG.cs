using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_NHOMQUYEN_CHUCNANG")]
    public class AU_NHOMQUYEN_CHUCNANG : DataInfoEntity
    {

        [StringLength(50)]
        [Required]
        public string MANHOMQUYEN { get; set; }

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
