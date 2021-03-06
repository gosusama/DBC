﻿create or replace PROCEDURE BAOCAO_DCN_CHITIET(P_TABLE_NAME IN VARCHAR2,P_GROUPBY IN VARCHAR2, P_PTHUCNHAN IN VARCHAR2, P_MAKHO IN VARCHAR2, 
P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_MATHUE IN VARCHAR2,
P_TUNGAY IN DATE,P_DENNGAY IN DATE, P_NGUOIDUNG IN VARCHAR2, cur  OUT SYS_REFCURSOR) IS

  P_EXPRESSION VARCHAR(3000);
  QUERY_STR VARCHAR(3000);
  P_SELECT_COLUMNS_IN_GROUPBY VARCHAR(3000);
  P_SELECT_COLUMNS_OUT_GROUPBY VARCHAR(3000);
  P_TABLE_GROUPBY VARCHAR(3000);
  P_COLUMNS_GROUPBY VARCHAR(3000);
  P_WHERE_PTNHAN VARCHAR(500):= '';
BEGIN
IF P_PTHUCNHAN = 'NHANCHUYENKHO' THEN
    P_WHERE_PTNHAN := ' AND NVL(t.MADONVIXUAT,'''') = NVL(t.UNITCODE,'''') ';
ELSE 
    P_WHERE_PTNHAN := ' AND NVL(t.MADONVIXUAT,'''') <> NVL(t.UNITCODE,'''') ';
END IF;
IF TRIM(P_MAKHO) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND t.MAKHOXUAT IN ('||P_MAKHO||')';
END IF;
IF TRIM(P_MALOAI) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
END IF;
IF TRIM(P_MANHOM) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
END IF;
IF TRIM(P_NGUOIDUNG) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND t.I_CREATE_BY IN ('||P_NGUOIDUNG||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAVATTU IN ('||P_MAVATTU||')';
END IF;    
IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
END IF;  
IF TRIM(P_MATHUE) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND ct.VAT IN ('||P_MATHUE||')';
END IF;

IF P_GROUPBY = 'MADONVI' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU,b.BARCODE, t.MADONVIXUAT,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' a.MA AS MaCon, a.TEN AS TenCon,a.BARCODE AS BARCODE, c.MADONVI AS MaCha, c.TENDONVI AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= 'b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE , t.MAKHOXUAT AS GROUPCODE,t.NGAYCHUNGTU AS NGAYCHUNGTU ';
P_TABLE_GROUPBY:= ' INNER JOIN AU_DONVI c ON a.MADONVI = c.MADONVI AND c.MADONVI = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'MAKHO' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU,b.BARCODE, t.MAKHOXUAT,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' a.MA AS MaCon, a.TEN AS TenCon,a.BARCODE AS BARCODE, c.MAKHO AS MaCha, c.TENKHO AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= 'b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE , t.MAKHOXUAT AS GROUPCODE,t.NGAYCHUNGTU AS NGAYCHUNGTU ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_KHO c ON a.GROUPCODE = c.MAKHO AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'MANHOMVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE,b.MANHOMVATTU,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE,c.MANHOMVTU AS MaCha, c.TENNHOMVT AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= 'b.MAVATTU AS MA, b.TENVATTU AS TEN , b.BARCODE AS BARCODE, b.MANHOMVATTU AS GROUPCODE,t.NGAYCHUNGTU AS NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHOMVATTU c ON a.GROUPCODE = c.MANHOMVTU AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'MALOAIVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE, b.MALOAIVATTU,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE,c.MALOAIVATTU AS MaCha, c.TENLOAIVT AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE, b.MALOAIVATTU AS GROUPCODE,t.NGAYCHUNGTU AS NGAYCHUNGTU ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_LOAIVATTU c ON a.GROUPCODE = c.MALOAIVATTU AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'MANHACUNGCAP' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE,  b.MAKHACHHANG,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE, c.MANCC AS MaCha, c.TENNCC AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE, b.MAKHACHHANG AS GROUPCODE,t.NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHACUNGCAP c ON a.GROUPCODE = c.MANCC AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'NGUOIDUNG' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE,  t.I_CREATE_BY, t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE, c.USERNAME AS MaCha, c.TENNHANVIEN AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE, t.I_CREATE_BY AS GROUPCODE,t.NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' INNER JOIN AU_NGUOIDUNG c ON a.GROUPCODE = c.USERNAME AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'MALOAITHUE' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE, ct.VAT,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE,a.GROUPCODE AS MaCha, c.LOAITHUE AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN, b.BARCODE AS BARCODE, ct.VAT AS GROUPCODE,t.NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' LEFT JOIN DM_LOAITHUE c ON a.GROUPCODE = c.MALOAITHUE AND c.UNITCODE = '''||P_UNITCODE||'''';
END;
ELSIF P_GROUPBY = 'PHIEU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU,b.BARCODE,t.MACHUNGTUPK,t.NOIDUNG, t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon,a.BARCODE AS BARCODE, a.MACHUNGTUPK AS MaCha, a.NOIDUNG AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN,b.BARCODE AS BARCODE, t.MACHUNGTUPK AS MACHUNGTUPK,t.NOIDUNG AS NOIDUNG ,t.NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' ';
END;
ELSE --ELSIF P_GROUPBY = 'MAVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU, b.BARCODE,t.NGAYCHUNGTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS MaCon, a.TEN AS TenCon, a.BARCODE AS BARCODE,  a.MA AS MaCha, a.TEN AS TenCha,a.NGAYCHUNGTU AS NgayGiaoDich';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN , b.BARCODE AS BARCODE,t.NGAYCHUNGTU';
P_TABLE_GROUPBY:= ' ';
END;
END IF;
  QUERY_STR:= 'select ROUND(a.SOLUONGBAN,2) as SOLUONGBAN,
                     a.DONGIANHAP,
                     ROUND(a.TIENTHUE) AS TIENTHUE,
                     ROUND(a.DOANHTHU) AS DOANHTHU,
                     ROUND(a.DOANHTHU + a.TIENTHUE) AS TONGBAN,
                     a.TIENCHIETKHAU,
  '||P_SELECT_COLUMNS_OUT_GROUPBY||'
  from (
  SELECT  '||P_SELECT_COLUMNS_IN_GROUPBY||'
                          ,SUM(ct.SOLUONG) as SOLUONGBAN
                          ,ROUND(SUM(ct.DONGIA),2) as DONGIANHAP
                          ,t.UNITCODE AS MADONVI
                          ,t.MAKHOXUAT AS MAKHOXUAT
                          ,ROUND(SUM(CASE WHEN NVL(t.TIENCHIETKHAU,0) >0 AND NVL(t.THANHTIENTRUOCVAT,0)>0 THEN ct.THANHTIEN *(NVL(t.TIENCHIETKHAU,0)/t.THANHTIENTRUOCVAT) ELSE 0 END),2) as TIENCHIETKHAU
                          ,SUM(ct.SOLUONG * ct.DONGIA) as DOANHTHU 
                          ,SUM((NVL(g.TYGIA,0) / 100)*(ct.SOLUONG *ct.DONGIA)) as TIENTHUE
                      from VATTUCHUNGTUCHITIET ct 
INNER JOIN VATTUCHUNGTU t on t.MACHUNGTUPK = ct.MACHUNGTUPK AND t.LOAICHUNGTU = ''DCN'' '||P_WHERE_PTNHAN||'
INNER JOIN DM_VATTU b on b.MAVATTU = ct.MAHANG
LEFT JOIN DM_LOAITHUE g on g.MALOAITHUE = ct.VAT
WHERE
    t.TRANGTHAI = 10
        AND t.UNITCODE = '''||P_UNITCODE||'''
    AND t.NGAYDUYETPHIEU <=TO_DATE('''||P_DENNGAY||''',''DD/MM/YY'')
    AND t.NGAYDUYETPHIEU >=TO_DATE('''||P_TUNGAY||''',''DD/MM/YY'')
	'||P_EXPRESSION||'
	group by  t.UNITCODE,t.MAKHOXUAT,'||P_COLUMNS_GROUPBY||'
  order by '||P_COLUMNS_GROUPBY||') a
	  '||P_TABLE_GROUPBY;
 DBMS_OUTPUT.put_line (QUERY_STR );    
  OPEN cur FOR QUERY_STR;
  EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
         DBMS_OUTPUT.put_line (SQLERRM);     
  END BAOCAO_DCN_CHITIET;
 