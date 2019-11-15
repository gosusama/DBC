﻿create or replace PROCEDURE            "BAOCAO_KIEMKE_TONGHOP" (P_DKIENLOC IN VARCHAR2,P_GROUPBY IN VARCHAR2,P_MAKHO IN VARCHAR2,P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2, P_MAKHACHHANG IN VARCHAR2,P_MAKEHANG IN VARCHAR2, P_UNITCODE IN VARCHAR2, P_TUNGAY IN DATE, P_DENNGAY IN DATE, CUR  OUT SYS_REFCURSOR) AS
QUERY_STR VARCHAR(32767);
P_SELECT_COLUMNS_IN_GROUPBY VARCHAR(32767);
P_SELECT_COLUMNS_OUT_GROUPBY VARCHAR(32767);
P_TABLE_GROUPBY VARCHAR(32767);
P_COLUMNS_GROUPBY VARCHAR(32767);
P_EXPRESSION  VARCHAR(32767):= '';
P_DIEUKIENLOC VARCHAR(32767):='';
BEGIN
IF TRIM(P_MAKHO) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND t.KHOKIEMKE IN ('||P_MAKHO||')';
END IF;
IF TRIM(P_MALOAI) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
END IF;
IF TRIM(P_MANHOM) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAVATTU IN ('||P_MAVATTU||')';
END IF;    
IF TRIM(P_MAKHACHHANG) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAKHACHHANG IN ('||P_MAKHACHHANG||')';
END IF;  
IF TRIM(P_MAKEHANG) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND ct.KEKIEMKE IN ('||P_MAKEHANG||')';
END IF;  

IF P_DKIENLOC = 'BAOCAOTHUA' THEN
BEGIN
P_DIEUKIENLOC := 'AND NVL(ct.SOLUONGCHENHLECH,0) < 0';
END;
ELSIF P_DKIENLOC ='BAOCAOTHIEU' THEN
BEGIN
P_DIEUKIENLOC :='AND NVL(ct.SOLUONGCHENHLECH,0) > 0';
END;
ELSE
P_DIEUKIENLOC := '';
END IF;
IF P_GROUPBY = 'MAKHO' THEN
BEGIN
P_COLUMNS_GROUPBY:= 't.KHOKIEMKE';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' c.MAKHO AS Ma, c.TENKHO AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' t.KHOKIEMKE AS MA ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_KHO c ON a.MA = c.MAKHO ';
END;
ELSIF P_GROUPBY = 'MANHOMVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MANHOMVATTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' c.MANHOMVTU AS Ma, c.TENNHOMVT AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MANHOMVATTU AS MA ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHOMVATTU c ON a.MA = c.MANHOMVTU ';
END;
ELSIF P_GROUPBY = 'MALOAIVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MALOAIVATTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'c.MALOAIVATTU AS Ma, c.TENLOAIVT AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MALOAIVATTU as MA ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_LOAIVATTU c ON a.MA = c.MALOAIVATTU ';
END;
ELSIF P_GROUPBY = 'MANHACUNGCAP' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAKHACHHANG';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'c.MANCC AS Ma, c.TENNCC AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAKHACHHANG AS MA';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHACUNGCAP c ON a.MA = c.MANCC';
END;
ELSIF P_GROUPBY = 'MAKEHANG' THEN
BEGIN
P_COLUMNS_GROUPBY:= 't.KEKIEMKE';
P_SELECT_COLUMNS_OUT_GROUPBY:= ' c.MAKEHANG AS Ma, c.TENKEHANG AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' t.KEKIEMKE AS MA';
P_TABLE_GROUPBY:= ' INNER JOIN MDKEHANG c ON a.MA = c.MAKEHANG';
END;
ELSE --ELSIF P_GROUPBY = 'MAVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU,b.TENVATTU';
P_SELECT_COLUMNS_OUT_GROUPBY:= 'a.MA AS Ma, a.TEN AS Ten ';
P_SELECT_COLUMNS_IN_GROUPBY:= ' b.MAVATTU AS MA, b.TENVATTU AS TEN';
P_TABLE_GROUPBY:= ' ';
END;
END IF;

QUERY_STR := 'select (a.GIAVON) as GIAVON,(a.SOLUONGTONMAY) as SOLUONGTONMAY,(a.SOLUONGKIEMKE) as SOLUONGKIEMKE
			   ,(a.SOLUONGTHUA) as SOLUONGTHUA,(a.GIATRITHUA) as GIATRITHUA,(a.SOLUONGTHIEU) as SOLUONGTHIEU,(a.GIATRITHIEU) as GIATRITHIEU,
'||P_SELECT_COLUMNS_OUT_GROUPBY||'
from (
SELECT  '||P_SELECT_COLUMNS_IN_GROUPBY||'
						  ,SUM(NVL(ct.GIAVON,0)) as GIAVON
                          ,SUM(NVL(ct.SOLUONGTONMAY,0)) as SOLUONGTONMAY
                          ,SUM(NVL(ct.SOLUONGKIEMKE,0)) as SOLUONGKIEMKE                          
						  ,SUM(CASE 
									WHEN NVL(ct.SOLUONGCHENHLECH,0) < 0
									THEN (NVL(ct.SOLUONGKIEMKE,0) - NVL(ct.SOLUONGTONMAY,0))
									ELSE 0 END) as SOLUONGTHUA
						  ,SUM(CASE 
									WHEN NVL(ct.SOLUONGCHENHLECH,0) < 0
									THEN (NVL(ct.TIENKIEMKE,0) - NVL(ct.TIENTONMAY,0))
									ELSE 0 END) as GIATRITHUA
						  ,SUM(CASE 
						  			WHEN NVL(ct.SOLUONGCHENHLECH,0) > 0
									THEN (NVL(ct.SOLUONGTONMAY,0) - NVL(ct.SOLUONGKIEMKE,0))
									ELSE 0 END) as SOLUONGTHIEU
						  ,SUM(CASE 
									WHEN NVL(ct.SOLUONGCHENHLECH,0) > 0
									THEN (NVL(ct.TIENTONMAY,0) - NVL(ct.TIENKIEMKE,0))
									ELSE 0 END) as GIATRITHIEU										
from NVKIEMKECHITIET ct 
inner join NVKIEMKE t on t.MAPHIEUKIEMKE = ct.MAPHIEUKIEMKE 
INNER JOIN DM_VATTU b on b.MAVATTU = ct.MAVATTU
WHERE t.TRANGTHAI = 10
    AND t.UNITCODE = '''||P_UNITCODE||'''
    AND t.NGAYDUYETPHIEU <=TO_DATE('''||P_DENNGAY||''',''DD/MM/YY'')
    AND t.NGAYDUYETPHIEU>= TO_DATE('''||P_TUNGAY||''',''DD/MM/YY'') 
	'||P_EXPRESSION||' 	
	'||P_DIEUKIENLOC||'
	group by  '||P_COLUMNS_GROUPBY||') a
 '||P_TABLE_GROUPBY;
BEGIN
DBMS_OUTPUT.put_line (QUERY_STR);  
OPEN CUR FOR QUERY_STR;
EXCEPTION
            WHEN NO_DATA_FOUND THEN
             NULL;
               WHEN OTHERS THEN
      NULL;
      END;

END BAOCAO_KIEMKE_TONGHOP;