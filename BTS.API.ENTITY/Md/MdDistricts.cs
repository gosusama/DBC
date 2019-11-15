using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_TINHHUYEN")]
    public class MdDistricts : DataInfoEntity
    {
        [Column("CITY_ID")]
        [Required]
        [StringLength(10)]
        [Description("Mã thành phố")]
        public string CityId { get; set; }

        [Column("DISTRICTS_ID")]
        [Required]
        [StringLength(10)]
        [Description("Mã quận huyện")]
        public string DistrictsId { get; set; }

        [Column("DISTRICTS_NAME")]
        [Required]
        [StringLength(200)]
        [Description("Tên Quận huyện")]
        public string DistrictsName { get; set; }

        [Column("LEVEL")]
        public Nullable<int> Level { get; set; }

        [Column("STATUS")]
        public int Status { get; set; }
    }
}