create or replace PROCEDURE  "BAOCAO_XNT_TONGHOP" (P_GROUPBY IN VARCHAR2, P_MAKHO IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2, P_NHACUNGCAP IN VARCHAR2,P_UNITCODE  IN VARCHAR2,P_USERNAME IN VARCHAR2,P_TUNGAY IN DATE,P_DENNGAY IN DATE,P_SELECTEDVAT IN VARCHAR2, XTNCOLLECTION  OUT SYS_REFCURSOR) AS
QUERY_STR VARCHAR(4000);
KY_KETTHUC NUMBER;
NAM_KETTHUC NUMBER;
P_SELECT_COLUMNS_GROUPBY VARCHAR(32767);
P_TABLE_GROUPBY VARCHAR(32767);
P_COLUMNS_GROUPBY VARCHAR(1000);
P_EXPRESSION VARCHAR(1000);
P_SQL_INSERT VARCHAR(10000);
P_INNER_JOIN VARCHAR(500);
P_NOTNULL VARCHAR(1000);
N NUMBER:= 0;
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

    IF P_SELECTEDVAT = '0' THEN ----Không lấy giá VAT
    BEGIN
        FOR cur_period in (SELECT to_date(P_TUNGAY, 'dd/MM/yyyy') + LEVEL - 1 AS today
                        FROM dual CONNECT BY LEVEL <= to_date(P_DENNGAY, 'dd/MM/yyyy') - to_date(P_TUNGAY, 'dd/MM/yyyy') + 1) 
        LOOP
              BEGIN
                BEGIN--TÄ‚Æ’Ã‚Â¬m kÄ‚Â¡Ã‚Â»Ã‚Â³
                SELECT KY, NAM INTO KY_KETTHUC, NAM_KETTHUC FROM DM_KYKETOAN WHERE to_date(DENNGAY, 'dd/MM/yyyy') = to_date(cur_period.today, 'dd/MM/yyyy') AND UNITCODE LIKE '%'||P_UNITCODE||'%' AND TRANGTHAI = 10 AND rownum =1;
                   EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                               goto end_loop;
                END;--End TÄ‚Æ’Ã‚Â¬m ky

                BEGIN --BÄ‚Â¡Ã‚ÂºÃ‚Â¯t Ä‚â€žÃ¢â‚¬ËœÄ‚Â¡Ã‚ÂºÃ‚Â§u thÄ‚Â¡Ã‚Â»Ã‚Â±c thi nhiÄ‚Â¡Ã‚Â»Ã¢â‚¬Â¡m vu
                N:=N+1;
                IF (N = 1) THEN
                BEGIN -- BÄ‚Â¡Ã‚ÂºÃ‚Â£n ghi Ä‚â€žÃ¢â‚¬ËœÄ‚Â¡Ã‚ÂºÃ‚Â§u tiÄ‚Æ’Ã‚Âªn
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
                IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
                P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
                END IF;   

                EXECUTE IMMEDIATE P_SQL_INSERT;
                EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
                END;
                ELSE
                BEGIN --VÄ‚Æ’Ã‚Â²ng thÄ‚Â¡Ã‚Â»Ã‚Â© 2 trÄ‚Â¡Ã‚Â»Ã…Â¸ Ä‚â€žÃ¢â‚¬Ëœi    

                P_SQL_INSERT := ' INSERT INTO TMP_XTN_NGAY
                         (UNITCODE, MAKHO, MAVATTU, GIAVON,NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                         SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.NHAPSL, a.NHAPGT, a.XUATSL, a.XUATGT,'''||P_USERNAME||''' AS USERNAME
                         FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b ON a.MAVATTU = b.MAVATTU  AND b.UNITCODE = a.UNITCODE
                         WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''  ';
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
                IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
                P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
                END IF;
                --DBMS_OUTPUT.PUT_LINE('P_SQL_INSERT: '||P_SQL_INSERT);
                execute IMMEDIATE P_SQL_INSERT;
                EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                               goto end_loop;
                               WHEN OTHERS THEN
                               goto end_loop;
                END;--KÄ‚Â¡Ã‚ÂºÃ‚Â¿t thÄ‚Æ’Ã‚Âºc vong 2 trÄ‚Â¡Ã‚Â»Ã…Â¸ Ä‚â€žÃ¢â‚¬Ëœi;
                END IF;
                END; -- KÄ‚Â¡Ã‚ÂºÃ‚Â¿t thÄ‚Æ’Ã‚Âºc thÄ‚Â¡Ã‚Â»Ã‚Â±c thi nhiÄ‚Â¡Ã‚Â»Ã¢â‚¬Â¡m vÄ‚Â¡Ã‚Â»Ã‚Â¥
              END;
                      <<end_loop>>
                      null;
        END LOOP;
    END;
    ELSE ----Lấy giá VAT
    BEGIN
        FOR cur_period in (SELECT to_date(P_TUNGAY, 'dd/MM/yyyy') + LEVEL - 1 AS today
                        FROM dual CONNECT BY LEVEL <= to_date(P_DENNGAY, 'dd/MM/yyyy') - to_date(P_TUNGAY, 'dd/MM/yyyy') + 1) 
        LOOP
              BEGIN
                BEGIN--TÄ‚Æ’Ã‚Â¬m kÄ‚Â¡Ã‚Â»Ã‚Â³
                SELECT KY, NAM INTO KY_KETTHUC, NAM_KETTHUC FROM DM_KYKETOAN WHERE to_date(DENNGAY, 'dd/MM/yyyy') = to_date(cur_period.today, 'dd/MM/yyyy') AND UNITCODE LIKE '%'||P_UNITCODE||'%' AND TRANGTHAI = 10 AND rownum =1;
                   EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                               goto end_loop;
                END;--End TÄ‚Æ’Ã‚Â¬m ky

                BEGIN --BÄ‚Â¡Ã‚ÂºÃ‚Â¯t Ä‚â€žÃ¢â‚¬ËœÄ‚Â¡Ã‚ÂºÃ‚Â§u thÄ‚Â¡Ã‚Â»Ã‚Â±c thi nhiÄ‚Â¡Ã‚Â»Ã¢â‚¬Â¡m vu
                N:=N+1;
                IF (N = 1) THEN
                BEGIN -- BÄ‚Â¡Ã‚ÂºÃ‚Â£n ghi Ä‚â€žÃ¢â‚¬ËœÄ‚Â¡Ã‚ÂºÃ‚Â§u tiÄ‚Æ’Ã‚Âªn
                 P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY
                         (UNITCODE, MAKHO, MAVATTU, GIAVON, TONDAUKYSL, TONDAUKYGT, NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                         SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.TONDAUKYSL, a.TONDAUKYGT, a.NHAPSL, a.NHAPGT*(1 + b.TYLE_VAT_VAO/100) as NHAPGT, a.XUATSL, (a.XUATGT*(1 + b.TYLE_VAT_VAO/100)) AS XUATGT ,'''||P_USERNAME||''' AS USERNAME
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
                IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
                P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
                END IF;   

                EXECUTE IMMEDIATE P_SQL_INSERT;
                EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                           goto end_loop;
                           WHEN OTHERS THEN
                           goto end_loop;
                END;
                ELSE
                BEGIN --VÄ‚Æ’Ã‚Â²ng thÄ‚Â¡Ã‚Â»Ã‚Â© 2 trÄ‚Â¡Ã‚Â»Ã…Â¸ Ä‚â€žÃ¢â‚¬Ëœi    

                P_SQL_INSERT := ' INSERT INTO TMP_XTN_NGAY
                         (UNITCODE, MAKHO, MAVATTU, GIAVON,NHAPSL, NHAPGT, XUATSL, XUATGT,USERNAME)
                         SELECT a.UNITCODE, a.MAKHO, a.MAVATTU, a.GIAVON, a.NHAPSL, a.NHAPGT, a.XUATSL, a.XUATGT,'''||P_USERNAME||''' AS USERNAME
                         FROM XNT_'|| NAM_KETTHUC ||'_KY_'||KY_KETTHUC||' a INNER JOIN DM_VATTU b ON a.MAVATTU = b.MAVATTU  AND b.UNITCODE = a.UNITCODE
                         WHERE 1=1 AND a.UNITCODE LIKE ''%'||P_UNITCODE||'%''  ';
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
                IF TRIM(P_NHACUNGCAP) IS NOT NULL THEN
                P_SQL_INSERT := P_SQL_INSERT||' AND b.MAKHACHHANG IN ('||P_NHACUNGCAP||')';
                END IF;
                --DBMS_OUTPUT.PUT_LINE('P_SQL_INSERT: '||P_SQL_INSERT);
                execute IMMEDIATE P_SQL_INSERT;
                EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                               goto end_loop;
                               WHEN OTHERS THEN
                               goto end_loop;
                END;--KÄ‚Â¡Ã‚ÂºÃ‚Â¿t thÄ‚Æ’Ã‚Âºc vong 2 trÄ‚Â¡Ã‚Â»Ã…Â¸ Ä‚â€žÃ¢â‚¬Ëœi;
                END IF;
                END; -- KÄ‚Â¡Ã‚ÂºÃ‚Â¿t thÄ‚Æ’Ã‚Âºc thÄ‚Â¡Ã‚Â»Ã‚Â±c thi nhiÄ‚Â¡Ã‚Â»Ã¢â‚¬Â¡m vÄ‚Â¡Ã‚Â»Ã‚Â¥
              END;
                      <<end_loop>>
                      null;      
        END LOOP;
    END;
    END IF;
    IF P_GROUPBY = 'MADONVI' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'c.MADONVI, c.TENDONVI,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' c.MADONVI AS Code, c.TENDONVI AS Ten,a.UNITCODE AS MADONVI';
    P_TABLE_GROUPBY:= ' INNER JOIN AU_DONVI c ON c.MADONVI = a.UNITCODE ';
    P_NOTNULL := ' AND c.MADONVI IS NOT NULL' ; 
    END;
    ELSIF P_GROUPBY = 'MAKHO' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'c.MAKHO, c.TENKHO,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' c.MAKHO AS Code, c.TENKHO AS Ten,a.UNITCODE AS MADONVI';
    P_TABLE_GROUPBY:= ' INNER JOIN DM_KHO c ON a.MAKHO = c.MAKHO AND c.UNITCODE = a.UNITCODE';
    P_NOTNULL := ' AND c.MAKHO IS NOT NULL' ; 
    END;
    ELSIF P_GROUPBY = 'MANHOMVATTU' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'c.MANHOMVTU, c.TENNHOMVT,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' c.MANHOMVTU AS Code, c.TENNHOMVT AS Ten,a.UNITCODE  AS MADONVI';
    P_TABLE_GROUPBY:= ' INNER JOIN DM_NHOMVATTU c ON b.MANHOMVATTU = c.MANHOMVTU AND c.UNITCODE = a.UNITCODE';
    P_NOTNULL := ' AND c.MANHOMVTU IS NOT NULL' ; 
    END;
    ELSIF P_GROUPBY = 'MALOAIVATTU' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'c.MALOAIVATTU, c.TENLOAIVT,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' c.MALOAIVATTU AS Code, c.TENLOAIVT AS Ten,a.UNITCODE  AS MADONVI';
    P_TABLE_GROUPBY:= ' INNER JOIN DM_LOAIVATTU c ON b.MALOAIVATTU = c.MALOAIVATTU AND c.UNITCODE = a.UNITCODE';
    P_NOTNULL := ' AND c.MALOAIVATTU IS NOT NULL' ; 
    END;
    ELSIF P_GROUPBY = 'MAKHACHHANG' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'c.MANCC, c.TENNCC,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' c.MANCC AS Code, c.TENNCC AS Ten,a.UNITCODE  AS MADONVI';
    P_TABLE_GROUPBY:= ' INNER JOIN DM_NHACUNGCAP c ON b.MAKHACHHANG = c.MANCC AND c.UNITCODE = a.UNITCODE';
    P_EXPRESSION:= '';
    P_NOTNULL := ' AND c.MANCC IS NOT NULL' ;
    END;
    ELSE --ELSIF P_GROUPBY = 'MAVATTU' THEN
    BEGIN
    P_COLUMNS_GROUPBY:= 'b.MAVATTU, b.TENVATTU,a.UNITCODE';
    P_SELECT_COLUMNS_GROUPBY:= ' b.MAVATTU AS Code, b.TENVATTU AS Ten,a.UNITCODE  AS MADONVI';
    P_TABLE_GROUPBY:= '';
    P_NOTNULL := ' AND b.MAVATTU IS NOT NULL' ;
    END;
    END IF;
    QUERY_STR:= 'select '||P_SELECT_COLUMNS_GROUPBY||', 
                                        SUM(a.tondaukysl)  as OpeningBalanceQuantity,
                                        SUM(a.tondaukygt) as OpeningBalanceValue,
                                        SUM(a.nhapsl) as IncreaseQuantity, SUM(a.nhapgt) as IncreaseValue,
                                        SUM(a.xuatsl) as DecreaseQuantity, SUM(a.xuatgt) as DecreaseValue,
                                        SUM(a.tondaukysl - a.xuatsl + a.nhapsl) as ClosingQuantity, 
                                        SUM(a.tondaukygt - a.xuatgt + a.nhapgt) as ClosingValue
                                        FROM TMP_XTN_NGAY a 
                                        INNER JOIN DM_VATTU b on a.Mavattu = b.mavattu AND b.UNITCODE = a.UNITCODE AND a.USERNAME = '''||P_USERNAME||'''
                                        '||P_TABLE_GROUPBY||'
                                        WHERE 1=1 
                                        '||P_EXPRESSION||'
                                        AND (a.tondaukysl * a.tondaukysl) + (a.nhapsl * a.nhapsl) + (a.xuatsl * a.xuatsl) + (a.toncuoikysl* a.toncuoikysl) > 0 
                                        '||P_NOTNULL||' ';
    QUERY_STR := QUERY_STR||' GROUP BY '||P_COLUMNS_GROUPBY || ' ORDER BY ' || P_COLUMNS_GROUPBY;

    BEGIN
    OPEN XTNCOLLECTION FOR QUERY_STR;
    EXCEPTION
       WHEN NO_DATA_FOUND
       THEN
          DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
       WHEN OTHERS
       THEN
          DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);
    END;
END BAOCAO_XNT_TONGHOP;
--------------------------------------------------------
--  DDL for Procedure BAOCAO_XNT_TONGHOP
--------------------------------------------------------