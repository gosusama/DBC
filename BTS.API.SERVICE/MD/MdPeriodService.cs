using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
   
    public interface IMdPeriodService : IDataInfoService<MdPeriod>
    {
        string GetKyKeToan(DateTime ngayChungTu);
        bool CreateNewPeriodByDay(int year);
        ProcessState CheckProcess(MdPeriod instance);
        MdPeriod InitializePeriod(MdPeriod instance);
        string GetPreTableName(MdPeriod period);
        bool UpDateGiaVon(MdPeriod instance);
        bool UpdatePrice(string UnitCode, DateTime FromDate, DateTime ToDate);
    }
    public class MdPeriodService : DataInfoServiceBase<MdPeriod>, IMdPeriodService
    {
        public MdPeriodService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdPeriod, bool>> GetKeyFilter(MdPeriod instance)
        {
            //var unitCode = GetCurrentUnitCode();
            return x => x.Period == instance.Period && x.Year == instance.Year && x.UnitCode ==instance.UnitCode;
            //return x => x.Period == instance.Period;
        }

        public string GetKyKeToan(DateTime ngayChungTu)
        {
            DateTime beginDay = new DateTime(ngayChungTu.Year, ngayChungTu.Month, ngayChungTu.Day,0,0,0);
            DateTime endDay = new DateTime(ngayChungTu.Year, ngayChungTu.Month, ngayChungTu.Day, 23, 59, 59);
           
            var kyKeToan = Repository.DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
            if (kyKeToan != null)
            {
                return ProcedureCollection.GetTableName(kyKeToan.Year, kyKeToan.Period);
            }
            else
            {
                var now = DateTime.Now;
                beginDay = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                endDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                kyKeToan = Repository.DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
                return ProcedureCollection.GetTableName(kyKeToan.Year, kyKeToan.Period);
            }
        }


        public bool CreateNewPeriodByDay(int year)
        {
            var unitCode = GetCurrentUnitCode();
            var havePeriod = Repository.DbSet.Any(x => x.Year == year && x.UnitCode == unitCode);
            if (havePeriod)
            {
                return false;
            }
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);
            int count = 0;
            while (startDate <= endDate)
            {
                count++;
                var item = new MdPeriod()
                {
                    ToDate = startDate.Date,
                    FromDate = startDate.Date,
                    Period = count,
                    Year = startDate.Year,
                    Name = string.Format("Kỳ {0}", count)
                };
                startDate = startDate.AddDays(1);
                Insert(item);
            }
            return true;
        }

        public ProcessState CheckProcess(MdPeriod instance)
        {
            var unitCode = GetCurrentUnitCode();
            ProcessState result = ProcessState.IsPending;
            string processName = CodeProcess.KHOASO.ToString();
            var processKhoaSo = UnitOfWork.Repository<MdMonitorProcess>().DbSet.FirstOrDefault(x => x.ProcessCode == processName && x.UnitCode == unitCode);
            if (processKhoaSo != null)
            {
                result = processKhoaSo.State;
            }
            return result;
        }
        public bool UpDateGiaVon(MdPeriod instance)
        {
            var tableName = instance.GetTableName();
            return ProcedureCollection.CapNhatGiaVonQuayGiaoDich(tableName, instance.UnitCode,instance.ToDate);
        }

        /// <summary>
        /// UpdatePrice
        /// </summary>
        /// <param name="UnitCode"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public bool UpdatePrice(string UnitCode, DateTime FromDate, DateTime ToDate)
        {
            return ProcedureCollection.UpdatePriceByDate(UnitCode, FromDate, ToDate);
        }

        public string GetPreTableName(MdPeriod period)
        {
            var preDate = period.ToDate.AddDays(-1);
          
            var prePeriod = Repository.DbSet.FirstOrDefault(x => x.ToDate == preDate && x.TrangThai == (int)ApprovalState.IsComplete);//Đối với hóa sổ kỳ = ngày
            string preTalbeName = null;
            if (prePeriod != null)
            {

                preTalbeName = prePeriod.GetTableName();
            }
            return preTalbeName;
        }
        public MdPeriod InitializePeriod(MdPeriod instance)
        {
            var unitCode = GetCurrentUnitCode();
            DateTime beginDay = new DateTime(instance.Year, instance.ToDate.Month, instance.ToDate.Day ,0,0,0);
            DateTime nowDay = new DateTime(instance.Year, instance.FromDate.Month, instance.FromDate.Day - 1, 0, 0, 0);
            //đổi trạng thái
            var kyKeToan = Repository.DbSet.Where(x => x.FromDate < beginDay && x.UnitCode == unitCode).ToList();
            if (kyKeToan.Count > 0)
            {
                foreach (var data in kyKeToan)
                {
                    data.TrangThai = 10;
                    var result = Update(data);
                }
            }
            //khóa sổ kỳ hiện tại
            var kyTruoc = Repository.DbSet.FirstOrDefault(x => x.FromDate == nowDay && x.UnitCode == unitCode);
            return kyTruoc;
        }
    }
}
