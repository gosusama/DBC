using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_NGUOIDUNG_NHOMQUYEN")]
    public class AU_NGUOIDUNG_NHOMQUYEN : DataInfoEntity
    {

        [Required]
        [StringLength(50)]
        public string USERNAME { get; set; }

        [Required]
        [StringLength(50)]
        public string MANHOMQUYEN { get; set; }
    }
}

