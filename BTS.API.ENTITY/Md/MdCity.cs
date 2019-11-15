using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_THANHPHO")]
    public class MdCity : DataInfoEntity
    {
        [Column("CITY_ID")]
        [Required]
        [StringLength(10)]
        [Description("Mã thành phố")]
        public string CityId { get; set; }

        [Column("CITY_NAME")]
        [Required]
        [StringLength(200)]
        [Description("Tên thành phố")]
        public string CityName { get; set; }

        [Column("LEVEL")]
        public Nullable<int> Level { get; set; }

        [Column("STATUS")]
        public int Status { get; set; }
    }
}