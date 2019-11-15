using BTS.API.ENTITY.DCL;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using BTS.API.ENTITY;
using System.Data.Common;
using Oracle.ManagedDataAccess.Types;
using System.IO;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BTS.API.SERVICE.NV;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;

namespace BTS.API.SERVICE.DCL
{
    public interface ITanSuatMuaHangService : IEntityService<NvGiaoDichQuay>
    {
        
    }

    public class TanSuatMuaHangService : EntityServiceBase<NvGiaoDichQuay>, ITanSuatMuaHangService
    {
        public TanSuatMuaHangService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

       
        private string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            var subStrAray = str.Split(',');
            int length = subStrAray.Length;
            string[] resultArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                resultArray[i] = "'" + subStrAray[i] + "'";
            }
            return String.Join(",", resultArray);
        }
        private string _convertToArrayConditionDetail(string str, List<InventoryExcel> result)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            var subStrAray = str.Split(',');
            int length = subStrAray.Length;
            string[] resultArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                InventoryExcel model = new InventoryExcel();
                resultArray[i] = "'" + subStrAray[i] + "'";
                model.Code = subStrAray[i];
                result.Add(model);
            }
            return String.Join(",", resultArray);
        }
        public string GetTableName(int year, int period)
        {
            return string.Format("XNT_{0}_KY_{1}", year, period);
        }

    }
}
