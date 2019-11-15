create or replace PROCEDURE            "BAOCAO_XNT_CHITIET" (P_GROUPBY IN VARCHAR2, P_MAKHO IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_MANHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2,P_USERNAME IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE , P_SELECTEDVAT IN VARCHAR2, cur  OUT SYS_REFCURSOR) AS
QUERY_STR VARCHAR(5000);
KY_KETTHUC NUMBER;
NAM_KETTHUC NUMBER;
P_SELECT_COLUMNS_GROUPBY VARCHAR(2000);
P_TABLE_GROUPBY VARCHAR(2000);
P_COLUMNS_GROUPBY VARCHAR(2000);
P_EXPRESSION VARCHAR(2000);
P_SQL_INSERT VARCHAR(5000);
P_NOTNULL VARCHAR(1000);
N NUMBER:=0;
CHECK_EXIST_TABLE NUMBER(18,2) := 0;
CREATE_TABLE VARCHAR(3000);
BEGIN
 CREATE_TABLE:=' CREATE GLOBAL TEMPORARY TABLE "TBNETERP"."TMP_XTN_NGAY" 
   (	"UNITCODE" NVARCHAR2(20), 
	"MAKHO" NVARCHAR2(50), 
	"MAVATTU" NVARCHAR2(50),
    "USERNAME" NVARCHAR2(50), 
	"GIAVON" NUMBER(17,4) DEFAULT 0, 
	"TONDAUKYSL" NUMBER(17,4) DEFAULT 0, 
	"TONDAUKYGT" NUMBER(17,4) DEFAULT 0, 
	"NHAPSL" NUMBER(17,4) DEFAULT 0, 
	"NHAPGT" NUMBER(17,4) DEFAULT 0, 
	"XUATSL" NUMBER(17,4) DEFAULT 0, 
	"XUATGT" NUMBER(17,4) DEFAULT 0, 
	"TONCUOIKYSL" NUMBER(17,4) DEFAULT 0, 
	"TONCUOIKYGT" NUMBER(17,4) DEFAULT 0
   ) ON COMMIT PRESERVE ROWS';
   EXECUTE IMMEDIATE 'SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = ''TMP_XTN_NGAY''' INTO CHECK_EXIST_TABLE;
    IF CHECK_EXIST_TABLE > 0 THEN DBMS_OUTPUT.PUT_LINE('');
    ELSE EXECUTE IMMEDIATE CREATE_TABLE;
    END IF;
    EXECUTE IMMEDIATE 'DELETE TMP_XTN_NGAY WHERE USERNAME = '''||P_USERNAME||''' AND UNITCODE = '''||P_UNITCODE||'''';
    IF P_SELECTEDVAT = '0' THEN 
    BEGIN
        FOR cur_period in (SELECT to_date(P_TUNGAY, 'dd/MM/yyyy') + LEVEL - 1 AS today
        FROM dual
        CONNECT BY LEVEL <= to_date(P_DENNGAY, 'dd/MM/yyyy') - to_date(P_TUNGAY, 'dd/MM/yyyy') + 1) LOOP
            BEGIN
            
            BEGIN--TĂƒÂ¬m kĂ¡Â»Â³
            SELECT KY, NAM INTO KY_KETTHUC, NAM_KETTHUC FROM DM_KYKETOAN WHERE to_date(DENNGAY, 'dd/MM/yyyy') = to_date(cur_period.today, 'dd/MM/yyyy') AND UNITCODE LIKE '%'||P_UNITCODE||'%' AND TRANGTHAI = 10 AND rownum =1;
               EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
            END;--End TĂƒÂ¬m ky
            
            BEGIN --BĂ¡ÂºÂ¯t Ă„â€˜Ă¡ÂºÂ§u thĂ¡Â»Â±c thi nhiĂ¡Â»â€¡m vu
            N:=N+1;
            IF (N = 1) THEN
            BEGIN -- BĂ¡ÂºÂ£n ghi Ă„â€˜Ă¡ÂºÂ§u tiĂƒÂªn
             P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY
                     (UNITCODE, MAKHO, MAVATTU, GIAVON, TONDAUKYSL, TONDAUKYGT, NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                     SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.TONDAUKYSL, a.TONDAUKYGT, a.NHAPSL, a.NHAPGT, a.XUATSL, a.XUATGT,'''||P_USERNAME||''' AS USERNAME
                     FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu  AND b.UNITCODE = a.UNITCODE
                     WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''
                     ';
                     --
            IF TRIM(P_MAKHO) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAKHO IN ('||P_MAKHO||')';
            END IF;
            IF TRIM(P_MALOAI) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
            END IF;
            IF TRIM(P_MANHOM) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
            END IF;
            IF TRIM(P_MAVATTU) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAVATTU IN ('||P_MAVATTU||')';
            END IF;    
            IF TRIM(P_MANHACUNGCAP) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_MANHACUNGCAP||')';
            END IF;   
            execute IMMEDIATE P_SQL_INSERT;
            
            EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
            END;
            ELSE
            BEGIN --VĂƒÂ²ng thĂ¡Â»Â© 2 trĂ¡Â»Å¸ Ă„â€˜i
             P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY
                     (UNITCODE, MAKHO, MAVATTU, GIAVON,NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                     SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.NHAPSL, a.NHAPGT, a.XUATSL, a.XUATGT,'''||P_USERNAME||''' AS USERNAME
                     FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu  AND b.UNITCODE = a.UNITCODE
                     WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''
                     ';
                     --
            IF TRIM(P_MAKHO) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAKHO IN ('||P_MAKHO||')';
            END IF;
            IF TRIM(P_MALOAI) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
            END IF;
            IF TRIM(P_MANHOM) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
            END IF;
            IF TRIM(P_MAVATTU) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAVATTU IN ('||P_MAVATTU||')';
            END IF;     
            IF TRIM(P_MANHACUNGCAP) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_MANHACUNGCAP||')';
            END IF;           
            execute IMMEDIATE P_SQL_INSERT;
            EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
            END;--KĂ¡ÂºÂ¿t thĂƒÂºc vong 2 trĂ¡Â»Å¸ Ă„â€˜i;
            END IF;
            END; -- KĂ¡ÂºÂ¿t thĂƒÂºc thĂ¡Â»Â±c thi nhiĂ¡Â»â€¡m vĂ¡Â»Â¥
            
            END;
                    <<end_loop>>
                    null;      
        END LOOP;
    END;
    ELSE BEGIN
        FOR cur_period in (SELECT to_date(P_TUNGAY, 'dd/MM/yyyy') + LEVEL - 1 AS today
        FROM dual
        CONNECT BY LEVEL <= to_date(P_DENNGAY, 'dd/MM/yyyy') - to_date(P_TUNGAY, 'dd/MM/yyyy') + 1) LOOP
            BEGIN
            
            BEGIN--TĂƒÂ¬m kĂ¡Â»Â³
            SELECT KY, NAM INTO KY_KETTHUC, NAM_KETTHUC FROM DM_KYKETOAN WHERE to_date(DENNGAY, 'dd/MM/yyyy') = to_date(cur_period.today, 'dd/MM/yyyy') AND UNITCODE LIKE '%'||P_UNITCODE||'%' AND TRANGTHAI = 10 AND rownum =1;
               EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
            END;--End TĂƒÂ¬m ky
            
            BEGIN --BĂ¡ÂºÂ¯t Ă„â€˜Ă¡ÂºÂ§u thĂ¡Â»Â±c thi nhiĂ¡Â»â€¡m vu
            N:=N+1;
            IF (N = 1) THEN
            BEGIN -- BĂ¡ÂºÂ£n ghi Ă„â€˜Ă¡ÂºÂ§u tiĂƒÂªn
             P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY
                     (UNITCODE, MAKHO, MAVATTU, GIAVON, TONDAUKYSL, TONDAUKYGT, NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                     SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.TONDAUKYSL, a.TONDAUKYGT, a.NHAPSL, a.NHAPGT, a.XUATSL, a.XUATGT,'''||P_USERNAME||''' AS USERNAME
                     FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu  AND b.UNITCODE = a.UNITCODE
                     WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''
                     ';
                     --
            IF TRIM(P_MAKHO) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAKHO IN ('||P_MAKHO||')';
            END IF;
            IF TRIM(P_MALOAI) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
            END IF;
            IF TRIM(P_MANHOM) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
            END IF;
            IF TRIM(P_MAVATTU) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAVATTU IN ('||P_MAVATTU||')';
            END IF;    
            IF TRIM(P_MANHACUNGCAP) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_MANHACUNGCAP||')';
            END IF;   
            execute IMMEDIATE P_SQL_INSERT;
            
            EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
            END;
            ELSE
            BEGIN --VĂƒÂ²ng thĂ¡Â»Â© 2 trĂ¡Â»Å¸ Ă„â€˜i
             P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY
                     (UNITCODE, MAKHO, MAVATTU, GIAVON,NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                     SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.NHAPSL, a.NHAPGT, a.NHAPGT*(1 + b.TYLE_VAT_VAO/100) as NHAPGT, a.XUATSL, (a.XUATGT*(1 + b.TYLE_VAT_VAO/100)) AS XUATGT,'''||P_USERNAME||''' AS USERNAME
                     FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu  AND b.UNITCODE = a.UNITCODE
                     WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''
                     ';
                     --
            IF TRIM(P_MAKHO) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAKHO IN ('||P_MAKHO||')';
            END IF;
            IF TRIM(P_MALOAI) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
            END IF;
            IF TRIM(P_MANHOM) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
            END IF;
            IF TRIM(P_MAVATTU) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND a.MAVATTU IN ('||P_MAVATTU||')';
            END IF;     
            IF TRIM(P_MANHACUNGCAP) IS NOT NULL THEN
            P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_MANHACUNGCAP||')';
            END IF;           
            execute IMMEDIATE P_SQL_INSERT;
            EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
            END;--KĂ¡ÂºÂ¿t thĂƒÂºc vong 2 trĂ¡Â»Å¸ Ă„â€˜i;
            END IF;
            END; -- KĂ¡ÂºÂ¿t thĂƒÂºc thĂ¡Â»Â±c thi nhiĂ¡Â»â€¡m vĂ¡Â»Â¥
            
            END;
                    <<end_loop>>
                    null;      
        END LOOP;
    END;
    END IF;

IF P_GROUPBY = 'MADONVI' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'c.MADONVI, c.TENDONVI, b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE ';
P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU as TenVT,b.BARCODE as BARCODE, c.MADONVI AS PARENT, c.TENDONVI AS PARENT_NAME, a.UNITCODE AS MADONVI ';
P_TABLE_GROUPBY:= ' INNER JOIN AU_DONVI c ON a.UNITCODE = c.MADONVI';
P_NOTNULL := ' AND c.MADONVI IS NOT NULL ';
END;
ELSIF P_GROUPBY = 'MAKHO' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'c.MAKHO, c.TENKHO, b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE ';
P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU as TenVT,b.BARCODE as BARCODE, c.MAKHO AS PARENT, c.TENKHO AS PARENT_NAME, a.UNITCODE AS MADONVI ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_KHO c ON a.MAKHO = c.MAKHO AND c.UNITCODE = a.UNITCODE';
P_NOTNULL := ' AND c.MAKHO IS NOT NULL ';
END;
ELSIF P_GROUPBY = 'MANHOMVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'c.MANHOMVTU, c.TENNHOMVT, b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE ';
P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU as TenVT,b.BARCODE as BARCODE, c.MANHOMVTU AS PARENT, c.TENNHOMVT AS PARENT_NAME, a.UNITCODE AS MADONVI  ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHOMVATTU c ON b.MANHOMVATTU = c.MANHOMVTU AND c.UNITCODE = a.UNITCODE';
P_NOTNULL := ' AND c.MANHOMVTU IS NOT NULL ';
END;
ELSIF P_GROUPBY = 'MALOAIVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'c.MALOAIVATTU, c.TENLOAIVT, b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE';
P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU as TenVT,b.BARCODE as BARCODE, c.MALOAIVATTU AS PARENT, c.TENLOAIVT AS PARENT_NAME, a.UNITCODE AS MADONVI ';
P_TABLE_GROUPBY:= ' INNER JOIN DM_LOAIVATTU c ON b.MALOAIVATTU = c.MALOAIVATTU AND c.UNITCODE = a.UNITCODE';
P_NOTNULL := ' AND c.MALOAIVATTU IS NOT NULL ';
END;
ELSIF P_GROUPBY = 'MAKHACHHANG' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'c.MANCC, c.TENNCC, b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE';
P_SELECT_COLUMNS_GROUPBY := ' b.MAVATTU AS Code, b.TENVATTU as TenVT,b.BARCODE as BARCODE,c.MANCC AS PARENT, c.TENNCC AS PARENT_NAME, a.UNITCODE AS MADONVI';
P_TABLE_GROUPBY:= ' INNER JOIN DM_NHACUNGCAP c ON b.MAKHACHHANG = c.MANCC AND c.UNITCODE = a.UNITCODE';
P_NOTNULL := ' AND c.MANCC IS NOT NULL ';
P_EXPRESSION:= '';
END;
ELSE --ELSIF P_GROUPBY = 'MAVATTU' THEN
BEGIN
P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU,b.BARCODE, a.UNITCODE';
P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU AS TenVT,b.BARCODE AS BARCODE,b.MAVATTU AS PARENT, b.TENVATTU AS PARENT_NAME, a.UNITCODE AS MADONVI';
P_TABLE_GROUPBY:= '';
P_NOTNULL := ' AND b.MAVATTU IS NOT NULL ';
END;
END IF;

QUERY_STR:= 'select '||P_SELECT_COLUMNS_GROUPBY||',
                                    SUM(a.tondaukysl)  as OpeningBalanceQuantity,
                                    SUM(a.tondaukygt) as OpeningBalanceValue,
                                    SUM(a.nhapsl) as IncreaseQuantity, SUM(a.nhapgt) as IncreaseValue,
                                    SUM(a.xuatsl) as DecreaseQuantity, SUM(a.xuatgt) as DecreaseValue,
                                    SUM(a.tondaukysl - a.xuatsl + a.nhapsl) as ClosingQuantity, 
                                    SUM(a.tondaukygt - a.xuatgt + a.nhapgt) as ClosingValue
                                    from TMP_XTN_NGAY a 
                                    INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu AND b.UNITCODE = a.UNITCODE AND a.USERNAME = '''||P_USERNAME||'''
                                    '||P_TABLE_GROUPBY||'
                                    WHERE 1=1  
                                    and (a.tondaukysl * a.tondaukysl) + (a.nhapsl * a.nhapsl) + (a.xuatsl * a.xuatsl) + (a.toncuoikysl* a.toncuoikysl)  > 0 '||P_NOTNULL||'
									'||P_EXPRESSION||'
                                    GROUP BY '||P_COLUMNS_GROUPBY || ' ORDER BY ' || P_COLUMNS_GROUPBY ;
--DBMS_OUTPUT.put_line (QUERY_STR);
BEGIN
OPEN cur FOR QUERY_STR;
 --DBMS_OUTPUT.put_line (QUERY_STR);
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM); 
END;
END BAOCAO_XNT_CHITIET;
--------------------------------------------------------
--  DDL for Procedure BAOCAO_XNT_CHITIET
--------------------------------------------------------