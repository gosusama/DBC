using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Common
{
    public abstract class  DataExportModelAbs
    {
        public abstract void ReadData();
        public abstract void FormatData();
        public abstract void ExportData();
        public void ExportFormatedData()
        {
            this.ReadData();
            this.FormatData();
            this.ExportData();
        }
    }
}
