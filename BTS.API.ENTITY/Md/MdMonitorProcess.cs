using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_THEODOITIENTRINH")]
    public class MdMonitorProcess : DataInfoEntity
    {
        [Column("PROCESSCODE")]
        [StringLength(50)]
        public string ProcessCode { get; set; }
        [Column("DESCRIPTION")]
        [StringLength(300)]
        public string Description { get; set; }
        [Column("STATE")]
        public ProcessState State { get; set; }
    }
}
