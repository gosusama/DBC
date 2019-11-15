using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("MD_ID_BUILDER")]
    public class MdIdBuilder : EntityBase
    {
        [Key]
        [Column("ID")]
        [StringLength(50)]
        public string Id { get; set; }

        [Column("TYPE")]
        [StringLength(100)]
        public string Type { get; set; }

        [Column("CODE")]
        [StringLength(100)]
        public string Code { get; set; }

        [Column("CURRENT")]
        [StringLength(10)]
        public string Current { get; set; }

        [Column("UNITCODE")]
        [StringLength(50)]
        public string UnitCode { get; set; }

        [Column("NGAYTAO")]

        public Nullable<DateTime> NgayTao { get; set; }

        public string GenerateNumber()
        {
            var result = "";
            int number;
            var length = Current.Length;
            if (int.TryParse(Current, out number))
            {
                result = string.Format("{0}", number + 1);
                result = AddString(result, length, "0");
            }
            return result;
        }
        public string GenerateChar()
        {
            var result = "";
            int number;
            char newChar = Convert.ToChar(Current);
            newChar++;
            if ((int)newChar > 90)
            {
                return result;
            }     
            result = newChar.ToString();
            return result;
        }
        public string AddString(string input, int length, string character)
        {
            var result = input;
            while (result.Length < length)
            {
                result = string.Format("{0}{1}", character, result);
            }
            return result;
        }
    }
}