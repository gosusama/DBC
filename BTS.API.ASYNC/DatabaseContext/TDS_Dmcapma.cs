namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TDS_Dmcapma
    {
        [StringLength(20)]
        public string Macappk { get; set; }

        
        
        [StringLength(20)]
        public string Madonvi { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Loaima { get; set; }
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Mastart { get; set; }

        public string GenerateNumber()
        {
            var result = "";
            int number;
            var length = Macappk.Length;
            if (int.TryParse(Macappk, out number))
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
            char newChar = Convert.ToChar(Macappk);
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
