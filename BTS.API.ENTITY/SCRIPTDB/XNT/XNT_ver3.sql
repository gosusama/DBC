CREATE GLOBAL TEMPORARY TABLE "TBNETERP"."TMP_XTN_NGAY" 
   (	    "UNITCODE" NVARCHAR2(20),"MAKHO" NVARCHAR2(50),"MAVATTU" NVARCHAR2(50),
    "GIAVON" NUMBER(17,4) DEFAULT 0,
  "TONDAUKYSL" NUMBER(17,4) DEFAULT 0,
  "TONDAUKYGT" NUMBER(17,4) DEFAULT 0,
  "NHAPSL" NUMBER(17,4) DEFAULT 0,
    "NHAPGT" NUMBER(17,4) DEFAULT 0,
    "XUATSL" NUMBER(17,4) DEFAULT 0,
    "XUATGT" NUMBER(17,4) DEFAULT 0,
  "TONCUOIKYSL" NUMBER(17,4) DEFAULT 0, 
  "TONCUOIKYGT" NUMBER(17,4) DEFAULT 0
   ) ON COMMIT DELETE ROWS ;


CREATE OR REPLACE VIEW VIEW_VATTUGD_XNT
as SELECT VATTUCHUNGTU.ID, UNITCODE,
	   TRANGTHAI,
       LOAICHUNGTU,
       TO_DATE(NGAYCHUNGTU,'DD/MM/YY') AS NGAYCHUNGTU,
	   VATTUCHUNGTUCHITIET.MAHANG as MAVATTU,
       MAKHOXUAT,
       MAKHONHAP,
       MADONVINHAN,
       MADONVIXUAT,
       SOLUONG,
       DONGIA,
	   I_CREATE_BY AS NGUOITAO,
	   '' AS MAMAYBAN
  FROM VATTUCHUNGTU
       INNER JOIN VATTUCHUNGTUCHITIET
          ON (VATTUCHUNGTU.MACHUNGTUPK = VATTUCHUNGTUCHITIET.MACHUNGTUPK)
          WHERE TRANGTHAI=10;
/

create or replace PACKAGE            "XNT" AS
   
   PROCEDURE XNT_KHOASO(P_TABLENAME_KYTRUOC IN VARCHAR2,P_TABLENAME IN VARCHAR2, P_UNITCODE IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER);
   PROCEDURE XNT_CREATE_TABLE_TONKY(P_TABLENAME_KYTRUOC IN VARCHAR2,P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER);  
   PROCEDURE XNT_CREATE_TONDAUKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE );
   PROCEDURE XNT_UPDATE_TONKYTRUOC(P_TABLENAME_KYTRUOC IN VARCHAR2,P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER);  
   PROCEDURE XNT_TANG_TONKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE );
   PROCEDURE XNT_GIAM_TONKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE );
   
   PROCEDURE XNT_TANG_PHIEU(P_TABLENAME IN VARCHAR2, P_NAM IN VARCHAR2,P_KY IN NUMBER,P_ID IN VARCHAR2);
   PROCEDURE XNT_GIAM_PHIEU(P_TABLENAME IN VARCHAR2, P_NAM IN VARCHAR2,P_KY IN NUMBER,P_ID IN VARCHAR2);
   
   PROCEDURE BAOCAO_XNT(P_WAREHOUSE IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER, XTNCOLLECTION OUT SYS_REFCURSOR); 
   PROCEDURE BAOCAO_XNT_NGAY(P_WAREHOUSE IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, XTNCOLLECTION  OUT SYS_REFCURSOR);
   PROCEDURE XNT_PHATSINH_TANG_NGAY(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2);
   PROCEDURE XNT_PHATSINH_GIAM_NGAY(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2);
   PROCEDURE TINH_PHATSINH_KY_DENNGAY_TANG(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2);
   PROCEDURE TINH_PHATSINH_KY_DENNGAY_GIAM(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2);
    
 END XNT;
/


/


/create or replace PACKAGE BODY  "XNT" AS
--------------------------------------------------------
--  DDL for Procedure XNT_CREATE_TABLE_TONKY_TRUOC
--------------------------------------------------------

   PROCEDURE XNT_CREATE_TABLE_TONKY(P_TABLENAME_KYTRUOC IN VARCHAR2, P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER) IS
   P_SQL  VARCHAR2(32767);  
   P_SQL_INSERT  VARCHAR2(32767);  
   N_COUNT NUMBER(17,4):=0;
   BEGIN
    P_SQL := 'CREATE TABLE '||P_TABLENAME||'(
    "UNITCODE" NVARCHAR2(20), "NAM" NVARCHAR2(50),"KY" NVARCHAR2(200),"MAKHO" NVARCHAR2(50),"MAVATTU" NVARCHAR2(50),
    "GIAVON" NUMBER(17,4) DEFAULT 0,
  "TONDAUKYSL" NUMBER(17,4) DEFAULT 0,
  "TONDAUKYGT" NUMBER(17,4) DEFAULT 0,
  "NHAPSL" NUMBER(17,4) DEFAULT 0,
    "NHAPGT" NUMBER(17,4) DEFAULT 0,
    "XUATSL" NUMBER(17,4) DEFAULT 0,
    "XUATGT" NUMBER(17,4) DEFAULT 0,
  "TONCUOIKYSL" NUMBER(17,4) DEFAULT 0, 
  "TONCUOIKYGT" NUMBER(17,4) DEFAULT 0
 ) SEGMENT CREATION IMMEDIATE PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE
  (
    INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT
  )';

   P_SQL_INSERT :=       'INSERT INTO '||P_TABLENAME||' a
(a.UNITCODE, a.NAM, a.KY, a.MAKHO, a.MAVATTU, a.GIAVON, a.TONCUOIKYGT, a.TONCUOIKYSL, a.TONDAUKYGT, a.TONDAUKYSL)
SELECT UNITCODE, NAM, KY, MAKHO, MAVATTU, GIAVON, TONCUOIKYGT, TONCUOIKYSL, TONCUOIKYGT, TONCUOIKYSL FROM '||P_TABLENAME_KYTRUOC||' b';
IF TRIM(P_TABLENAME_KYTRUOC) IS NOT NULL THEN
 
 P_SQL_INSERT := 'DECLARE CURSOR KHO_Table IS SELECT MAKHO FROM DMKHO WHERE UNITCODE=''' || P_UNITCODE || ''';
        BEGIN FOR KHO_Entiy IN KHO_Table LOOP
         BEGIN 
            INSERT INTO '|| P_TABLENAME ||'(UNITCODE,NAM,KY,MAKHO,MAVATTU) 
             SELECT '''|| P_UNITCODE ||''' AS UNITCODE,'||P_NAM||' AS NAM,'||P_KY||' AS KY,KHO_Entiy.MAKHO AS MAKHO,MAVATTU FROM DMVATTU WHERE UNITCODE='''|| P_UNITCODE ||''';
            
          END;
         END LOOP;
        END;';
        END IF;
  BEGIN
    SELECT COUNT(*)  INTO N_COUNT  FROM ALL_TAB_COLUMNS  WHERE TABLE_NAME = UPPER(P_TABLENAME);
    EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
  END;
  IF(N_COUNT IS NULL OR N_COUNT<=0) THEN
    BEGIN
        DBMS_OUTPUT.PUT_LINE(P_SQL_INSERT);
        EXECUTE IMMEDIATE P_SQL;    
        EXECUTE IMMEDIATE P_SQL_INSERT;    
    END;
    END IF;  
  END XNT_CREATE_TABLE_TONKY;

--------------------------------------------------------
--  DDL for Procedure XNT_UPDATE_TONKYTRUOC
--------------------------------------------------------
  PROCEDURE XNT_CREATE_TONDAUKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE )
  IS
  N_SQL_INSERT VARCHAR(32767);
  BEGIN
   N_SQL_INSERT:='DECLARE 
    N_COUNT NUMBER :=0; P_TONDAUKYSL NUMBER :=0; P_TONDAUKYGT NUMBER :=0;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHONHAP,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS NHAPGT FROM VIEW_VATTUGD_XNT V WHERE 
    UNITCODE='''||P_MADONVI||''' AND 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHONHAP,UNITCODE ;  
   
  BEGIN FOR VT_INDEX IN NHAP_VATTUGD LOOP
    N_COUNT :=0;
      BEGIN 
        SELECT COUNT(*) INTO N_COUNT FROM '||P_TABLENAME||' WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  MAKHO=VT_INDEX.MAKHONHAP AND UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;
    BEGIN
      IF(N_COUNT=0) THEN 
        BEGIN 
      INSERT INTO '||P_TABLENAME||' (UNITCODE,NAM,KY,MAKHO,MAVATTU) 
      SELECT VT_INDEX.UNITCODE AS UNITCODE,'|| P_NAM ||' AS NAM,'|| P_KY ||' AS KY,VT_INDEX.MAKHONHAP AS MAKHO,VT_INDEX.MAVATTU FROM DMVATTU WHERE UNITCODE=VT_INDEX.UNITCODE AND MAVATTU=VT_INDEX.MAVATTU;
      COMMIT;
        END;
       END IF;
     END;
     
    
    BEGIN     
      UPDATE '||P_TABLENAME||' SET
      TONDAUKYSL=VT_INDEX.SOLUONG, TONDAUKYGT=VT_INDEX.NHAPGT,TONCUOIKYSL=VT_INDEX.SOLUONG,TONCUOIKYGT=VT_INDEX.NHAPGT
      WHERE UNITCODE =VT_INDEX.UNITCODE AND  MAVATTU=VT_INDEX.MAVATTU AND MAKHO=VT_INDEX.MAKHONHAP; 
        COMMIT;
    END;
  
    END LOOP; 
    END;';
    
        DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);        
      EXECUTE IMMEDIATE N_SQL_INSERT;
        EXCEPTION WHEN OTHERS THEN DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);
  END XNT_CREATE_TONDAUKY;
   PROCEDURE XNT_UPDATE_TONKYTRUOC(P_TABLENAME_KYTRUOC IN VARCHAR2,P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER) IS
   P_SQL  VARCHAR2(32767);
   BEGIN

      P_SQL:='
        BEGIN
            UPDATE '||P_TABLENAME||' SET
            (TONDAUKYSL, 
            TONDAUKYGT,
            TONCUOIKYSL,
            TONCUOIKYGT
			) = (SELECT
             TONCUOIKYSL,
             TONCUOIKYGT,
             TONCUOIKYSL,
             TONCUOIKYGT              
            FROM '||P_TABLENAME_KYTRUOC||' WHERE '||P_TABLENAME||'.UNITCODE = '||P_TABLENAME_KYTRUOC||'.UNITCODE AND '||P_TABLENAME||'.MAKHO = '||P_TABLENAME_KYTRUOC||'.MAKHO AND '||P_TABLENAME||'.MAVATTU = '||P_TABLENAME_KYTRUOC||'.MAVATTU) 
            WHERE EXISTS (SELECT MAVATTU FROM  '||P_TABLENAME_KYTRUOC||' WHERE '||P_TABLENAME||'.UNITCODE = '||P_TABLENAME_KYTRUOC||'.UNITCODE AND '||P_TABLENAME||'.MAKHO = '||P_TABLENAME_KYTRUOC||'.MAKHO AND '||P_TABLENAME||'.MAVATTU = '||P_TABLENAME_KYTRUOC||'.MAVATTU);
            COMMIT;
        END;';



    DBMS_OUTPUT.PUT_LINE(P_SQL);
    EXECUTE IMMEDIATE P_SQL;    
    EXCEPTION WHEN OTHERS THEN  DBMS_OUTPUT.PUT_LINE(P_SQL);
        
  END XNT_UPDATE_TONKYTRUOC;

--------------------------------------------------------

PROCEDURE XNT_TANG_TONKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE ) IS
    N_SQL_INSERT VARCHAR(32767);
BEGIN
     
  N_SQL_INSERT:='DECLARE 
    N_COUNT NUMBER :=0; P_TONDAUKYSL NUMBER :=0; P_TONDAUKYGT NUMBER :=0;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHONHAP,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS NHAPGT FROM VIEW_VATTUGD_XNT V WHERE 
    UNITCODE='''||P_MADONVI||''' AND 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHONHAP,UNITCODE ;  
   
  BEGIN FOR VT_INDEX IN NHAP_VATTUGD LOOP
    N_COUNT :=0;
      BEGIN 
        SELECT COUNT(*) INTO N_COUNT FROM '||P_TABLENAME||' WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  MAKHO=VT_INDEX.MAKHONHAP AND UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;
    BEGIN
      IF(N_COUNT=0) THEN 
        BEGIN 
      INSERT INTO '||P_TABLENAME||' (UNITCODE,NAM,KY,MAKHO,MAVATTU) 
      SELECT VT_INDEX.UNITCODE AS UNITCODE,'|| P_NAM ||' AS NAM,'|| P_KY ||' AS KY,VT_INDEX.MAKHONHAP AS MAKHO,VT_INDEX.MAVATTU FROM DMVATTU WHERE UNITCODE=VT_INDEX.UNITCODE AND MAVATTU=VT_INDEX.MAVATTU;
      COMMIT;
        END;
       END IF;
     END;
     
    
    BEGIN     
      UPDATE '||P_TABLENAME||' SET
      NHAPSL=NHAPSL+VT_INDEX.SOLUONG, NHAPGT=NHAPGT+VT_INDEX.NHAPGT,TONCUOIKYSL=TONCUOIKYSL+VT_INDEX.SOLUONG,TONCUOIKYGT=TONCUOIKYGT+VT_INDEX.NHAPGT
      WHERE UNITCODE =VT_INDEX.UNITCODE AND  MAVATTU=VT_INDEX.MAVATTU AND MAKHO=VT_INDEX.MAKHONHAP; 
        COMMIT;
    END;
  
    END LOOP; 
    END;';
    
        DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);        
      EXECUTE IMMEDIATE N_SQL_INSERT;
        EXCEPTION WHEN OTHERS THEN DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);
END XNT_TANG_TONKY;
---------------------------------------------------
PROCEDURE XNT_GIAM_TONKY(P_TABLENAME IN VARCHAR2, P_MADONVI IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_MANHOMPTNX IN VARCHAR2,P_TUNGAY DATE,P_DENNGAY DATE ) IS
    N_SQL_INSERT VARCHAR(32767);
  BEGIN
    N_SQL_INSERT:='DECLARE 
    N_COUNT NUMBER :=0; P_TONDAUKYSL NUMBER :=0; P_TONDAUKYGT NUMBER :=0;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHOXUAT,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS XUATGT FROM VIEW_VATTUGD_XNT V WHERE 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHOXUAT,UNITCODE ;  
   
  BEGIN FOR VT_INDEX IN NHAP_VATTUGD LOOP
    N_COUNT :=0;
      BEGIN 
        SELECT COUNT(*) INTO N_COUNT FROM '||P_TABLENAME||' WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  MAKHO=VT_INDEX.MAKHOXUAT AND UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;
    BEGIN
      IF(N_COUNT=0) THEN 
        BEGIN 
      INSERT INTO '||P_TABLENAME||' (UNITCODE,NAM,KY,MAKHO,MAVATTU) 
      SELECT VT_INDEX.UNITCODE AS UNITCODE,'|| P_NAM ||' AS NAM,'|| P_KY ||' AS KY,VT_INDEX.MAKHOXUAT AS MAKHO,VT_INDEX.MAVATTU FROM DMVATTU WHERE UNITCODE=VT_INDEX.UNITCODE AND MAVATTU=VT_INDEX.MAVATTU;
      COMMIT;
        END;
       END IF;
     END;
    
    BEGIN     
      UPDATE '||P_TABLENAME||' SET
      XUATSL=XUATSL+VT_INDEX.SOLUONG, XUATGT=XUATGT+VT_INDEX.XUATGT, TONCUOIKYSL=TONCUOIKYSL-VT_INDEX.SOLUONG,TONCUOIKYGT=TONCUOIKYGT-VT_INDEX.XUATGT
      WHERE UNITCODE =VT_INDEX.UNITCODE AND  MAVATTU=VT_INDEX.MAVATTU AND MAKHO=VT_INDEX.MAKHOXUAT;      
        COMMIT;
    END;
  
    END LOOP; 
    END;';
    
        DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);        
      EXECUTE IMMEDIATE N_SQL_INSERT;
        EXCEPTION WHEN OTHERS THEN COMMIT;
  END XNT_GIAM_TONKY;
  PROCEDURE XNT_KHOASO(P_TABLENAME_KYTRUOC IN VARCHAR2,P_TABLENAME IN VARCHAR2, P_UNITCODE IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER)IS
    P_TUNGAY DATE;
    P_DENNGAY DATE;
	P_CHECK_TABLE_EXISTS NUMBER;
    BEGIN
      SELECT TO_DATE(TUNGAY,'DD/MM/YY') AS TUNGAY,TO_DATE(DENNGAY,'DD/MM/YY') AS DENNGAY  INTO P_TUNGAY,P_DENNGAY FROM DMKYKETOAN WHERE KY=P_KY AND NAM=P_NAM AND UNITCODE=P_UNITCODE;
  
      BEGIN
        XNT_CREATE_TABLE_TONKY(P_TABLENAME_KYTRUOC,P_TABLENAME, P_UNITCODE, P_NAM, P_KY);        
        
        XNT_TANG_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'NMUA',P_TUNGAY,P_DENNGAY);
        XNT_TANG_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'NHBANTL',P_TUNGAY,P_DENNGAY);
        XNT_TANG_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'DCN',P_TUNGAY,P_DENNGAY);
		 XNT_TANG_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'NKHAC',P_TUNGAY,P_DENNGAY);
        
        -- xuat
        XNT_GIAM_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'XBAN',P_TUNGAY,P_DENNGAY);
		XNT_GIAM_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'XBANLE',P_TUNGAY,P_DENNGAY);
		XNT_GIAM_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'XKHAC',P_TUNGAY,P_DENNGAY);
        XNT_GIAM_TONKY(P_TABLENAME, P_UNITCODE, P_NAM, P_KY, 'DCX',P_TUNGAY,P_DENNGAY);
        
      END;
    
      END XNT_KHOASO;
PROCEDURE BAOCAO_XNT(P_WAREHOUSE IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_TABLENAME  IN VARCHAR2, P_UNITCODE  IN VARCHAR2, P_NAM  IN NUMBER,P_KY  IN NUMBER, XTNCOLLECTION  OUT SYS_REFCURSOR) IS
QUERY_STR VARCHAR(32767) := 'select a.MAVATTU as Code, b.TENVATTU as Name, b.donvitinh as Unit, 
                                    a.tondaukysl as OpeningBalanceQuantity, a.tondaukygt as OpeningBalanceValue,
                                    a.nhapsl as IncreaseQuantity, a.nhapgt as IncreaseValue,
                                    a.xuatsl as DecreaseQuantity, a.xuatgt as DecreaseValue,
                                    a.toncuoikysl as ClosingQuantity, a.toncuoikygt as ClosingValue, a.makho as WareHouseCode, a.unitcode as UnitCode
                                    from '|| P_TABLENAME || ' a left join dmvattu b on a.Mavattu = b.mavattu
                                    WHERE a.UNITCODE = '''||P_UNITCODE||''' AND a.NAM = '||P_NAM||' AND a.KY = '||P_KY;

BEGIN

IF TRIM(P_WAREHOUSE) IS NOT NULL THEN

QUERY_STR := QUERY_STR||' AND a.MAKHO IN ('||P_WAREHOUSE||')';
END IF;

IF TRIM(P_MALOAI) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
END IF;
IF TRIM(P_MANHOM) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MAVATTU IN ('||P_MAVATTU||')';
END IF;

open XTNCOLLECTION for QUERY_STR;
DBMS_OUTPUT.put_line (QUERY_STR);
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);
END BAOCAO_XNT;



PROCEDURE BAOCAO_XNT_NGAY(P_WAREHOUSE IN VARCHAR2, P_MALOAI IN VARCHAR2,  P_MANHOM IN VARCHAR2,P_MAVATTU IN VARCHAR2,P_UNITCODE  IN VARCHAR2, P_TUNGAY IN DATE,P_DENNGAY IN DATE, XTNCOLLECTION  OUT SYS_REFCURSOR) IS
QUERY_STR VARCHAR(32767);
KY_KETTHUC NUMBER;
NAM_KETTHUC NUMBER;
KY_BATDAU NUMBER;
NAM_BATDAU NUMBER;
P_SQL_INSERT VARCHAR(32767);
NGAYBATDAU_CUAKY DATE;
NGAYBATDAU_CUADAUKY DATE;
BEGIN
SELECT KY, NAM, TUNGAY INTO KY_BATDAU, NAM_BATDAU, NGAYBATDAU_CUADAUKY FROM DMKYKETOAN WHERE  P_TUNGAY BETWEEN  TUNGAY AND DENNGAY  AND UNITCODE = P_UNITCODE;
SELECT KY, NAM, TUNGAY INTO KY_KETTHUC, NAM_KETTHUC, NGAYBATDAU_CUAKY FROM DMKYKETOAN WHERE P_DENNGAY BETWEEN  TUNGAY AND DENNGAY   AND UNITCODE = P_UNITCODE;


 P_SQL_INSERT := 'INSERT INTO TMP_XTN_NGAY (UNITCODE,MAKHO,MAVATTU, TONDAUKYSL, TONDAUKYGT,TONCUOIKYSL, TONCUOIKYGT) SELECT 
UNITCODE,MAKHO,MAVATTU,TONCUOIKYSL, TONCUOIKYGT, TONCUOIKYSL, TONCUOIKYGT FROM XNT_'||NAM_KETTHUC||'_KY_'||KY_KETTHUC;
BEGIN
EXECUTE IMMEDIATE P_SQL_INSERT;    
EXCEPTION WHEN OTHERS THEN DBMS_OUTPUT.put_line (P_SQL_INSERT);
END;
---Tính tòn cu?i k? tính ??n ngày P_DENNGAY
--BEGIN
--TINH_PHATSINH_KY_DENNGAY_TANG(NGAYBATDAU_CUAKY, P_DENNGAY, 'NMUA');
--TINH_PHATSINH_KY_DENNGAY_TANG(NGAYBATDAU_CUAKY, P_DENNGAY, 'DCN');
--TINH_PHATSINH_KY_DENNGAY_TANG(NGAYBATDAU_CUAKY, P_DENNGAY, 'NHBANTL');
--TINH_PHATSINH_KY_DENNGAY_TANG(NGAYBATDAU_CUAKY, P_DENNGAY, 'NKHAC');
--END;
--BEGIN
--TINH_PHATSINH_KY_DENNGAY_GIAM(NGAYBATDAU_CUAKY, P_DENNGAY, 'XBAN');
--TINH_PHATSINH_KY_DENNGAY_GIAM(NGAYBATDAU_CUAKY, P_DENNGAY, 'DCX');
--TINH_PHATSINH_KY_DENNGAY_GIAM(NGAYBATDAU_CUAKY, P_DENNGAY, 'XKHAC');
--END;  
--Tính s? phát sính t? P_TUNGAY cho ??n P_DENNGAY VÀ tính luôn t?n ??u k?
BEGIN
XNT_PHATSINH_TANG_NGAY(P_TUNGAY, P_DENNGAY, 'NMUA');
XNT_PHATSINH_TANG_NGAY(P_TUNGAY, P_DENNGAY, 'DCN');
XNT_PHATSINH_TANG_NGAY(P_TUNGAY, P_DENNGAY, 'NHBANTL');
XNT_PHATSINH_TANG_NGAY(P_TUNGAY, P_DENNGAY, 'NKHAC');
END;
BEGIN
XNT_PHATSINH_GIAM_NGAY(P_TUNGAY, P_DENNGAY, 'XBAN');
XNT_PHATSINH_GIAM_NGAY(P_TUNGAY, P_DENNGAY, 'DCX');
XNT_PHATSINH_GIAM_NGAY(P_TUNGAY, P_DENNGAY, 'XKHAC');
END;  
--open XTNCOLLECTION for SELECT * FROM TMP_XTN_NGAY;
QUERY_STR:= 'select a.MAVATTU as Code, b.TENVATTU as Name, b.donvitinh as Unit, 
                                    a.tondaukysl as OpeningBalanceQuantity, a.tondaukygt as OpeningBalanceValue,
                                    a.nhapsl as IncreaseQuantity, a.nhapgt as IncreaseValue,
                                    a.xuatsl as DecreaseQuantity, a.xuatgt as DecreaseValue,
                                    a.toncuoikysl as ClosingQuantity, a.toncuoikygt as ClosingValue, a.makho as WareHouseCode, a.unitcode as UnitCode
                                    from TMP_XTN_NGAY a left join dmvattu b on a.Mavattu = b.mavattu WHERE 1=1';
IF TRIM(P_WAREHOUSE) IS NOT NULL THEN

QUERY_STR := QUERY_STR||' AND a.MAKHO IN ('||P_WAREHOUSE||')';
END IF;

IF TRIM(P_MALOAI) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MALOAIVATTU IN ('||P_MALOAI||')';
END IF;
IF TRIM(P_MANHOM) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MANHOMVATTU IN ('||P_MANHOM||')';
END IF;
IF TRIM(P_MAVATTU) IS NOT NULL THEN
QUERY_STR := QUERY_STR||' AND b.MAVATTU IN ('||P_MAVATTU||')';
END IF;                                    
OPEN XTNCOLLECTION FOR QUERY_STR;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);
END BAOCAO_XNT_NGAY;
PROCEDURE XNT_PHATSINH_TANG_NGAY(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2) IS
N_SQL_INSERT VARCHAR(32767);
BEGIN
N_SQL_INSERT:='
	DECLARE
  N_COUNT NUMBER;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHONHAP,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS NHAPGT FROM VIEW_VATTUGD_XNT V WHERE 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHONHAP,UNITCODE ; 
	BEGIN 
	FOR VT_INDEX IN NHAP_VATTUGD 
	LOOP
    N_COUNT :=0;
      BEGIN
        SELECT COUNT(*) INTO N_COUNT FROM TMP_XTN_NGAY  WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;	

	IF N_COUNT = 0 THEN
	INSERT INTO TMP_XTN_NGAY (UNITCODE, MAKHO, MAVATTU, NHAPSL, NHAPGT) 
	VALUES (VT_INDEX.UNITCODE, VT_INDEX.MAKHONHAP, VT_INDEX.MAVATTU, VT_INDEX.SOLUONG, VT_INDEX.NHAPGT);
	END IF;
	IF N_COUNT > 0 THEN                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
		UPDATE TMP_XTN_NGAY SET
		NHAPSL = NHAPSL + VT_INDEX.SOLUONG,
		NHAPGT = NHAPGT + VT_INDEX.NHAPGT,
    TONDAUKYSL = TONDAUKYSL - VT_INDEX.SOLUONG,
    TONDAUKYGT = TONDAUKYGT - VT_INDEX.NHAPGT
		WHERE UNITCODE = VT_INDEX.UNITCODE AND MAKHO = VT_INDEX.MAKHONHAP AND MAVATTU = VT_INDEX.MAVATTU;
	END IF;
  END LOOP;
	END;
	';
	EXECUTE IMMEDIATE N_SQL_INSERT;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (N_SQL_INSERT  || SQLERRM);
END XNT_PHATSINH_TANG_NGAY;
PROCEDURE XNT_PHATSINH_GIAM_NGAY(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2) IS
N_SQL_INSERT VARCHAR(32767);
BEGIN
N_SQL_INSERT:='
	DECLARE
  N_COUNT NUMBER;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHOXUAT,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS XUATGT FROM VIEW_VATTUGD_XNT V WHERE 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHOXUAT,UNITCODE ; 
	BEGIN 
	FOR VT_INDEX IN NHAP_VATTUGD 
	LOOP
    N_COUNT :=0;
      BEGIN
        SELECT COUNT(*) INTO N_COUNT FROM TMP_XTN_NGAY  WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;	

	IF N_COUNT = 0 THEN
	INSERT INTO TMP_XTN_NGAY (UNITCODE, MAKHO, MAVATTU, XUATSL, XUATGT) 
	VALUES (VT_INDEX.UNITCODE, VT_INDEX.MAKHOXUAT, VT_INDEX.MAVATTU, VT_INDEX.SOLUONG, VT_INDEX.XUATGT);
	END IF;
	IF N_COUNT > 0 THEN                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
		UPDATE TMP_XTN_NGAY SET
		XUATSL = XUATSL + VT_INDEX.SOLUONG,
		XUATGT = XUATGT + VT_INDEX.XUATGT,
    TONDAUKYSL = TONDAUKYSL + VT_INDEX.SOLUONG,
    TONDAUKYGT = TONDAUKYGT + VT_INDEX.XUATGT   
		WHERE UNITCODE = VT_INDEX.UNITCODE AND MAKHO = VT_INDEX.MAKHOXUAT AND MAVATTU = VT_INDEX.MAVATTU;
	END IF;
  END LOOP;
	END;
	';
	EXECUTE IMMEDIATE N_SQL_INSERT;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (N_SQL_INSERT  || SQLERRM);
END XNT_PHATSINH_GIAM_NGAY;
PROCEDURE TINH_PHATSINH_KY_DENNGAY_TANG(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2) IS
N_SQL_INSERT VARCHAR(32767);
BEGIN
N_SQL_INSERT:='
	DECLARE
  N_COUNT NUMBER;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHONHAP,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS NHAPGT FROM VIEW_VATTUGD_XNT V WHERE 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHONHAP,UNITCODE ; 
	BEGIN 
	FOR VT_INDEX IN NHAP_VATTUGD 
	LOOP
    N_COUNT :=0;
      BEGIN
        SELECT COUNT(*) INTO N_COUNT FROM TMP_XTN_NGAY  WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;	

	IF N_COUNT = 0 THEN
	INSERT INTO TMP_XTN_NGAY (UNITCODE, MAKHO, MAVATTU, NHAPSL, NHAPGT) 
	VALUES (VT_INDEX.UNITCODE, VT_INDEX.MAKHONHAP, VT_INDEX.MAVATTU, VT_INDEX.SOLUONG, VT_INDEX.NHAPGT);
	END IF;
	IF N_COUNT > 0 THEN                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
		UPDATE TMP_XTN_NGAY SET
    		NHAPSL = NHAPSL + VT_INDEX.SOLUONG,
		NHAPGT = NHAPGT + VT_INDEX.NHAPGT,
		TONCUOIKYSL = TONCUOIKYSL + VT_INDEX.SOLUONG,
		TONCUOIKYGT = TONCUOIKYGT + VT_INDEX.NHAPGT
		WHERE UNITCODE = VT_INDEX.UNITCODE AND MAKHO = VT_INDEX.MAKHONHAP AND MAVATTU = VT_INDEX.MAVATTU;
	END IF;
  END LOOP;
	END;
	';
	EXECUTE IMMEDIATE N_SQL_INSERT;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (N_SQL_INSERT  || SQLERRM);
END TINH_PHATSINH_KY_DENNGAY_TANG;
PROCEDURE TINH_PHATSINH_KY_DENNGAY_GIAM(P_TUNGAY IN DATE, P_DENNGAY IN DATE,P_MANHOMPTNX IN VARCHAR2) IS
N_SQL_INSERT VARCHAR(32767);
BEGIN
N_SQL_INSERT:='
	DECLARE
  N_COUNT NUMBER;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHOXUAT,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS XUATGT FROM VIEW_VATTUGD_XNT V WHERE 
    LOAICHUNGTU='''||P_MANHOMPTNX||''' AND TRANGTHAI=10
    AND NGAYCHUNGTU <=TO_DATE('''|| P_DENNGAY ||''',''DD/MM/YY'')
    AND NGAYCHUNGTU >=TO_DATE('''|| P_TUNGAY ||''',''DD/MM/YY'')
    GROUP BY MAVATTU,MAKHOXUAT,UNITCODE ; 
	BEGIN 
	FOR VT_INDEX IN NHAP_VATTUGD 
	LOOP
    N_COUNT :=0;
      BEGIN
        SELECT COUNT(*) INTO N_COUNT FROM TMP_XTN_NGAY  WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;	

	IF N_COUNT = 0 THEN
	INSERT INTO TMP_XTN_NGAY (UNITCODE, MAKHO, MAVATTU, XUATSL, XUATGT) 
	VALUES (VT_INDEX.UNITCODE, VT_INDEX.MAKHOXUAT, VT_INDEX.MAVATTU, VT_INDEX.SOLUONG, VT_INDEX.XUATGT);
	END IF;
	IF N_COUNT > 0 THEN                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
		UPDATE TMP_XTN_NGAY SET
		TONCUOIKYSL = TONCUOIKYSL - VT_INDEX.SOLUONG,
		TONCUOIKYGT = TONCUOIKYGT - VT_INDEX.XUATGT
		WHERE UNITCODE = VT_INDEX.UNITCODE AND MAKHO = VT_INDEX.MAKHOXUAT AND MAVATTU = VT_INDEX.MAVATTU;
	END IF;
  END LOOP;
	END;
	';
	EXECUTE IMMEDIATE N_SQL_INSERT;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (N_SQL_INSERT  || SQLERRM);
END TINH_PHATSINH_KY_DENNGAY_GIAM;
  PROCEDURE XNT_GIAM_PHIEU(P_TABLENAME IN VARCHAR2,P_NAM IN VARCHAR2,P_KY IN NUMBER,P_ID IN VARCHAR2 ) IS
    N_SQL_INSERT VARCHAR(32767);
  BEGIN
  N_SQL_INSERT:='DECLARE 
    N_COUNT NUMBER :=0; P_TONDAUKYSL NUMBER :=0; P_TONDAUKYGT NUMBER :=0;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHOXUAT,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS XUATGT FROM VIEW_VATTUGD_XNT V WHERE 
    ID='''||P_ID||'''    
    GROUP BY MAVATTU,MAKHOXUAT,UNITCODE ;  
   
  BEGIN FOR VT_INDEX IN NHAP_VATTUGD LOOP
    N_COUNT :=0;
      BEGIN 
        SELECT COUNT(*) INTO N_COUNT FROM '||P_TABLENAME||' WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  MAKHO=VT_INDEX.MAKHOXUAT AND UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;
    BEGIN
      IF(N_COUNT=0) THEN 
        BEGIN 
      INSERT INTO '||P_TABLENAME||' (UNITCODE,NAM,KY,MAKHO,MAVATTU) 
      SELECT VT_INDEX.UNITCODE AS UNITCODE,'|| P_NAM ||' AS NAM,'|| P_KY ||' AS KY,VT_INDEX.MAKHOXUAT AS MAKHO,VT_INDEX.MAVATTU FROM DMVATTU WHERE UNITCODE=VT_INDEX.UNITCODE AND MAVATTU=VT_INDEX.MAVATTU;
      COMMIT;
        END;
       END IF;
     END;
    
    BEGIN     
      UPDATE '||P_TABLENAME||' SET
      XUATSL=XUATSL+VT_INDEX.SOLUONG, XUATGT=XUATGT+VT_INDEX.XUATGT, TONCUOIKYSL=TONCUOIKYSL-VT_INDEX.SOLUONG,TONCUOIKYGT=TONCUOIKYGT-VT_INDEX.XUATGT
      WHERE UNITCODE =VT_INDEX.UNITCODE AND  MAVATTU=VT_INDEX.MAVATTU AND MAKHO=VT_INDEX.MAKHOXUAT;      
        COMMIT;
    END;
  
    END LOOP; 
    END;';
    
      EXECUTE IMMEDIATE N_SQL_INSERT;
        EXCEPTION WHEN OTHERS THEN  DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);        
  END XNT_GIAM_PHIEU;
  PROCEDURE XNT_TANG_PHIEU(P_TABLENAME IN VARCHAR2, P_NAM IN VARCHAR2,P_KY IN NUMBER,P_ID IN VARCHAR2 ) IS
    N_SQL_INSERT VARCHAR(32767);    
BEGIN

    N_SQL_INSERT:='DECLARE 
    N_COUNT NUMBER :=0; P_TONDAUKYSL NUMBER :=0; P_TONDAUKYGT NUMBER :=0;
    CURSOR NHAP_VATTUGD IS SELECT MAVATTU,MAKHONHAP,UNITCODE,SUM(SOLUONG) AS SOLUONG,SUM(SOLUONG*DONGIA) AS NHAPGT FROM VIEW_VATTUGD_XNT V WHERE 
    ID='''||P_ID||'''     
    GROUP BY MAVATTU,MAKHONHAP,UNITCODE ;  
   
  BEGIN FOR VT_INDEX IN NHAP_VATTUGD LOOP
    N_COUNT :=0;
      BEGIN 
        SELECT COUNT(*) INTO N_COUNT FROM '||P_TABLENAME||' WHERE 
        MAVATTU=VT_INDEX.MAVATTU AND  MAKHO=VT_INDEX.MAKHONHAP AND UNITCODE=VT_INDEX.UNITCODE;      
        EXCEPTION WHEN OTHERS THEN N_COUNT:=0;
      END;
    BEGIN
      IF(N_COUNT=0) THEN 
        BEGIN 
      INSERT INTO '||P_TABLENAME||' (UNITCODE,NAM,KY,MAKHO,MAVATTU) 
      SELECT VT_INDEX.UNITCODE AS UNITCODE,'|| P_NAM ||' AS NAM,'|| P_KY ||' AS KY,VT_INDEX.MAKHONHAP AS MAKHO,VT_INDEX.MAVATTU FROM DMVATTU WHERE UNITCODE=VT_INDEX.UNITCODE AND MAVATTU=VT_INDEX.MAVATTU;
      COMMIT;
        END;
       END IF;
     END;
     
    
    BEGIN     
      UPDATE '||P_TABLENAME||' SET
      NHAPSL=NHAPSL+VT_INDEX.SOLUONG, NHAPGT=NHAPGT+VT_INDEX.NHAPGT,TONCUOIKYSL=TONCUOIKYSL+VT_INDEX.SOLUONG,TONCUOIKYGT=TONCUOIKYGT+VT_INDEX.NHAPGT
      WHERE UNITCODE =VT_INDEX.UNITCODE AND  MAVATTU=VT_INDEX.MAVATTU AND MAKHO=VT_INDEX.MAKHONHAP; 
        COMMIT;
    END;
  
    END LOOP; 
    END;';
    
        DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);        
      EXECUTE IMMEDIATE N_SQL_INSERT;
        EXCEPTION WHEN OTHERS THEN DBMS_OUTPUT.PUT_LINE(N_SQL_INSERT);
END XNT_TANG_PHIEU;
END XNT;