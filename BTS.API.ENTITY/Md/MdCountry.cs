using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BTS.API.ENTITY.Md
{
    /// <summary>
    /// Quốc gia
    /// </summary>
    [Table("MD_COUNTRY")]
    public class MdCountry : DataInfoEntity
    {

        /// <summary>
        /// Key
        /// </summary>
        [Column("COUNTRY_CODE")]
        [Required]
        [StringLength(50)]
        [Description("Mã quốc gia")]
        public string Code { get; set; }

        [Column("DESCRIPTION")]
        [Required]
        [StringLength(150)]
        [Description("Tên quốc gia")]
        public string Description { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}