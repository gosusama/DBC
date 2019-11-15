using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_KYKETOAN")]
    public class MdPeriod : DataInfoEntity
    {
        [Column("KY")]
        public int Period { get; set; }
        [Column("NAME")]
        [StringLength(500)]
        public string Name { get; set; }
        [Column("TUNGAY")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        [Column("DENNGAY")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
        [Column("NAM")]
        public int Year { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        public string GetTableName()
        {
            return string.Format("XNT_{0}_KY_{1}", this.Year, this.Period);
        }
    }
}
