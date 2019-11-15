using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BTS.API.SERVICE.NV
{
    public interface INvKhuyenMaiTinhTienService : IDataInfoService<NvChuongTrinhKhuyenMai>
    {
        NvKhuyenMaiTinhTienVm.Dto CreateNewInstance();
        NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiTinhTienVm.Dto instance);
        NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiTinhTienVm.Dto instance);
        MemoryStream TemplateExcelKhuyenMaiTinhTien();
    }
    public class NvKhuyenMaiTinhTienService : DataInfoServiceBase<NvChuongTrinhKhuyenMai>, INvKhuyenMaiTinhTienService
    {
        public NvKhuyenMaiTinhTienService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public string BuildCode(TypeVoucher type = TypeVoucher.CTKM)
        {
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type.ToString(),
                    Code = type.ToString(),
                    Current = "0",
                };
            }
            var maChungTuGenerate = config.GenerateNumber();
            config.Current = maChungTuGenerate;
            result = string.Format("{0}", maChungTuGenerate);
            return result;
        }
        public string SaveCode(TypeVoucher type = TypeVoucher.CTKM)
        {
            var result = "";
            var strType = type.ToString();
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == strType).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Code = strType,
                    Current = "0",
                };
                config.Current = config.GenerateNumber();
                idRepo.Insert(config);
            }
            else
            {
                config.Current = config.GenerateNumber();
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}", config.Current);
            return result;
        }
        public NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiTinhTienVm.Dto instance)
        {
            NvChuongTrinhKhuyenMai item = AutoMapper.Mapper.Map<NvKhuyenMaiTinhTienVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaChuongTrinh = SaveCode();
            NvChuongTrinhKhuyenMai result = Insert(item);
            List<NvChuongTrinhKhuyenMaiChiTiet> detailData = AutoMapper.Mapper.Map<List<NvKhuyenMaiTinhTienVm.DtoDetail>, List<NvChuongTrinhKhuyenMaiChiTiet>>(instance.DataDetails);

            detailData.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChuongTrinh = item.MaChuongTrinh;
            });
            item.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            item.ICreateBy = GetClaimsPrincipal().Identity.Name;
            item.ICreateDate = DateTime.Now;
            UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiTinhTienVm.Dto instance)
        {
            string _unitCode = GetCurrentUnitCode();
            NvChuongTrinhKhuyenMai exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            NvChuongTrinhKhuyenMai masterData = Mapper.Map<NvKhuyenMaiTinhTienVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            List<NvChuongTrinhKhuyenMaiChiTiet> detailData = Mapper.Map<List<NvKhuyenMaiTinhTienVm.DtoDetail>, List<NvChuongTrinhKhuyenMaiChiTiet>>(instance.DataDetails);
            DbSet<MdMerchandise> merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            {
                IQueryable<NvChuongTrinhKhuyenMaiChiTiet> detailCollection = UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(x => x.MaChuongTrinh == exsitItem.MaChuongTrinh);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                x.MaChuongTrinh = masterData.MaChuongTrinh;
                x.Id = Guid.NewGuid().ToString();
            });
            masterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            masterData.IUpdateBy = GetClaimsPrincipal().Identity.Name;
            masterData.IUpdateDate = DateTime.Now;
            masterData.UnitCode = _unitCode;
            UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().InsertRange(detailData);
            NvChuongTrinhKhuyenMai result = Update(masterData);
            return result;
        }

        public MemoryStream TemplateExcelKhuyenMaiTinhTien()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells[1, 1].Value = "Số thứ tự"; worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 2].Value = "Mã hàng"; worksheet.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 3].Value = "Tên hàng"; worksheet.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 1, 1, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(29, 126, 237));
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 11));
                package.SaveAs(ms);
                return ms;
            }
        }

        protected override Expression<Func<NvChuongTrinhKhuyenMai, bool>> GetKeyFilter(NvChuongTrinhKhuyenMai instance)
        {
            return x => x.MaChuongTrinh == instance.MaChuongTrinh;
        }
        public NvKhuyenMaiTinhTienVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvKhuyenMaiTinhTienVm.Dto()
            {
                LoaiKhuyenMai = (int)LoaiKhuyenMai.TinhTien,
                MaChuongTrinh = BuildCode(),
                TrangThai = 10,
                //NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
                // NgayDieuDong = DateTime.Now
            };
        }

    }
}
