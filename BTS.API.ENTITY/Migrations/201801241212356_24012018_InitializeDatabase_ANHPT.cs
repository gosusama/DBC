namespace BTS.API.ENTITY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24012018_InitializeDatabase_ANHPT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TBNETERP.AU_DONVI",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MADONVI = c.String(nullable: false, maxLength: 50),
                        MADONVICHA = c.String(maxLength: 50),
                        TENDONVI = c.String(maxLength: 150),
                        SODIENTHOAI = c.String(maxLength: 50),
                        DIACHI = c.String(maxLength: 200),
                        EMAIL = c.String(maxLength: 50),
                        MACUAHANG = c.String(maxLength: 50),
                        TENCUAHANG = c.String(maxLength: 200),
                        URL = c.String(maxLength: 300),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_LOG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        NGAY = c.DateTime(nullable: false),
                        TRANGTHAI = c.String(maxLength: 50),
                        THOIGIAN = c.String(maxLength: 50),
                        MAMAYBAN = c.String(maxLength: 50),
                        MANHANVIEN = c.String(maxLength: 50),
                        TINHTRANG = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_MENU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MENUIDCHA = c.String(maxLength: 100),
                        MENUID = c.String(maxLength: 100),
                        TITLE = c.String(maxLength: 200),
                        URL = c.String(maxLength: 500),
                        SORT = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_NGUOIDUNG_NHOMQUYEN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        USERNAME = c.String(nullable: false, maxLength: 50),
                        MANHOMQUYEN = c.String(nullable: false, maxLength: 50),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_NGUOIDUNG_QUYEN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        USERNAME = c.String(nullable: false, maxLength: 50),
                        MACHUCNANG = c.String(nullable: false, maxLength: 50),
                        XEM = c.Decimal(nullable: false, precision: 1, scale: 0),
                        THEM = c.Decimal(nullable: false, precision: 1, scale: 0),
                        SUA = c.Decimal(nullable: false, precision: 1, scale: 0),
                        XOA = c.Decimal(nullable: false, precision: 1, scale: 0),
                        DUYET = c.Decimal(nullable: false, precision: 1, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_NGUOIDUNG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        USERNAME = c.String(nullable: false, maxLength: 50),
                        PASSWORD = c.String(nullable: false, maxLength: 50),
                        MANHANVIEN = c.String(maxLength: 50),
                        TENNHANVIEN = c.String(maxLength: 200),
                        SODIENTHOAI = c.String(maxLength: 20),
                        SOCHUNGMINHTHU = c.String(maxLength: 20),
                        GIOITINH = c.Decimal(nullable: false, precision: 10, scale: 0),
                        MAPHONG = c.String(maxLength: 50),
                        CHUCVU = c.String(maxLength: 50),
                        LEVEL = c.Decimal(precision: 10, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_NHOMQUYEN_CHUCNANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MANHOMQUYEN = c.String(nullable: false, maxLength: 50),
                        MACHUCNANG = c.String(nullable: false, maxLength: 50),
                        XEM = c.Decimal(nullable: false, precision: 1, scale: 0),
                        THEM = c.Decimal(nullable: false, precision: 1, scale: 0),
                        SUA = c.Decimal(nullable: false, precision: 1, scale: 0),
                        XOA = c.Decimal(nullable: false, precision: 1, scale: 0),
                        DUYET = c.Decimal(nullable: false, precision: 1, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_NHOMQUYEN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MANHOMQUYEN = c.String(nullable: false, maxLength: 50),
                        TENNHOMQUYEN = c.String(maxLength: 100),
                        MOTA = c.String(maxLength: 200),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_THAMSOHETHONG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MA_THAMSO = c.String(maxLength: 50),
                        TEN_THAMSO = c.String(maxLength: 500),
                        GIATRI_THAMSO = c.Decimal(nullable: false, precision: 10, scale: 0),
                        KIEUDULIEU = c.Decimal(nullable: false, precision: 10, scale: 0),
                        IS_EDIT = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_CLIENT",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        SECRECT = c.String(nullable: false),
                        NAME = c.String(nullable: false, maxLength: 100),
                        APPLICATIONTYPE = c.Decimal(nullable: false, precision: 10, scale: 0),
                        ACTIVE = c.Decimal(nullable: false, precision: 1, scale: 0),
                        REFRESHTOKENLIFETIME = c.Decimal(nullable: false, precision: 10, scale: 0),
                        ALLOWEDORIGIN = c.String(maxLength: 100),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DCL_KHOASO",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAKHOASO = c.String(nullable: false, maxLength: 50),
                        NGAYKHOASO = c.DateTime(nullable: false),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        USER_KHOASO = c.String(maxLength: 150),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DCL_SODUCUOIKY",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAKHOASO = c.String(nullable: false, maxLength: 50),
                        TAIKHOAN = c.String(nullable: false, maxLength: 50),
                        SODU_NO_CUOIKY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SODU_CO_CUOIKY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SODU_CUOIKY = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.SOTONGHOP",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUNGTUPK = c.String(nullable: false, maxLength: 50),
                        MACHUNGTU = c.String(maxLength: 50),
                        NGAYCHUNGTU = c.DateTime(),
                        LOAICHUNGTU = c.String(nullable: false, maxLength: 50),
                        TKNO = c.String(maxLength: 50),
                        TKCO = c.String(maxLength: 50),
                        SOTIEN = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DOITUONGNO = c.String(maxLength: 50),
                        DOITUONGCO = c.String(maxLength: 50),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        INDEX = c.Decimal(nullable: false, precision: 10, scale: 0),
                        NOIDUNG = c.String(maxLength: 300),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DATA_DONGBO",
                c => new
                    {
                        MAVATTU = c.String(nullable: false, maxLength: 20),
                        MAMAYBAN = c.String(maxLength: 2000),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        NGAYDONGBO = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MAVATTU);
            
            CreateTable(
                "TBNETERP.DM_BOHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MABOHANG = c.String(nullable: false, maxLength: 50),
                        TENBOHANG = c.String(nullable: false, maxLength: 300),
                        NGAYCHUNGTU = c.DateTime(),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        GHICHU = c.String(maxLength: 500),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_BOHANGCHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MABOHANG = c.String(nullable: false, maxLength: 50),
                        MAHANG = c.String(nullable: false, maxLength: 50),
                        TENHANG = c.String(maxLength: 500),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        TYLECKLE = c.Decimal(precision: 18, scale: 2),
                        TYLECKBUON = c.Decimal(precision: 18, scale: 2),
                        TONGLE = c.Decimal(precision: 18, scale: 2),
                        DONGIA = c.Decimal(precision: 18, scale: 2),
                        TONGBUON = c.Decimal(precision: 18, scale: 2),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        GHICHU = c.String(maxLength: 500),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_CHIETKHAUKH",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHIETKHAU = c.String(nullable: false, maxLength: 50),
                        TIENTU = c.Decimal(precision: 18, scale: 2),
                        TIENDEN = c.Decimal(precision: 18, scale: 2),
                        TYLECHIETKHAU = c.Decimal(precision: 18, scale: 2),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_THANHPHO",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        CITY_ID = c.String(nullable: false, maxLength: 10),
                        CITY_NAME = c.String(nullable: false, maxLength: 200),
                        LEVEL = c.Decimal(precision: 10, scale: 0),
                        STATUS = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_COLOR",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACOLOR = c.String(nullable: false, maxLength: 50),
                        TENCOLOR = c.String(maxLength: 500),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_NGOAITE",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MANGOAITE = c.String(nullable: false, maxLength: 50),
                        TENNGOAITE = c.String(maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_KHACHHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAKH = c.String(maxLength: 50),
                        TENKH = c.String(maxLength: 500),
                        TENKHAC = c.String(maxLength: 500),
                        DIACHI = c.String(maxLength: 500),
                        TINHTHANHPHO = c.String(name: "TINH/THANHPHO", maxLength: 50),
                        QUANHUYEN = c.String(maxLength: 50),
                        MASOTHUE = c.String(maxLength: 50),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        DIENTHOAI = c.String(maxLength: 50),
                        CMTND = c.String(maxLength: 50),
                        EMAIL = c.String(maxLength: 100),
                        LOAIKHACHHANG = c.Decimal(precision: 10, scale: 0),
                        SODIEM = c.Decimal(precision: 18, scale: 2),
                        TIENNGUYENGIA = c.Decimal(precision: 18, scale: 2),
                        TIENSALE = c.Decimal(precision: 18, scale: 2),
                        TONGTIEN = c.Decimal(precision: 18, scale: 2),
                        MATHE = c.String(maxLength: 50),
                        NGAYCAPTHE = c.DateTime(),
                        NGAYHETHAN = c.DateTime(),
                        GHICHU = c.String(maxLength: 500),
                        HANGKHACHHANG = c.String(maxLength: 50),
                        HANGKHACHHANGCU = c.String(maxLength: 50),
                        NGAYSINH = c.DateTime(),
                        NGAYDACBIET = c.DateTime(),
                        QUENTHE = c.Decimal(precision: 10, scale: 0),
                        NGAYCHAMSOC = c.DateTime(),
                        ISCARE = c.Decimal(precision: 1, scale: 0),
                        TIENNGUYENGIA_CHAMSOC = c.Decimal(precision: 18, scale: 2),
                        TONGTIEN_CHAMSOC = c.Decimal(precision: 18, scale: 2),
                        GHICHUCU = c.String(maxLength: 1000),
                        NGAYMUAHANG = c.DateTime(),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_PHONGBAN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAPHONG = c.String(maxLength: 50),
                        TENPHONG = c.String(maxLength: 200),
                        THONGTINBOSUNG = c.String(maxLength: 1000),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_TINHHUYEN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        CITY_ID = c.String(nullable: false, maxLength: 10),
                        DISTRICTS_ID = c.String(nullable: false, maxLength: 10),
                        DISTRICTS_NAME = c.String(nullable: false, maxLength: 200),
                        LEVEL = c.Decimal(precision: 10, scale: 0),
                        STATUS = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_DONVITINH",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MADVT = c.String(nullable: false, maxLength: 50),
                        TENDVT = c.String(maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_HANGKHACHHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAHANGKH = c.String(nullable: false, maxLength: 50),
                        TENHANGKH = c.String(maxLength: 200),
                        SODIEM = c.Decimal(precision: 18, scale: 2),
                        TYLEGIAMGIASN = c.Decimal(precision: 18, scale: 2),
                        TYLEGIAMGIA = c.Decimal(precision: 18, scale: 2),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.MD_ID_BUILDER",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        TYPE = c.String(maxLength: 100),
                        CODE = c.String(maxLength: 100),
                        CURRENT = c.String(maxLength: 10),
                        UNITCODE = c.String(maxLength: 50),
                        NGAYTAO = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_VATTU_GIACA",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAVATTU = c.String(nullable: false, maxLength: 50),
                        MADONVI = c.String(nullable: false, maxLength: 50),
                        GIAVON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIAMUAVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIAMUA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIABANLE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIABANBUON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TY_LELAI_BUON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TY_LELAI_LE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MAVATVAO = c.String(maxLength: 50),
                        TYLE_VAT_VAO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MAVATRA = c.String(maxLength: 50),
                        TYLE_VAT_RA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIA_BANLE_VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIA_BANBUON_VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SOTONMAX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SOTONMIN = c.Decimal(nullable: false, precision: 18, scale: 2),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_VATTU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAVATTU = c.String(nullable: false, maxLength: 50),
                        TENVATTU = c.String(maxLength: 200),
                        TENVIETTAT = c.String(maxLength: 200),
                        DONVITINH = c.String(maxLength: 50),
                        MAKEHANG = c.String(maxLength: 50),
                        MALOAIVATTU = c.String(nullable: false, maxLength: 50),
                        MANHOMVATTU = c.String(maxLength: 50),
                        MABAOBI = c.String(maxLength: 50),
                        MAKHACHHANG = c.String(maxLength: 50),
                        MANCC = c.String(maxLength: 50),
                        CHIETKHAUNCC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIAMUA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIABANLE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIABANBUON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TYLELAIBUON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TYLELAILE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SOTONMAX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SOTONMIN = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BARCODE = c.String(maxLength: 2000),
                        MAKHAC = c.String(maxLength: 50),
                        MAVATVAO = c.String(maxLength: 50),
                        TYLE_VAT_VAO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MAVATRA = c.String(maxLength: 50),
                        TYLE_VAT_RA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIABANVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GIAVON = c.Decimal(precision: 18, scale: 2),
                        PTTINHGIA = c.String(maxLength: 200),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TKDOANHTHU = c.String(maxLength: 50),
                        TKGIAVON = c.String(maxLength: 50),
                        TKHANGHOA = c.String(maxLength: 50),
                        TKPHAITRA = c.String(maxLength: 50),
                        ITEMCODE = c.String(maxLength: 50),
                        KEKIEMKE = c.String(maxLength: 100),
                        MASIZE = c.String(maxLength: 50),
                        MACOLOR = c.String(maxLength: 50),
                        PATH_IMAGE = c.String(maxLength: 2000),
                        AVATAR = c.Binary(),
                        IMAGE = c.String(maxLength: 2000),
                        MACHA = c.String(maxLength: 50),
                        TITLE = c.String(maxLength: 2000),
                        TRANGTHAICON = c.Decimal(nullable: false, precision: 18, scale: 2),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_LOAIVATTU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MALOAIVATTU = c.String(nullable: false, maxLength: 50),
                        MACHA = c.String(maxLength: 50),
                        TENLOAIVT = c.String(nullable: false, maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_THEODOITIENTRINH",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        PROCESSCODE = c.String(maxLength: 50),
                        DESCRIPTION = c.String(maxLength: 300),
                        STATE = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_NHOMVATTU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MALOAIVATTU = c.String(nullable: false, maxLength: 50),
                        MANHOMVTU = c.String(nullable: false, maxLength: 50),
                        MACHA = c.String(maxLength: 50),
                        TENNHOMVT = c.String(maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_BAOBI",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MABAOBI = c.String(nullable: false, maxLength: 50),
                        TENBAOBI = c.String(nullable: false, maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        SOLUONG = c.Decimal(nullable: false, precision: 18, scale: 2),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_KYKETOAN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        KY = c.Decimal(nullable: false, precision: 10, scale: 0),
                        NAME = c.String(maxLength: 500),
                        TUNGAY = c.DateTime(nullable: false),
                        DENNGAY = c.DateTime(nullable: false),
                        NAM = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_MAYBANHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        CODE = c.String(maxLength: 50),
                        NAME = c.String(maxLength: 500),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_KEHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAKEHANG = c.String(nullable: false, maxLength: 50),
                        TENKEHANG = c.String(nullable: false, maxLength: 150),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_SIZE",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MASIZE = c.String(nullable: false, maxLength: 50),
                        TENSIZE = c.String(maxLength: 100),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_NHACUNGCAP",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MANCC = c.String(maxLength: 50),
                        TENNCC = c.String(maxLength: 500),
                        DIACHI = c.String(maxLength: 500),
                        TINHTHANHPHO = c.String(name: "TINH/THANHPHO", maxLength: 50),
                        MASOTHUE = c.String(maxLength: 50),
                        NGUOILIENHE = c.String(maxLength: 300),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        DIENTHOAI = c.String(maxLength: 50),
                        FAX = c.String(maxLength: 50),
                        CHUCVU = c.String(maxLength: 50),
                        EMAIL = c.String(maxLength: 100),
                        XUATXU = c.String(maxLength: 100),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_LOAILYDO",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MALYDO = c.String(nullable: false, maxLength: 50),
                        MACHA = c.String(maxLength: 50),
                        TENLYDO = c.String(maxLength: 300),
                        LOAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_KHO",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAKHO = c.String(maxLength: 50),
                        TENKHO = c.String(maxLength: 200),
                        MADONVI = c.String(maxLength: 50),
                        MACUAHANG = c.String(maxLength: 50),
                        TAIKHOANKT = c.String(maxLength: 50),
                        DIACHI = c.String(maxLength: 200),
                        THONGTINBOSUNG = c.String(maxLength: 1000),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_XUATXU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAXUATXU = c.String(nullable: false, maxLength: 50),
                        TENXUATXU = c.String(nullable: false, maxLength: 200),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        GHICHU = c.String(maxLength: 500),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NV_CT_KHUYENMAI_CHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUONGTRINH = c.String(nullable: false, maxLength: 50),
                        MAKHOXUAT = c.String(maxLength: 50),
                        MAKHOKHUYENMAI = c.String(maxLength: 50),
                        LOAICHUONGTRINH = c.Decimal(nullable: false, precision: 10, scale: 0),
                        MAHANG = c.String(nullable: false, maxLength: 50),
                        HANGKMCHINH = c.Decimal(precision: 1, scale: 0),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        TUGIO = c.String(maxLength: 50),
                        DENGIO = c.String(maxLength: 50),
                        MAHANG_KHUYENMAI = c.String(maxLength: 50),
                        SOLUONG_KHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        TYLEKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        GIATRIKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        TENHANG = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NV_CT_KHUYENMAI_HANGKM",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUONGTRINH = c.String(nullable: false, maxLength: 50),
                        MAKHOXUAT = c.String(maxLength: 50),
                        MAKHOKHUYENMAI = c.String(maxLength: 50),
                        LOAICHUONGTRINH = c.Decimal(nullable: false, precision: 10, scale: 0),
                        MAHANG = c.String(maxLength: 50),
                        TENHANG = c.String(maxLength: 200),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        TYLEKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        GIATRIKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NV_CT_KHUYENMAI",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUONGTRINH = c.String(nullable: false, maxLength: 50),
                        TUNGAY = c.DateTime(nullable: false),
                        DENNGAY = c.DateTime(nullable: false),
                        TUGIO = c.String(maxLength: 50),
                        DENGIO = c.String(maxLength: 50),
                        GIATRIKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        TYLEKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        MAKHOXUAT = c.String(maxLength: 50),
                        MAKHOKHUYENMAI = c.String(maxLength: 50),
                        LOAIKHUYENMAI = c.Decimal(precision: 10, scale: 0),
                        NOIDUNG = c.String(maxLength: 500),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TIENBATDAU = c.Decimal(precision: 18, scale: 2),
                        TIENKETTHUC = c.Decimal(precision: 18, scale: 2),
                        MAGIAMGIA = c.String(maxLength: 200),
                        DIENTHOAI = c.String(maxLength: 15),
                        TYLEBATDAU = c.Decimal(precision: 18, scale: 2),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        HANGKHACHHANG = c.String(maxLength: 50),
                        TRANGTHAISUDUNG = c.Decimal(precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVDATHANGCHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        SOPHIEU = c.String(nullable: false, maxLength: 50),
                        SOPHIEUPK = c.String(nullable: false, maxLength: 50),
                        MAHD = c.String(maxLength: 50),
                        MAHANG = c.String(maxLength: 50),
                        TENHANG = c.String(maxLength: 300),
                        MABAOBI = c.String(maxLength: 50),
                        DONVITINH = c.String(maxLength: 50),
                        BARCODE = c.String(maxLength: 2000),
                        SOLUONGBAO = c.Decimal(precision: 18, scale: 2),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        DONGIA = c.Decimal(precision: 18, scale: 2),
                        SOTONMAX = c.Decimal(precision: 18, scale: 2),
                        SOTONMIN = c.Decimal(precision: 18, scale: 2),
                        SOLUONGTON = c.Decimal(precision: 18, scale: 2),
                        LUONGQUYCACH = c.Decimal(precision: 18, scale: 2),
                        SOLUONGDUYET = c.Decimal(precision: 18, scale: 2),
                        SOLUONGBAODUYET = c.Decimal(precision: 18, scale: 2),
                        SOLUONGLEDUYET = c.Decimal(precision: 18, scale: 2),
                        DONGIADUYET = c.Decimal(precision: 18, scale: 2),
                        SOLUONGLE = c.Decimal(precision: 18, scale: 2),
                        THANHTIEN = c.Decimal(precision: 18, scale: 2),
                        GHICHU = c.String(maxLength: 500),
                        DONGIADEXUAT = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVDATHANG",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        SOPHIEU = c.String(nullable: false, maxLength: 50),
                        SOPHIEUPK = c.String(nullable: false, maxLength: 50),
                        LOAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        NGAY = c.DateTime(),
                        NGUOILAP = c.String(maxLength: 150),
                        MAKHACHHANG = c.String(maxLength: 50),
                        MANHANVIEN = c.String(maxLength: 50),
                        MADONVIDAT = c.String(maxLength: 50),
                        MANHACUNGCAP = c.String(maxLength: 50),
                        MAHD = c.String(maxLength: 50),
                        NOIDUNG = c.String(maxLength: 250),
                        THANHTIENTRUOCVAT = c.Decimal(precision: 18, scale: 2),
                        VAT = c.String(maxLength: 50),
                        TIENVAT = c.Decimal(precision: 18, scale: 2),
                        TIENCHIETKHAU = c.Decimal(precision: 18, scale: 2),
                        THANHTIENSAUVAT = c.Decimal(precision: 18, scale: 2),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TENNN = c.String(maxLength: 250),
                        SDTNN = c.String(maxLength: 50),
                        DIACHINN = c.String(maxLength: 250),
                        TRANGTHAITT = c.Decimal(precision: 10, scale: 0),
                        HINHTHUCTT = c.String(maxLength: 10),
                        TENNGH = c.String(maxLength: 250),
                        SDTNGH = c.String(maxLength: 50),
                        CMNDNGH = c.String(maxLength: 250),
                        ISBANBUON = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVHANGGDQUAY_ASYNCCLIENT",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAGDQUAYPK = c.String(nullable: false, maxLength: 50),
                        MAKHOHANG = c.String(nullable: false, maxLength: 50),
                        MADONVI = c.String(maxLength: 50),
                        MAVATTU = c.String(nullable: false, maxLength: 50),
                        BARCODE = c.String(maxLength: 2000),
                        TENDAYDU = c.String(maxLength: 300),
                        NGUOITAO = c.String(maxLength: 300),
                        MABOPK = c.String(maxLength: 50),
                        NGAYTAO = c.DateTime(),
                        NGAYPHATSINH = c.DateTime(),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        TTIENCOVAT = c.Decimal(precision: 18, scale: 2),
                        VATBAN = c.Decimal(precision: 18, scale: 2),
                        GIABANLECOVAT = c.Decimal(precision: 18, scale: 2),
                        MAKHACHHANG = c.String(maxLength: 50),
                        MAKEHANG = c.String(maxLength: 50),
                        MACHUONGTRINHKM = c.String(maxLength: 50),
                        LOAIKHUYENMAI = c.String(maxLength: 50),
                        TIENCHIETKHAU = c.Decimal(precision: 18, scale: 2),
                        TYLECHIETKHAU = c.Decimal(precision: 18, scale: 2),
                        TYLEKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        TIENKHUYENMAI = c.Decimal(precision: 18, scale: 2),
                        TYLEVOUCHER = c.Decimal(precision: 18, scale: 2),
                        TIENVOUCHER = c.Decimal(precision: 18, scale: 2),
                        TYLELAILE = c.Decimal(precision: 18, scale: 2),
                        GIAVON = c.Decimal(precision: 18, scale: 2),
                        ISBANAM = c.Decimal(precision: 10, scale: 0),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVGDQUAY_ASYNCCLIENT",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAGIAODICH = c.String(maxLength: 50),
                        MAGIAODICHQUAYPK = c.String(nullable: false, maxLength: 50),
                        MADONVI = c.String(maxLength: 50),
                        LOAIGIAODICH = c.Decimal(precision: 18, scale: 2),
                        NGAYTAO = c.DateTime(),
                        MANGUOITAO = c.String(maxLength: 300),
                        NGUOITAO = c.String(maxLength: 300),
                        MAQUAYBAN = c.String(maxLength: 300),
                        NGAYPHATSINH = c.DateTime(),
                        HINHTHUCTHANHTOAN = c.String(maxLength: 200),
                        MAVOUCHER = c.String(maxLength: 50),
                        TIENKHACHDUA = c.Decimal(precision: 18, scale: 2),
                        TIENVOUCHER = c.Decimal(precision: 18, scale: 2),
                        TIENTHEVIP = c.Decimal(precision: 18, scale: 2),
                        TIENTRALAI = c.Decimal(precision: 18, scale: 2),
                        TIENTHE = c.Decimal(precision: 18, scale: 2),
                        TIENCOD = c.Decimal(precision: 18, scale: 2),
                        TIENMAT = c.Decimal(precision: 18, scale: 2),
                        TTIENCOVAT = c.Decimal(precision: 18, scale: 2),
                        THOIGIAN = c.String(maxLength: 150),
                        MAKHACHHANG = c.String(maxLength: 50),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVKIEMKECHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAPHIEUKIEMKE = c.String(nullable: false, maxLength: 50),
                        MAVATTU = c.String(maxLength: 50),
                        BARCODE = c.String(maxLength: 2000),
                        TENVATTU = c.String(maxLength: 500),
                        MADONVI = c.String(maxLength: 50),
                        KHOKIEMKE = c.String(maxLength: 50),
                        SOPHIEUKIEMKE = c.String(maxLength: 50),
                        LOAIVATTUKK = c.String(maxLength: 50),
                        NHOMVATTUKK = c.String(maxLength: 50),
                        KEKIEMKE = c.String(maxLength: 50),
                        SOLUONGTONMAY = c.Decimal(precision: 18, scale: 2),
                        SOLUONGKIEMKE = c.Decimal(precision: 18, scale: 2),
                        SOLUONGCHENHLECH = c.Decimal(precision: 18, scale: 2),
                        TIENTONMAY = c.Decimal(precision: 18, scale: 2),
                        TIENKIEMKE = c.Decimal(precision: 18, scale: 2),
                        TIENCHENHLECH = c.Decimal(precision: 18, scale: 2),
                        GIAVON = c.Decimal(precision: 18, scale: 2),
                        GHICHU = c.String(),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.NVKIEMKE",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MAPHIEUKIEMKE = c.String(nullable: false, maxLength: 50),
                        MADONVI = c.String(maxLength: 50),
                        NGAYKIEMKE = c.DateTime(),
                        KHOKIEMKE = c.String(maxLength: 50),
                        SOPHIEUKIEMKE = c.String(maxLength: 50),
                        LOAIVATTUKK = c.String(maxLength: 50),
                        NHOMVATTUKK = c.String(maxLength: 50),
                        KEKIEMKE = c.String(maxLength: 50),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        NGUOITAO = c.String(maxLength: 200),
                        NGAYDUYETPHIEU = c.DateTime(),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.VATTUCHUNGTUCHITIET",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MACHUNGTU = c.String(maxLength: 50),
                        MACHUNGTUPK = c.String(nullable: false, maxLength: 50),
                        MAHANG = c.String(maxLength: 50),
                        TENHANG = c.String(maxLength: 300),
                        MABAOBI = c.String(maxLength: 50),
                        MAKHOANMUC = c.String(maxLength: 50),
                        MAKHACHHANG = c.String(maxLength: 50),
                        DONVITINH = c.String(maxLength: 50),
                        BARCODE = c.String(maxLength: 2000),
                        LUONGQUYCACH = c.Decimal(precision: 18, scale: 2),
                        SOLUONGLE = c.Decimal(precision: 18, scale: 2),
                        SOLUONGLECT = c.Decimal(precision: 18, scale: 2),
                        SOLUONGBAOCT = c.Decimal(precision: 18, scale: 2),
                        SOLUONGCT = c.Decimal(precision: 18, scale: 2),
                        SOLUONGBAO = c.Decimal(precision: 18, scale: 2),
                        SOLUONG = c.Decimal(precision: 18, scale: 2),
                        DONGIA = c.Decimal(precision: 18, scale: 2),
                        THANHTIEN = c.Decimal(precision: 18, scale: 2),
                        TIENGIAMGIA = c.Decimal(precision: 18, scale: 2),
                        GIAVON = c.Decimal(precision: 18, scale: 2),
                        INDEX = c.Decimal(precision: 10, scale: 0),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.VATTUCHUNGTU",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        LOAICHUNGTU = c.String(nullable: false, maxLength: 50),
                        MACHUNGTUPK = c.String(nullable: false, maxLength: 50),
                        MACHUNGTU = c.String(nullable: false, maxLength: 50),
                        NGAYCHUNGTU = c.DateTime(),
                        NGAYDUYETPHIEU = c.DateTime(),
                        MAKHOXUAT = c.String(maxLength: 50),
                        SOPHIEUDATHANG = c.String(maxLength: 50),
                        MAKHONHAP = c.String(maxLength: 50),
                        SOHOADON = c.String(maxLength: 50),
                        NGAYHOADON = c.DateTime(),
                        MADONVINHAN = c.String(maxLength: 50),
                        MADONVIXUAT = c.String(maxLength: 50),
                        LOAITAIKHOAN = c.String(maxLength: 50),
                        SOHD = c.String(maxLength: 50),
                        MAKHACHHANG = c.String(maxLength: 50),
                        MAHANG = c.String(maxLength: 50),
                        MASOTHUE = c.String(maxLength: 50),
                        NGUOIVANCHUYEN = c.String(maxLength: 300),
                        PHUONGTIENVANCHUYEN = c.String(maxLength: 300),
                        NOIDUNG = c.String(maxLength: 400),
                        MALYDOKHAC = c.String(maxLength: 50),
                        MAMAYBAN = c.String(maxLength: 50),
                        LENHDIEUDONG = c.String(maxLength: 400),
                        THANHTIENTRUOCVAT = c.Decimal(precision: 18, scale: 2),
                        TIENCHIETKHAU = c.Decimal(precision: 18, scale: 2),
                        TIENVAT = c.Decimal(precision: 18, scale: 2),
                        TONGTIENGIAMGIA = c.Decimal(precision: 18, scale: 2),
                        THANHTIENSAUVAT = c.Decimal(precision: 18, scale: 2),
                        VAT = c.String(maxLength: 50),
                        NGAYDIEUDONG = c.DateTime(),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        TRANGTHAITHANHTOAN = c.Decimal(precision: 10, scale: 0),
                        TIENMAT = c.Decimal(precision: 18, scale: 2),
                        TIENTHE = c.Decimal(precision: 18, scale: 2),
                        TIENCOD = c.Decimal(precision: 18, scale: 2),
                        MANHANVIEN = c.String(maxLength: 50),
                        TENNGH = c.String(maxLength: 250),
                        TENNN = c.String(maxLength: 250),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.AU_REFRESHTOKN",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        SUBJECT = c.String(nullable: false, maxLength: 50),
                        CLIENTID = c.String(nullable: false, maxLength: 50),
                        ISUSEDUTC = c.DateTime(nullable: false),
                        EXPIRESUTC = c.DateTime(nullable: false),
                        PROTECTEDTICKET = c.String(nullable: false),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "TBNETERP.DM_LOAITHUE",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        MALOAITHUE = c.String(nullable: false, maxLength: 50),
                        LOAITHUE = c.String(maxLength: 100),
                        TYGIA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TAIKHOANKT = c.String(maxLength: 50),
                        TRANGTHAI = c.Decimal(nullable: false, precision: 10, scale: 0),
                        I_CREATE_DATE = c.DateTime(),
                        I_CREATE_BY = c.String(maxLength: 50),
                        I_UPDATE_DATE = c.DateTime(),
                        I_UPDATE_BY = c.String(maxLength: 50),
                        I_STATE = c.String(maxLength: 50),
                        UNITCODE = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("TBNETERP.DM_LOAITHUE");
            DropTable("TBNETERP.AU_REFRESHTOKN");
            DropTable("TBNETERP.VATTUCHUNGTU");
            DropTable("TBNETERP.VATTUCHUNGTUCHITIET");
            DropTable("TBNETERP.NVKIEMKE");
            DropTable("TBNETERP.NVKIEMKECHITIET");
            DropTable("TBNETERP.NVGDQUAY_ASYNCCLIENT");
            DropTable("TBNETERP.NVHANGGDQUAY_ASYNCCLIENT");
            DropTable("TBNETERP.NVDATHANG");
            DropTable("TBNETERP.NVDATHANGCHITIET");
            DropTable("TBNETERP.NV_CT_KHUYENMAI");
            DropTable("TBNETERP.NV_CT_KHUYENMAI_HANGKM");
            DropTable("TBNETERP.NV_CT_KHUYENMAI_CHITIET");
            DropTable("TBNETERP.DM_XUATXU");
            DropTable("TBNETERP.DM_KHO");
            DropTable("TBNETERP.DM_LOAILYDO");
            DropTable("TBNETERP.DM_NHACUNGCAP");
            DropTable("TBNETERP.DM_SIZE");
            DropTable("TBNETERP.DM_KEHANG");
            DropTable("TBNETERP.DM_MAYBANHANG");
            DropTable("TBNETERP.DM_KYKETOAN");
            DropTable("TBNETERP.DM_BAOBI");
            DropTable("TBNETERP.DM_NHOMVATTU");
            DropTable("TBNETERP.DM_THEODOITIENTRINH");
            DropTable("TBNETERP.DM_LOAIVATTU");
            DropTable("TBNETERP.DM_VATTU");
            DropTable("TBNETERP.DM_VATTU_GIACA");
            DropTable("TBNETERP.MD_ID_BUILDER");
            DropTable("TBNETERP.DM_HANGKHACHHANG");
            DropTable("TBNETERP.DM_DONVITINH");
            DropTable("TBNETERP.DM_TINHHUYEN");
            DropTable("TBNETERP.DM_PHONGBAN");
            DropTable("TBNETERP.DM_KHACHHANG");
            DropTable("TBNETERP.DM_NGOAITE");
            DropTable("TBNETERP.DM_COLOR");
            DropTable("TBNETERP.DM_THANHPHO");
            DropTable("TBNETERP.DM_CHIETKHAUKH");
            DropTable("TBNETERP.DM_BOHANGCHITIET");
            DropTable("TBNETERP.DM_BOHANG");
            DropTable("TBNETERP.DATA_DONGBO");
            DropTable("TBNETERP.SOTONGHOP");
            DropTable("TBNETERP.DCL_SODUCUOIKY");
            DropTable("TBNETERP.DCL_KHOASO");
            DropTable("TBNETERP.AU_CLIENT");
            DropTable("TBNETERP.AU_THAMSOHETHONG");
            DropTable("TBNETERP.AU_NHOMQUYEN");
            DropTable("TBNETERP.AU_NHOMQUYEN_CHUCNANG");
            DropTable("TBNETERP.AU_NGUOIDUNG");
            DropTable("TBNETERP.AU_NGUOIDUNG_QUYEN");
            DropTable("TBNETERP.AU_NGUOIDUNG_NHOMQUYEN");
            DropTable("TBNETERP.AU_MENU");
            DropTable("TBNETERP.AU_LOG");
            DropTable("TBNETERP.AU_DONVI");
        }
    }
}
