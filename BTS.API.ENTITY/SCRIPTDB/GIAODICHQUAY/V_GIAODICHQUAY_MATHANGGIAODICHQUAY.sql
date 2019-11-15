CREATE OR REPLACE VIEW V_GDQUAY_HANGQUAY
AS SELECT 
  gdq.magiaodich as MaGiaoDich,
  gdq.MAGIAODICHQUAYPK as MaGiaoDichQuayPK,
  gdq.madonvi as MaDonVi,
  gdq.loaigiaodich as LoaiGiaoDich,
  gdq.ngaytao as NgayTao,
  gdq.manguoitao as MaNguoiTao,
  gdq.maquayban as MaQuayBan,
  gdq.nguoitao as NguoiTao,
  gdq.ngayphatsinh as NgayPhatSinh,
  nvl(mh.TTIENCOVAT,0) as TTienCoVat,
  mh.mavattu as MaVatTu,
  mh.tendaydu as TenDayDu,
  nvl(mh.soluong,0) as SoLuong,
  mh.mabopk as MaBoPK,
  nvl(mh.vatban,0) as VatBan,
  nvl(mh.giabanlecovat,0) as GiaBanLeCoVat,
  mh.makhohang as MaKhoHang,
  mh.barcode as Barcode
FROM NVGDQUAY_ASYNCCLIENT gdq
LEFT JOIN NVHANGGDQUAY_ASYNCCLIENT mh
ON gdq.MAGIAODICHQUAYPK = mh.MAGDQUAYPK;

/* tạo bảng temp lưu tạm */
CREATE TABLE TEMP_GDQ (
              MADONVI NVARCHAR2(50),
              MANGUOITAO NVARCHAR2(50),
              MAQUAYBAN NVARCHAR2(50),
              NGUOITAO NVARCHAR2(300),
              TONGBAN NUMBER(17,4) DEFAULT 0,
              TONGTRALAI NUMBER (17,4) DEFAULT 0,
              STT_TEMP NUMBER(38,0) DEFAULT 0
            )