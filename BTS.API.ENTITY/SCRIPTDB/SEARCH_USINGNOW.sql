create or replace PROCEDURE FILTER_EXTERNALCODE_KIEMKE
(
  P_MADONVI IN VARCHAR2 ,
  P_TABLE IN VARCHAR2,
  P_MAKHO IN VARCHAR2,
  EXTERNALCODE OUT SYS_REFCURSOR
) AS
  SQL_QUERRY VARCHAR(32264);
BEGIN 
    SQL_QUERRY :=  'SELECT C.ID,C.MAVATTU,C.TENVATTU,C.MAKEHANG,C.MALOAIVATTU,C.MANHOMVATTU,C.MAKHACHHANG,C.BARCODE,D.MAKHO,
    NVL(D.TONDAUKYSL,0) AS TONDAUKYSL,NVL(D.TONDAUKYGT,0) AS TONDAUKYGT,NVL(D.NHAPSL,0) AS NHAPSL,NVL(D.NHAPGT,0) AS NHAPGT,NVL(D.XUATSL,0) AS XUATSL,NVL(D.XUATGT,0) AS XUATGT,NVL(D.TONCUOIKYSL,0) AS TONCUOIKYSL,
    NVL(D.TONCUOIKYGT,0) AS TONCUOIKYGT,NVL(D.GIAVON,0) AS GIAVON FROM
(SELECT A.ID,A.MAVATTU,A.TENVATTU,A.MAKEHANG,A.MALOAIVATTU,A.MANHOMVATTU,A.MAKHACHHANG,A.BARCODE
FROM DMVATTU A LEFT JOIN NVKIEMKECHITIET B ON A.MAVATTU = B.MAVATTU WHERE B.KEKIEMKE IS NULL) C 
INNER JOIN '||P_TABLE||' D ON C.MAVATTU = D.MAVATTU AND D.MAKHO = '''||P_MAKHO||''' AND D.UNITCODE = '''||P_MADONVI||''' AND D.TONDAUKYSL != 0';        
BEGIN
OPEN EXTERNALCODE FOR SQL_QUERRY;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (SQL_QUERRY  || SQLERRM);
END;
END FILTER_EXTERNALCODE_KIEMKE;
/
create or replace PROCEDURE CAPNHAT_MANHOM_HANGHOA(P_ID IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHOMVATTU IN VARCHAR2) IS
      P_UPDATE_DMVATTU VARCHAR2(32767);
      P_UPDATE_NVKIEMKECHITIET VARCHAR2(32767);
      BEGIN
          P_UPDATE_DMVATTU := ' UPDATE DMVATTU a SET(a.MANHOMVATTU) = '''||P_NHOMVATTU||''' WHERE a.MAVATTU = '''||P_MAVATTU||''' AND ID='''||P_ID||''' ';
          P_UPDATE_NVKIEMKECHITIET := ' UPDATE NVKIEMKECHITIET a SET(a.NHOMVATTUKK) = '''||P_NHOMVATTU||''' WHERE a.MAVATTU = '''||P_MAVATTU||''' ';    
       BEGIN
            EXECUTE IMMEDIATE P_UPDATE_DMVATTU;
            EXECUTE IMMEDIATE P_UPDATE_NVKIEMKECHITIET;
       END;
END CAPNHAT_MANHOM_HANGHOA;
/
create or replace PROCEDURE PC_VATTU_COUNT
(
  P_MADONVI IN VARCHAR2 ,
  P_STRKEY IN VARCHAR2,
  P_SUBQUERY IN VARCHAR2,
  P_PAGENUMBER IN NUMBER,
  P_PAGESIZE IN NUMBER,
  P_TOTALITEM OUT NUMBER,
 cur OUT SYS_REFCURSOR
) AS 
P NUMBER;
STR_COUNT VARCHAR2(32264);
STR_QUERY VARCHAR2(32264);
BEGIN

BEGIN
STR_COUNT:= 'SELECT vt.id as Id,
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
 FROM DMVATTU vt LEFT JOIN DM_VATTU_GIACA gc on vt.MAVATTU = gc.MAVATTU ';
IF TRIM(P_MADONVI) IS NOT NULL THEN
STR_COUNT:= STR_COUNT|| ' AND gc.MADONVI = '''||P_MADONVI||'''';
END IF;
STR_COUNT:= 'SELECT * FROM ( '||STR_COUNT||' ) x WHERE 1=1 ';
IF TRIM(P_SUBQUERY) IS NOT NULL THEN
STR_COUNT := STR_COUNT || '  '||P_SUBQUERY;
END IF;
STR_COUNT := 'SELECT COUNT(a.MaVatTu) from ( ' || STR_COUNT || ' ) a';
DBMS_OUTPUT.PUT_LINE(STR_COUNT);
OPEN cur FOR STR_COUNT;    
EXCEPTION WHEN OTHERS THEN 
goto countinus;
END;
<<countinus>>
NULL;
END PC_VATTU_COUNT;

/
create or replace PROCEDURE PC_VATTU_SEARCH 
(
  P_MADONVI IN VARCHAR2 ,
  P_STRKEY IN VARCHAR2,
  P_SUBQUERY IN VARCHAR2,
 cur OUT SYS_REFCURSOR
) AS 
STR_QUERY VARCHAR2(32264);
BEGIN

STR_QUERY:=  'SELECT * FROM V_VATTU_GIABAN WHERE 1=1';
IF TRIM(P_MADONVI) IS NOT NULL THEN
STR_QUERY:= STR_QUERY|| ' AND MaDonVi = '''||P_MADONVI||'''';
END IF;
IF TRIM(P_SUBQUERY) IS NOT NULL THEN
STR_QUERY := STR_QUERY || '  '||P_SUBQUERY;
END IF;
        DBMS_OUTPUT.PUT_LINE(STR_QUERY);
        OPEN cur FOR STR_QUERY;    
        EXCEPTION WHEN OTHERS THEN COMMIT;
END PC_VATTU_SEARCH;

/


create or replace PROCEDURE PC_VATTU_SEARCH_PAGING 
(
  P_MADONVI IN NVARCHAR2,
  P_STRKEY IN NVARCHAR2,
  P_SUBQUERY IN NVARCHAR2,
  P_ORDERSTR IN NVARCHAR2,
  P_PAGENUMBER IN NUMBER,
  P_PAGESIZE IN NUMBER,
  P_TOTALITEM OUT NUMBER,
  
 cur OUT SYS_REFCURSOR
) AS 
STR_COUNT VARCHAR2(32264);
STR_QUERY VARCHAR2(32264);
BEGIN

STR_QUERY:=  '  SELECT vt.id as Id,
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
                left join DM_VATTU_GIACA gc on vt.MAVATTU = gc.MAVATTU  ';
IF TRIM(P_MADONVI) IS NOT NULL THEN
STR_QUERY:= STR_QUERY|| ' AND gc.MADONVI = '''||P_MADONVI||'''';
END IF;
STR_QUERY:= 'SELECT * FROM ( '||STR_QUERY||' ) a WHERE 1=1 ';
IF TRIM(P_SUBQUERY) IS NOT NULL THEN
STR_QUERY := STR_QUERY || '  '||P_SUBQUERY;
END IF;
IF TRIM(P_ORDERSTR) IS NOT NULL THEN
STR_QUERY := STR_QUERY || '  '||P_ORDERSTR;
END IF;


BEGIN
STR_COUNT:= 'SELECT count(vt.MAVATTU) INTO P_TOTALITEM  FROM DMVATTU vt LEFT JOIN DM_VATTU_GIACA gc on vt.MAVATTU = gc.MAVATTU ';
IF TRIM(P_MADONVI) IS NOT NULL THEN
STR_COUNT:= STR_COUNT|| ' AND gc.MADONVI = '''||P_MADONVI||'''';
END IF;
STR_COUNT:= STR_COUNT|| ' WHERE 1=1 ';
IF TRIM(P_SUBQUERY) IS NOT NULL THEN
STR_COUNT := STR_COUNT || '  '||P_SUBQUERY;
END IF;
DBMS_OUTPUT.PUT_LINE(STR_COUNT);
execute immediate STR_COUNT;
EXCEPTION WHEN OTHERS THEN 
goto countinus;
END;
<<countinus>>
STR_QUERY:= 'SELECT * FROM
(
    SELECT a.*, rownum r__
    FROM
    (
        '||STR_QUERY||'
    ) a
    WHERE rownum < (('||P_PAGENUMBER||' * '||P_PAGESIZE||') + 1 )
)
WHERE r__ >= ((('||P_PAGENUMBER||'-1) * '||P_PAGESIZE||') + 1)';

        DBMS_OUTPUT.PUT_LINE(STR_QUERY);
        OPEN cur FOR STR_QUERY;    
        EXCEPTION WHEN OTHERS THEN COMMIT;
END PC_VATTU_SEARCH_PAGING;

/

create or replace PROCEDURE PC_VATTU_SEARCH_SYNC
(
  P_MADONVI IN VARCHAR2 ,
  P_STRKEY IN VARCHAR2
, cur OUT SYS_REFCURSOR
) AS 
STR_QUERY VARCHAR2(32264);
BEGIN

STR_QUERY:=  'SELECT * FROM V_VATTU_GIABAN WHERE I_STATE=50';
IF TRIM(P_MADONVI) IS NOT NULL THEN
STR_QUERY:= STR_QUERY|| ' AND MaDonVi = '''||P_MADONVI||'''';
END IF;

        DBMS_OUTPUT.PUT_LINE(STR_QUERY);
        OPEN cur FOR STR_QUERY;    
        EXCEPTION WHEN OTHERS THEN COMMIT;
END PC_VATTU_SEARCH_SYNC;