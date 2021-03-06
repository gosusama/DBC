﻿create or replace PACKAGE "DATHANG" AS 
--VER 1
--VUDQ: 09/09/2017
PROCEDURE DATHANGTONGHOP(P_GROUPBY IN VARCHAR2, P_TRANGTHAI IN VARCHAR2, P_NHANVIEN IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, cur  OUT SYS_REFCURSOR);
PROCEDURE DATHANGCHITIET(P_GROUPBY IN VARCHAR2, P_TRANGTHAI IN VARCHAR2, P_NHANVIEN IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, cur  OUT SYS_REFCURSOR);
END DATHANG;
/
create or replace PACKAGE BODY  "DATHANG" AS
PROCEDURE DATHANGTONGHOP(P_GROUPBY IN VARCHAR2, P_TRANGTHAI IN VARCHAR2, P_NHANVIEN IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, cur  OUT SYS_REFCURSOR)
AS
P_EXPRESSION VARCHAR(3232);
QUERY_STR VARCHAR(32767);
P_TABLE_GROUPBY VARCHAR(32767);
P_COLUMNS_GROUPBY VARCHAR(32767);
P_SELECT_COLUMNS_GROUPBY VARCHAR(32767);
P_SELECT VARCHAR(3232);
P_SELECT2 VARCHAR(3232);
BEGIN
IF TRIM(P_TRANGTHAI) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.TRANGTHAI IN ('||P_TRANGTHAI||')';
END IF;
IF TRIM(P_NHANVIEN) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.MANHANVIEN IN ('||P_NHANVIEN||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAHANG IN ('||P_MAVATTU||')';
END IF;    
IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
END IF;   

IF P_GROUPBY = 'TRANGTHAI' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.TRANGTHAI';
P_SELECT_COLUMNS_GROUPBY:= ', a.TRANGTHAI ';
P_TABLE_GROUPBY:= ' ';
P_SELECT2:='';
P_SELECT:= ' ';
END;
ELSIF P_GROUPBY = 'MANHANVIEN' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.MANHANVIEN,Su.Tennhanvien';
P_SELECT_COLUMNS_GROUPBY:= ' ,a.MANHANVIEN,Su.Tennhanvien ';
P_TABLE_GROUPBY:= 'left join Sys_User su on A.Manhanvien = Su.Manhanvien';
P_SELECT2:='';

P_SELECT:= ' ';

END;
ELSIF P_GROUPBY = 'MAKHACHHANG' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.MAKHACHHANG,Kh.Tenkh,';
P_SELECT_COLUMNS_GROUPBY:= ',a.MAKHACHHANG,Kh.Tenkh, ';
P_TABLE_GROUPBY:= 'left join Dmkhachhang kh on A.Makhachhang = kh.Makh';
P_SELECT2:='';

P_SELECT:= ' ';

END;

END IF;


QUERY_STR:= 'SELECT SUM(b.SOLUONG) as SOLUONG, SUM(B.Dongia) as DONGIA '||P_SELECT_COLUMNS_GROUPBY||'
FROM Nvdathang a
inner join Nvdathangchitiet b on A.Sophieupk = B.Sophieupk
'||P_TABLE_GROUPBY||'
WHERE  A.Unitcode = '''||P_UNITCODE||'''
    AND A.Ngay >=TO_DATE('''||P_TUNGAY||''',''DD/MM/YY'')
    AND A.Ngay <=TO_DATE('''||P_DENNGAY||''',''DD/MM/YY'')
    '||P_EXPRESSION||'
group by '||P_COLUMNS_GROUPBY||'';
     DBMS_OUTPUT.put_line (QUERY_STR);  
OPEN CUR FOR QUERY_STR;
  EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
         DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);     
END DATHANGTONGHOP;

PROCEDURE DATHANGCHITIET(P_GROUPBY IN VARCHAR2, P_TRANGTHAI IN VARCHAR2, P_NHANVIEN IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, cur  OUT SYS_REFCURSOR)
AS
P_EXPRESSION VARCHAR(3232);
QUERY_STR VARCHAR(32767);
P_TABLE_GROUPBY VARCHAR(32767);
P_COLUMNS_GROUPBY VARCHAR(32767);
P_SELECT_COLUMNS_GROUPBY VARCHAR(32767);
P_SELECT VARCHAR(3232);
P_SELECT2 VARCHAR(3232);
BEGIN
IF TRIM(P_TRANGTHAI) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.TRANGTHAI IN ('||P_TRANGTHAI||')';
END IF;
IF TRIM(P_NHANVIEN) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.MANHANVIEN IN ('||P_NHANVIEN||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND b.MAHANG IN ('||P_MAVATTU||')';
END IF;    
IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
P_EXPRESSION := P_EXPRESSION||' AND a.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
END IF;   

IF P_GROUPBY = 'TRANGTHAI' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.TRANGTHAI';
P_SELECT_COLUMNS_GROUPBY:= ' ,a.TRANGTHAI ';
P_TABLE_GROUPBY:= ' ';
P_SELECT2:='';
P_SELECT:= ' ';
END;
ELSIF P_GROUPBY = 'MANHANVIEN' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.MANHANVIEN,Su.Tennhanvien';
P_SELECT_COLUMNS_GROUPBY:= ' ,a.MANHANVIEN,Su.Tennhanvien ';
P_TABLE_GROUPBY:= 'left join Sys_User su on A.Manhanvien = Su.Manhanvien';
P_SELECT2:='';

P_SELECT:= ' ';

END;
ELSIF P_GROUPBY = 'MAKHACHHANG' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'a.MAKHACHHANG,Kh.Tenkh,';
P_SELECT_COLUMNS_GROUPBY:= ',a.MAKHACHHANG,Kh.Tenkh, ';
P_TABLE_GROUPBY:= 'left join Dmkhachhang kh on A.Makhachhang = kh.Makh';
P_SELECT2:='';

P_SELECT:= ' ';

END;

END IF;


QUERY_STR:= 'SELECT b.MAHANG,b.TENHANG, SUM(b.SOLUONG) as SOLUONG, SUM(B.Dongia) as DONGIA '||P_SELECT_COLUMNS_GROUPBY||'
FROM Nvdathang a
inner join Nvdathangchitiet b on A.Sophieupk = B.Sophieupk
'||P_TABLE_GROUPBY||'
WHERE  A.Unitcode = '''||P_UNITCODE||'''
    AND A.Ngay >=TO_DATE('''||P_TUNGAY||''',''DD/MM/YY'')
    AND A.Ngay <=TO_DATE('''||P_DENNGAY||''',''DD/MM/YY'')
    '||P_EXPRESSION||'
group by b.MAHANG,b.TENHANG,'||P_COLUMNS_GROUPBY||'';
     DBMS_OUTPUT.put_line (QUERY_STR);  
OPEN CUR FOR QUERY_STR;
  EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
         DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);     
END DATHANGCHITIET;

END DATHANG;
/