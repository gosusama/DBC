using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE
{
    public class DataInfoDtoVm
    {
        public string Id { get; set; }
        public DateTime? ICreateDate { get; set; }
        public string ICreateBy { get; set; }
        public DateTime? IUpdateDate { get; set; }
        public string IUpdateBy { get; set; }
        public string IState { get; set; }
        public string UnitCode { get; set; }
    }
}
