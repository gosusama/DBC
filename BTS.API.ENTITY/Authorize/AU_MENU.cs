using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_MENU")]
    public class AU_MENU : DataInfoEntity
    {
        [Column("MENUIDCHA")]
        [StringLength(100)]
        public string MenuIdCha { get; set; }

        [Column("MENUID")]
        [StringLength(100)]
        public string MenuId { get; set; }

        [Column("TITLE")]
        [StringLength(200)]
        public string Title { get; set; }

        [Column("URL")]
        [StringLength(500)]
        public string Url { get; set; }

        [Column("SORT")]
        public int Sort { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        public List<AU_MENU> ChildrenMenu;
    }
}
