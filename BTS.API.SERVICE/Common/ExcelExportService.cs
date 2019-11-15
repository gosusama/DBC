using BTS.API.SERVICE.DCL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Common
{
    public class ExcelExportService : DataExportModelAbs
    {
        private ExcelPackage _package;
        public ExcelExportService(ExcelPackage package)
        {
            _package = package;
        }
        public ExcelPackage Package { get { return _package; } }
        public override void ExportData()
        {
            var stream = new MemoryStream(Package.GetAsByteArray());
        }

        public override void FormatData()
        {
            throw new NotImplementedException();
        }

        public override void ReadData()
        {
            throw new NotImplementedException();
        }
    }

    public class BaoCaoXuatNhapTon : ExcelExportService
    {
        private IList<InventoryExpImp> _data;
        
        public BaoCaoXuatNhapTon(IList<InventoryExpImp> data, ExcelPackage package) : base(package)
        {
            _data = data;
        }
        public override void ReadData()
        {
            if (_data == null || Package == null)
            {
                return;
            }
            using (Package)
            {
                var worksheet = Package.Workbook.Worksheets[1];
                int index = 0;
                for (int i = 0; i < _data.Count; i++)
                {
                    InventoryExpImp item = _data[i];
                    worksheet.Cells[index + 2, 1].Value = index + 1;
                    worksheet.Cells[index + 2, 2].Value = item.Code;
                    worksheet.Cells[index + 2, 3].Value = item.Name;
                    worksheet.Cells[index + 2, 4].Value = item.OpeningBalanceQuantity;
                    worksheet.Cells[index + 2, 5].Value = item.OpeningBalanceValue;
                    worksheet.Cells[index + 2, 6].Value = item.IncreaseQuantity;
                    worksheet.Cells[index + 2, 7].Value = item.IncreaseValue;
                    worksheet.Cells[index + 2, 8].Value = item.DecreaseQuantity;
                    worksheet.Cells[index + 2, 9].Value = item.DecreaseValue;
                    worksheet.Cells[index + 2, 10].Value = item.ClosingQuantity;
                    worksheet.Cells[index + 2, 11].Value = item.ClosingValue;
                }
            }
            base.ReadData();
        }
        public override void ExportData()
        {

            base.ExportData();
        }
    }
}
