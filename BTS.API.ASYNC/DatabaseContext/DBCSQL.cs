namespace BTS.API.ASYNC.DatabaseContext
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBCSQL : DbContext
    {
        public DBCSQL()
            : base("name=DBCSQL")
        {
        }

        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TDS_Banbuon> TDS_Banbuon { get; set; }
        public virtual DbSet<TDS_Barcode> TDS_Barcode { get; set; }
        public virtual DbSet<TDS_Dathang> TDS_Dathang { get; set; }
        public virtual DbSet<TDS_Dathangct> TDS_Dathangct { get; set; }
        public virtual DbSet<TDS_Dathangtudong> TDS_Dathangtudong { get; set; }
        public virtual DbSet<TDS_Dmbohang> TDS_Dmbohang { get; set; }
        public virtual DbSet<TDS_Dmbohangct> TDS_Dmbohangct { get; set; }
        public virtual DbSet<TDS_Dmchinhanhnganhang> TDS_Dmchinhanhnganhang { get; set; }
        public virtual DbSet<TDS_Dmchungtu> TDS_Dmchungtu { get; set; }
        public virtual DbSet<TDS_Dmdonvi> TDS_Dmdonvi { get; set; }
        public virtual DbSet<TDS_Dmdonvitinh> TDS_Dmdonvitinh { get; set; }
        public virtual DbSet<TDS_Dmgiaban> TDS_Dmgiaban { get; set; }
        public virtual DbSet<TDS_Dmgiagiaodich> TDS_Dmgiagiaodich { get; set; }
        public virtual DbSet<TDS_Dmhopdong> TDS_Dmhopdong { get; set; }
        public virtual DbSet<TDS_Dmkehang> TDS_Dmkehang { get; set; }
        public virtual DbSet<TDS_Dmkhachhang> TDS_Dmkhachhang { get; set; }
        public virtual DbSet<TDS_Dmkhachhangtt> TDS_Dmkhachhangtt { get; set; }
        public virtual DbSet<TDS_Dmkhohang> TDS_Dmkhohang { get; set; }
        public virtual DbSet<TDS_Dmkmchiphi> TDS_Dmkmchiphi { get; set; }
        public virtual DbSet<TDS_Dmloaichungtu> TDS_Dmloaichungtu { get; set; }
        public virtual DbSet<TDS_Dmloaikhachhang> TDS_Dmloaikhachhang { get; set; }
        public virtual DbSet<TDS_Dmloaikhohang> TDS_Dmloaikhohang { get; set; }
        public virtual DbSet<TDS_Dmloaitk> TDS_Dmloaitk { get; set; }
        public virtual DbSet<TDS_Dmmathang> TDS_Dmmathang { get; set; }
        public virtual DbSet<TDS_Dmmaxmindieuchuyen> TDS_Dmmaxmindieuchuyen { get; set; }
        public virtual DbSet<TDS_Dmmotasanpham> TDS_Dmmotasanpham { get; set; }
        public virtual DbSet<TDS_Dmmotasanphamchitiet> TDS_Dmmotasanphamchitiet { get; set; }
        public virtual DbSet<TDS_Dmnganhang> TDS_Dmnganhang { get; set; }
        public virtual DbSet<TDS_Dmnganhhang> TDS_Dmnganhhang { get; set; }
        public virtual DbSet<TDS_Dmnhomhang> TDS_Dmnhomhang { get; set; }
        public virtual DbSet<TDS_Dmnhomkmchiphi> TDS_Dmnhomkmchiphi { get; set; }
        public virtual DbSet<TDS_Dmnhomptnx> TDS_Dmnhomptnx { get; set; }
        public virtual DbSet<TDS_Dmnhomvuviec> TDS_Dmnhomvuviec { get; set; }
        public virtual DbSet<TDS_Dmnvkhachang> TDS_Dmnvkhachang { get; set; }
        public virtual DbSet<TDS_Dmptnx> TDS_Dmptnx { get; set; }
        public virtual DbSet<TDS_Dmquayhang> TDS_Dmquayhang { get; set; }
        public virtual DbSet<TDS_Dmtaikhoanketchuyen> TDS_Dmtaikhoanketchuyen { get; set; }
        public virtual DbSet<TDS_Dmtaikhoanketchuyenct> TDS_Dmtaikhoanketchuyenct { get; set; }
        public virtual DbSet<TDS_Dmtk> TDS_Dmtk { get; set; }
        public virtual DbSet<TDS_Dmtuyenduong> TDS_Dmtuyenduong { get; set; }
        public virtual DbSet<TDS_Dmvat> TDS_Dmvat { get; set; }
        public virtual DbSet<TDS_Dmvuviec> TDS_Dmvuviec { get; set; }
        public virtual DbSet<TDS_Giaodich> TDS_Giaodich { get; set; }
        public virtual DbSet<TDS_Giaodichct> TDS_Giaodichct { get; set; }
        public virtual DbSet<TDS_Giaodichdhthanhvien> TDS_Giaodichdhthanhvien { get; set; }
        public virtual DbSet<TDS_Giaodichdhthanhvienct> TDS_Giaodichdhthanhvienct { get; set; }
        public virtual DbSet<TDS_Giaodichquay> TDS_Giaodichquay { get; set; }
        public virtual DbSet<TDS_Giaodichquayamct> TDS_Giaodichquayamct { get; set; }
        public virtual DbSet<TDS_Giaodichquayct> TDS_Giaodichquayct { get; set; }
        public virtual DbSet<TDS_Giaodichtondau> TDS_Giaodichtondau { get; set; }
        public virtual DbSet<TDS_Giaodichtondauct> TDS_Giaodichtondauct { get; set; }
        public virtual DbSet<TDS_KhohangNhomhang> TDS_KhohangNhomhang { get; set; }
        public virtual DbSet<TDS_Khuyenmai> TDS_Khuyenmai { get; set; }
        public virtual DbSet<TDS_Khuyenmaict> TDS_Khuyenmaict { get; set; }
        public virtual DbSet<TDS_Kiemke> TDS_Kiemke { get; set; }
        public virtual DbSet<TDS_Kiemkect> TDS_Kiemkect { get; set; }
        public virtual DbSet<TDS_Lichdathang> TDS_Lichdathang { get; set; }
        public virtual DbSet<TDS_Lichxuathang> TDS_Lichxuathang { get; set; }
        public virtual DbSet<TDS_Maxmindathang> TDS_Maxmindathang { get; set; }
        public virtual DbSet<TDS_Menu> TDS_Menu { get; set; }
        public virtual DbSet<TDS_Menunhomquyen> TDS_Menunhomquyen { get; set; }
        public virtual DbSet<TDS_Nguoisudung> TDS_Nguoisudung { get; set; }
        public virtual DbSet<TDS_Nhomquyen> TDS_Nhomquyen { get; set; }
        public virtual DbSet<TDS_Nhomquyenphu> TDS_Nhomquyenphu { get; set; }
        public virtual DbSet<TDS_Quyctkt> TDS_Quyctkt { get; set; }
        public virtual DbSet<TDS_Sodutaikhoan> TDS_Sodutaikhoan { get; set; }
        public virtual DbSet<TDS_Sodutaikhoanct> TDS_Sodutaikhoanct { get; set; }
        public virtual DbSet<TDS_SXDathang> TDS_SXDathang { get; set; }
        public virtual DbSet<TDS_SXDathangct> TDS_SXDathangct { get; set; }
        public virtual DbSet<TDS_SXDmCTdinhmuc> TDS_SXDmCTdinhmuc { get; set; }
        public virtual DbSet<TDS_SXDmCTDinhmucct> TDS_SXDmCTDinhmucct { get; set; }
        public virtual DbSet<TDS_SXDmmaysanxuat> TDS_SXDmmaysanxuat { get; set; }
        public virtual DbSet<TDS_SXDmNangSuatNV> TDS_SXDmNangSuatNV { get; set; }
        public virtual DbSet<TDS_SXLenhsx> TDS_SXLenhsx { get; set; }
        public virtual DbSet<TDS_SXLenhsxct> TDS_SXLenhsxct { get; set; }
        public virtual DbSet<TDS_Taikhoanhachtoan> TDS_Taikhoanhachtoan { get; set; }
        public virtual DbSet<TDS_Thamsohethong> TDS_Thamsohethong { get; set; }
        public virtual DbSet<TDS_Thuachitheogd> TDS_Thuachitheogd { get; set; }
        public virtual DbSet<TDS_Congno> TDS_Congno { get; set; }
        public virtual DbSet<TDS_Ctugoc> TDS_Ctugoc { get; set; }
        public virtual DbSet<TDS_Dmcapma> TDS_Dmcapma { get; set; }
        public virtual DbSet<TDS_Lichsuthaydoigia> TDS_Lichsuthaydoigia { get; set; }
        public virtual DbSet<TDS_Log> TDS_Log { get; set; }
        public virtual DbSet<TDS_Quyctktct> TDS_Quyctktct { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TDS_Banbuon>()
                .Property(e => e.Tiendathanhtoan)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Banbuon>()
                .Property(e => e.Sotienno)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Banbuon>()
                .Property(e => e.Tongtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Barcode>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Manhacungcap)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .Property(e => e.Manganh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathang>()
                .HasMany(e => e.TDS_Dathangct)
                .WithRequired(e => e.TDS_Dathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Dongia)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Giabanlecovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Giabanbuoncovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Toncuoikygt)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Tondaukygt)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Nhaptkgt)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangct>()
                .Property(e => e.Xuattkgt)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dathangtudong>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathangtudong>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathangtudong>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dathangtudong>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmbohang>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmbohang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmbohang>()
                .HasMany(e => e.TDS_Dmbohangct)
                .WithRequired(e => e.TDS_Dmbohang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Tylechietkhaule)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Tylechietkhaubuon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Tongtienbanbuon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmbohangct>()
                .Property(e => e.Tongtienbanle)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmchinhanhnganhang>()
                .Property(e => e.Machinhanh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchinhanhnganhang>()
                .Property(e => e.Manganhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchinhanhnganhang>()
                .Property(e => e.Dienthoai)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Mactu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Matknodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Matkcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Maloaictu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmchungtu>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Madonvicha)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Dienthoai)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Dienthoai2)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Fax)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .Property(e => e.Masothue)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Dathang)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Dmchungtu)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Dmptnx)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Giaodich)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Giaodichquay)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvi>()
                .HasMany(e => e.TDS_Taikhoanhachtoan)
                .WithRequired(e => e.TDS_Dmdonvi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmdonvitinh>()
                .Property(e => e.Madvtinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmdonvitinh>()
                .HasMany(e => e.TDS_Dmmathang)
                .WithRequired(e => e.TDS_Dmdonvitinh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giabanlecovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giabanbuoncovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giabanlechuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giabanbuonchuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giamuacovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Giamuachuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Tylelaibuon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiaban>()
                .Property(e => e.Tylelaile)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Giabanlecovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Giabanbuoncovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Giabanlechuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Giabanbuonchuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Tylelaibuon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmgiagiaodich>()
                .Property(e => e.Tylelaile)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmhopdong>()
                .Property(e => e.Mahopdong)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmhopdong>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkehang>()
                .Property(e => e.Makehang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkehang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Maloaikhach)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Mahopdong)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Congnomax)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Matuyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Manganhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Machinhanh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Sotaikhoannganhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Diem)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Doanhso)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .Property(e => e.Mathe)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .HasMany(e => e.TDS_Dathang)
                .WithRequired(e => e.TDS_Dmkhachhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmkhachhang>()
                .HasMany(e => e.TDS_Taikhoanhachtoan)
                .WithRequired(e => e.TDS_Dmkhachhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmkhachhangtt>()
                .Property(e => e.Diemmin)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhachhangtt>()
                .Property(e => e.Diemmax)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhachhangtt>()
                .Property(e => e.Doanhsomin)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhachhangtt>()
                .Property(e => e.Doanhsomax)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmkhohang>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhohang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhohang>()
                .Property(e => e.Maloaikho)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkhohang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkmchiphi>()
                .Property(e => e.Makmchiphi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkmchiphi>()
                .Property(e => e.Manhomkmchiphi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmkmchiphi>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmloaichungtu>()
                .Property(e => e.Maloaictu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmloaichungtu>()
                .HasMany(e => e.TDS_Dmchungtu)
                .WithRequired(e => e.TDS_Dmloaichungtu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmloaikhachhang>()
                .Property(e => e.Maloaikhach)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmloaikhachhang>()
                .HasMany(e => e.TDS_Dmkhachhang)
                .WithRequired(e => e.TDS_Dmloaikhachhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmloaikhohang>()
                .Property(e => e.Maloaikho)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmloaikhohang>()
                .HasMany(e => e.TDS_Dmkhohang)
                .WithRequired(e => e.TDS_Dmloaikhohang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmloaitk>()
                .Property(e => e.Maloaitk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmloaitk>()
                .HasMany(e => e.TDS_Dmtk)
                .WithRequired(e => e.TDS_Dmloaitk)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Manganh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Makehang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Madvtinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Mahangcuancc)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Mavatmua)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Mavatban)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Trietkhauncc)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .Property(e => e.Itemcode)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Dathangct)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Dmbohangct)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Dmgiaban)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Giaodichct)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Giaodichquayamct)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmathang>()
                .HasMany(e => e.TDS_Giaodichquayct)
                .WithRequired(e => e.TDS_Dmmathang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmmaxmindieuchuyen>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmaxmindieuchuyen>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmaxmindieuchuyen>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmotasanpham>()
                .Property(e => e.Mamota)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmotasanphamchitiet>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmmotasanphamchitiet>()
                .Property(e => e.Mamota)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnganhang>()
                .Property(e => e.Manganhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnganhang>()
                .Property(e => e.Dienthoai)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnganhhang>()
                .Property(e => e.Manganh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnganhhang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnganhhang>()
                .HasMany(e => e.TDS_Dmmathang)
                .WithRequired(e => e.TDS_Dmnganhhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmnhomhang>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomhang>()
                .Property(e => e.Manganh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomhang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomhang>()
                .HasMany(e => e.TDS_Dmmathang)
                .WithRequired(e => e.TDS_Dmnhomhang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmnhomkmchiphi>()
                .Property(e => e.Manhomkmchiphi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomkmchiphi>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomptnx>()
                .Property(e => e.Manhomptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomptnx>()
                .HasMany(e => e.TDS_Dmptnx)
                .WithRequired(e => e.TDS_Dmnhomptnx)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Dmnhomvuviec>()
                .Property(e => e.Manhomvuviec)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnhomvuviec>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnvkhachang>()
                .Property(e => e.ManhanvienKD)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnvkhachang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmnvkhachang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Manhomptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Tinhchat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matknhapkhauno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matknhapkhauco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuedacbietno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuedacbietco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuegtgtno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuegtgtco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhauno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhauco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkgiamgiano)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkgiamgiaco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matklephino)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matklephico)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphino)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphico)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphikhacno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphikhacco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkkhuyenmaino)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkkhuyenmaico)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuekhautruno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuekhautruco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkbanamno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkbanamco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhausaubanhangno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhausaubanhangco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matknodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matknhapkhaunodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matknhapkhaucodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuedacbietnodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuedacbietcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuegtgtnodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuegtgtcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhaunodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhaucodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkgiamgianodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkgiamgiacodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matklephinodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matklephicodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphinodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphicodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphikhacnodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchiphikhaccodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkkhuyenmainodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkkhuyenmaicodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuekhautrunodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkthuekhautrucodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkbanamnodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkbanamcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhausaubanhangnodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Matkchietkhausaubanhangcodf)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Cttienchietkhau)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Ctthanhtien)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmptnx>()
                .Property(e => e.Ctthucdoanhthu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmquayhang>()
                .Property(e => e.Maquay)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmquayhang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmquayhang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmquayhang>()
                .Property(e => e.Tongtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyen>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyen>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyen>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyenct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyenct>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyenct>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtaikhoanketchuyenct>()
                .Property(e => e.Tinhchat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtk>()
                .Property(e => e.Matk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtk>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtk>()
                .Property(e => e.Maloaitk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtk>()
                .Property(e => e.Matkcha)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtk>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmtuyenduong>()
                .Property(e => e.Matuyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvat>()
                .Property(e => e.Mavat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvat>()
                .Property(e => e.Congthuc)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvat>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvat>()
                .Property(e => e.Doanhso)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvat>()
                .HasMany(e => e.TDS_Dmmathang)
                .WithOptional(e => e.TDS_Dmvat)
                .HasForeignKey(e => e.Mavatmua);

            modelBuilder.Entity<TDS_Dmvuviec>()
                .Property(e => e.Mavuviec)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvuviec>()
                .Property(e => e.Tenvuviec)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvuviec>()
                .Property(e => e.Manhomvuviec)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmvuviec>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Sochungtugoc)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Kemtheo)
                .IsFixedLength();

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Tiendathanhtoan)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Mahopdong)
                .IsFixedLength();

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Sohoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Kyhieuhoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Magiaodichphu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Manhanviencongno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .Property(e => e.Manhanviendathang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .HasMany(e => e.TDS_Giaodichct)
                .WithRequired(e => e.TDS_Giaodich)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Giaodich>()
                .HasMany(e => e.TDS_Taikhoanhachtoan)
                .WithRequired(e => e.TDS_Giaodich)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Dongiacovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Dongiachuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Tyleck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Tienck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Giavon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Doanhthu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichct>()
                .Property(e => e.Makhodieuchuyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Sochungtugoc)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Kemtheo)
                .IsFixedLength();

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Tiendathanhtoan)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Mahopdong)
                .IsFixedLength();

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Sohoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Kyhieuhoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Magiaodichphu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Manhanviencongno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvien>()
                .Property(e => e.Manhanviendathang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Dongiacovat)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Dongiachuavat)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Tienhang)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Tienvat)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Tyleck)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Tienck)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Giavon)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichdhthanhvienct>()
                .Property(e => e.Doanhthu)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Maquay)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .Property(e => e.Manhanviencongno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .HasMany(e => e.TDS_Giaodichquayct)
                .WithRequired(e => e.TDS_Giaodichquay)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Giaodichquay>()
                .HasMany(e => e.TDS_Taikhoanhachtoan)
                .WithRequired(e => e.TDS_Giaodichquay)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Giabanlecovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Giabanlechuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Tyleck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Tienck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Giavon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayamct>()
                .Property(e => e.Doanhthu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Giabanlecovat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Giabanlechuavat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Tyleck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Tienck)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Giavon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichquayct>()
                .Property(e => e.Doanhthu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichtondau>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondau>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondau>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondau>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondauct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondauct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Giaodichtondauct>()
                .Property(e => e.Giavon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Giaodichtondauct>()
                .Property(e => e.Tienvon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_KhohangNhomhang>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_KhohangNhomhang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_KhohangNhomhang>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_KhohangNhomhang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmai>()
                .Property(e => e.Machuongtrinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmai>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmai>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmai>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Machuongtrinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Masieuthiban)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Masieuthikm)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Mabohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Makhohangban)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Makhohangkm)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Giatrikmmax)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Giatrikmmin)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Tienchietkhau)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Khuyenmaict>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Manhomhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Makehang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Mavtu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Mavuviecthua)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .Property(e => e.Mavuviecthieu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemke>()
                .HasMany(e => e.TDS_Kiemkect)
                .WithRequired(e => e.TDS_Kiemke)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Manhomvtu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Makehang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Mavtu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Tientonmay)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Tienkiemke)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Tienchenhlech)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Kiemkect>()
                .Property(e => e.Giavon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Lichdathang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichdathang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichxuathang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichxuathang>()
                .Property(e => e.Makhohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichxuathang>()
                .Property(e => e.Manganhhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichxuathang>()
                .Property(e => e.Ngaytrongtuan)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichxuathang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Menuid)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Menucha)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Formname)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Maphanhe)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menu>()
                .Property(e => e.Thamso)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menunhomquyen>()
                .Property(e => e.Manhomquyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menunhomquyen>()
                .Property(e => e.Menuid)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menunhomquyen>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Menunhomquyen>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Manhomquyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Tendangnhap)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Matkhau)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Code1)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Code2)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Code3)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nguoisudung>()
                .Property(e => e.Maphanhe)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nhomquyen>()
                .Property(e => e.Manhomquyen)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nhomquyen>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nhomquyenphu>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Nhomquyenphu>()
                .Property(e => e.Chucnangphu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Mactktpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Mactu)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Soctkt)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Sohoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctkt>()
                .Property(e => e.Kyhieuhoadon)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoan>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoan>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoan>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoanct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoanct>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoanct>()
                .Property(e => e.Matk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Sodutaikhoanct>()
                .Property(e => e.Sotienduno)
                .HasPrecision(18, 7);

            modelBuilder.Entity<TDS_Sodutaikhoanct>()
                .Property(e => e.Sotienduco)
                .HasPrecision(18, 7);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathang>()
                .Property(e => e.Manvkinhdoanh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Dongia)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Doanhthugio)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Dongiaviethoadon)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Mamaysanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Magiacong)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDathangct>()
                .Property(e => e.Masovitinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDmmaysanxuat>()
                .Property(e => e.Mamaysanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDmNangSuatNV>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXDmNangSuatNV>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Maptnx)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Manguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Manvsanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsx>()
                .Property(e => e.Madonhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Masieuthi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Dongia)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Tienhang)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Tienvat)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Thanhtien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Mamaysanxuat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Magiacong)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_SXLenhsxct>()
                .Property(e => e.Masovitinh)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Sotien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Taikhoanhachtoan>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Thamsohethong>()
                .Property(e => e.Mathamso)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Thamsohethong>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Thuachitheogd>()
                .Property(e => e.Sotien)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Manhacungcap)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Manhanvien)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Sochungtugoc)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Sotienphatsinhno)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Sotienphatsinhco)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Congno>()
                .Property(e => e.Nguoitao)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Ctugoc>()
                .Property(e => e.Magiaodichpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Ctugoc>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Ctugoc>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Ctugoc>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmcapma>()
                .Property(e => e.Macappk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmcapma>()
                .Property(e => e.Madonvi)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Dmcapma>()
                .Property(e => e.Loaima)
                .IsFixedLength();

            modelBuilder.Entity<TDS_Dmcapma>()
                .Property(e => e.Mastart)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Lichsuthaydoigia>()
                .Property(e => e.Gianhapcu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Lichsuthaydoigia>()
                .Property(e => e.Giabanlecu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Lichsuthaydoigia>()
                .Property(e => e.Giabanbuoncu)
                .HasPrecision(20, 8);

            modelBuilder.Entity<TDS_Log>()
                .Property(e => e.Malog)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Mactktpk)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Matkno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Matkco)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Makhachhang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Mavuviec)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Manhanviencongno)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Manhanviengiaohang)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Mavat)
                .IsUnicode(false);

            modelBuilder.Entity<TDS_Quyctktct>()
                .Property(e => e.Makmchiphi)
                .IsUnicode(false);
        }
    }
}
