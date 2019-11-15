using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE
{
    public class DataDto
    {
        [StringLength(50)]
        public string Id { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public virtual DateTime? ICreateDate { get; set; }

        [StringLength(50)]
        public virtual string ICreateBy { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public virtual DateTime? IUpdateDate { get; set; }

        [StringLength(50)]
        public virtual string IUpdateBy { get; set; }

        [StringLength(50)]
        public virtual string IState { get; set; }

    }
}
