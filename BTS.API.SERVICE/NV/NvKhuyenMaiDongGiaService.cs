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
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BTS.API.SERVICE.NV
{
    public interface INvKhuyenMaiDongGiaService : IDataInfoService<NvChuongTrinhKhuyenMai>
    {
        NvKhuyenMaiDongGiaVm.Dto CreateNewInstance();
        NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiDongGiaVm.Dto instance);
        NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiDongGiaVm.Dto instance);
        MemoryStream TemplateExcelKhuyenMaiDongGia();

    }
    public class NvKhuyenMaiDongGiaService : DataInfoServiceBase<NvChuongTrinhKhuyenMai>, INvKhuyenMaiDongGiaService
    {
        public NvKhuyenMaiDongGiaService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiDongGiaVm.Dto instance)
        {
            var item = AutoMapper.Mapper.Map<NvKhuyenMaiDongGiaVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaChuongTrinh = SaveCode();
            var result = Insert(item);
            var detailData = AutoMapper.Mapper.Map<List<NvKhuyenMaiDongGiaVm.DtoDetail>, List<NvChuongTrinhKhuyenMaiChiTiet>>(instance.DataDetails);

            detailData.ForEach(x => {
                x.Id = Guid.NewGuid().ToString();
                x.MaChuongTrinh = item.MaChuongTrinh;
            });
            UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiDongGiaVm.Dto instance)
        {
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvKhuyenMaiDongGiaVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            var detailData = Mapper.Map<List<NvKhuyenMaiDongGiaVm.DtoDetail>, List<NvChuongTrinhKhuyenMaiChiTiet>>(instance.DataDetails);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            {
                var detailCollection = UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(x => x.MaChuongTrinh == exsitItem.MaChuongTrinh);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                x.MaChuongTrinh = masterData.MaChuongTrinh;
                x.Id = Guid.NewGuid().ToString();
            });
            UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }

        public MemoryStream TemplateExcelKhuyenMaiDongGia()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells[1, 1].Value = "Số thứ tự"; worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 2].Value = "Mã hàng"; worksheet.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 3].Value = "Tên hàng"; worksheet.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 4].Value = "Đơn giá khuyến mại"; worksheet.Cells[1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(29, 126, 237));
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
        public NvKhuyenMaiDongGiaVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvKhuyenMaiDongGiaVm.Dto()
            {
                LoaiKhuyenMai = (int)LoaiKhuyenMai.DongGia,
                MaChuongTrinh = BuildCode(),
                TrangThai = 10,
                //NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
               // NgayDieuDong = DateTime.Now
            };
        }

    }
}
