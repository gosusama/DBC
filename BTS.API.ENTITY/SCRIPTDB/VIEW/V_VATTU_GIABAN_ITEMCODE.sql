CREATE OR REPLACE FORCE VIEW "TBNETERP"."V_VATTU_GIABAN_ITEMCODE" ("ID", "MAHANG", "MAVATTU", "TENHANG", "TENVATTU", "TENVIETTAT", "BARCODE", "DONVITINH", "MABAOBI", "MAKEHANG", "MAKHAC", "MAKHACHHANG", "MALOAIVATTU", "MANHOMVATTU", "PTTINHGIA", "SOTONMAX", "SOTONMIN", "TRANGTHAI", "UNITCODE", "MADONVI", "GIAMUA", "GIAMUAVAT", "GIABANLE", "GIABANBUON", "TYLELAIBUON", "TYLELAILE", "MAVATRA", "MAVATVAO", "TYLEVATRA", "TYLEVATVAO", "GIABANLEVAT", "GIABANBUONVAT", "I_STATE", "ITEMCODE") AS 
  SELECT 
    vt.id as Id,
    vt.mavattu as MaHang,
    vt.mavattu as MaVatTu,
    vt.tenvattu as TenHang,
    vt.tenvattu as TenVatTu,
    vt.tenviettat as TenVietTat,
    vt.barcode as Barcode,
    vt.donvitinh as DonViTinh,
    vt.mabaobi as MaBaoBi,
    vt.makehang as MaKeHang,
    vt.makhac as MaKhac,
    vt.makhachhang as MaKhachHang,
    vt.maloaivattu as MaLoaiVatTu,
    vt.manhomvattu as MaNhomVatTu,
    vt.pttinhgia as PtTinhGia,
    nvl(vt.sotonmax, 0) as SoTonMax,
    nvl(vt.sotonmin, 0) as SoTonMin,
    vt.trangthai as TrangThai,
    vt.unitcode as UnitCode,
    gc.madonvi as MaDonVi,
    nvl(gc.giamua, 0) as GiaMua,
	nvl(gc.giamuavat, 0) as GiaMuaVat,
    nvl(gc.giabanle, 0) as GiaBanLe,
    nvl(gc.giabanbuon, 0) as GiaBanBuon,
    nvl(gc.ty_lelai_buon, 0) as TyLeLaiBuon,
    nvl(gc.ty_lelai_le, 0) as TyLeLaiLe,
    vt.mavatra as MaVatRa,
    vt.mavatvao as MaVatVao,
    nvl(vt.tyle_vat_ra, 0) as TyLeVatRa,
    nvl(vt.tyle_vat_vao, 0) as TyLeVatVao,
    nvl(gc.gia_banle_vat, 0) as GiaBanLeVat,
    nvl(gc.gia_banbuon_vat, 0) as GiaBanBuonVat,
	nvl(vt.I_State, 0) as I_State,
    nvl(vt.ItemCode, 0) as ItemCode
FROM DMVATTU vt
LEFT JOIN DM_VATTU_GIACA gc
ON vt.MAVATTU = gc.MAVATTU
where length(vt.ItemCode)>3
 
 ;
 
