using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.MD
{
    public class MdCustomerVm
    {

        public class Search : IDataSearch
        {
            public string MaKH { get; set; }
            public string TenKH { get; set; }
            public string TenKhac { get; set; }
            public string DiaChi { get; set; }
            public string TinhThanhPho { get; set; }
            public string QuanHuyen { get; set; }
            public string MaSoThue { get; set; }
            public int TrangThai { get; set; }
            public string DienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string Email { get; set; }
            public TypeCustomer? LoaiKhachHang { get; set; }
            public decimal? SoDiem { get; set; }
            public decimal? TienNguyenGia { get; set; }
            public decimal? TienSale { get; set; }
            public decimal? TongTien { get; set; }
            public string MaThe { get; set; }
            public DateTime? NgayCapThe { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public string GhiChu { get; set; }
            public string HangKhachHang { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdCustomer().MaKH);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdCustomer();

                if (!string.IsNullOrEmpty(this.MaKH))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKH),
                        Value = this.MaKH,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenKH))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenKH),
                        Value = this.TenKH,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DiaChi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DiaChi),
                        Value = this.DiaChi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TinhThanhPho))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TinhThanhPho),
                        Value = this.TinhThanhPho,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaSoThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaSoThue),
                        Value = this.MaSoThue,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DienThoai))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DienThoai),
                        Value = this.DienThoai,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Email))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Email),
                        Value = this.Email,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.QuanHuyen))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.QuanHuyen),
                        Value = this.QuanHuyen,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                MaKH = summary;
                TenKH = summary;
                DiaChi = summary;
                TinhThanhPho = summary;
                MaSoThue = summary;
                DienThoai = summary;
                Email = summary;

            }
        }
        public class CustomerDto : DataInfoDtoVm
        {
            public string MaKH { get; set; }
            public string TenKH { get; set; }
            public string TenKhac { get; set; }
            public string DiaChi { get; set; }
            public string TinhThanhPho { get; set; }
            public string MaSoThue { get; set; }
            public int TrangThai { get; set; }
            public string DienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string Email { get; set; }
            public TypeCustomer? LoaiKhachHang { get; set; }
            public decimal? SoDiem { get; set; }
            public decimal? TienNguyenGia { get; set; }
            public decimal? TienSale { get; set; }
            public decimal? TongTien { get; set; }
            public int? QuenThe { get; set; }
            public string MaThe { get; set; }
            public DateTime? NgayCapThe { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public string GhiChu { get; set; }
            public string HangKhachHang { get; set; }
            public string HangKhachHangCu { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; }
        }

        public class Dto : DataInfoDtoVm
        { 

            public string MaKH { get; set; }
            public string TenKH { get; set; }
            public string TenKhac { get; set; }
            public string DiaChi { get; set; }
            public string TinhThanhPho { get; set; }
            public string QuanHuyen { get; set; }
            public string MaSoThue { get; set; }
            public int TrangThai { get; set; }
            public string DienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string Email { get; set; }
            public TypeCustomer? LoaiKhachHang { get; set; }
            public decimal? SoDiem { get; set; }
            public decimal? TienNguyenGia { get; set; }
            public decimal? TienSale { get; set; }
            public decimal? TongTien { get; set; }
            public string MaThe { get; set; }
            public DateTime? NgayCapThe { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public string GhiChu { get; set; }
            public string HangKhachHang { get; set; }
            public string HangKhachHangCu { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; }
            public int QuenThe { get; set; }
            public DateTime? NgayChamSoc { get; set; }
            public bool IsCare { get; set; }
            public decimal TienNguyenGia_ChamSoc { get; set; }
            public decimal TongTien_ChamSoc { get; set; }
            public string GhiChuCu { get; set; }
            public DateTime? NgayMuaHang { get; set; }
            public bool IsGenCode { get; set; }
        }
    }

    public class TDS_KhachHangVM
    {

        public class Search : IDataSearch
        {
            public string Makhachhang { get; set; }
            public string Tenkhachhang { get; set; }
            public string Diachi { get; set; }
            public string Dienthoai { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public string Masothue { get; set; }
            //public string RolesId { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new TDS_Dmkhachhang().Tenkhachhang);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new TDS_Dmkhachhang();

                if (!string.IsNullOrEmpty(this.Makhachhang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Makhachhang),
                        Value = this.Makhachhang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Tenkhachhang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Tenkhachhang),
                        Value = this.Tenkhachhang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Diachi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Diachi),
                        Value = this.Diachi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Dienthoai))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Dienthoai),
                        Value = this.Dienthoai,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Fax))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Fax),
                        Value = this.Fax,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Email))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Email),
                        Value = this.Email,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Masothue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Masothue),
                        Value = this.Masothue,
                        Method = FilterMethod.Like
                    });
                }

                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                Makhachhang = summary;
                Tenkhachhang = summary;
                Diachi = summary;
                Dienthoai = summary;
                Fax = summary;
                Email = summary;

            }
        }
        public class Dto
        {
            public string Makhachhang { get; set; }
            public string Maloaikhach { get; set; }
            public string Tenkhachhang { get; set; }
            public string Diachi { get; set; }
            public string Dienthoai { get; set; }
            public string Dienthoai2 { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public string Masothue { get; set; }
            public string Mahopdong { get; set; }
            public DateTime? Ngayphatsinh { get; set; }
            public bool? Trangthai { get; set; }
            public decimal? Congnomax { get; set; }
            public string Matuyen { get; set; }
            public string Manganhang { get; set; }
            public string Machinhanh { get; set; }
            public string Sotaikhoannganhang { get; set; }
            public string Diachigiaohang { get; set; }
            public string Nguoigiaodich { get; set; }
            public decimal? Diem { get; set; }
            public decimal? Doanhso { get; set; }
            public string Mathe { get; set; }
        }

        
    }
}
