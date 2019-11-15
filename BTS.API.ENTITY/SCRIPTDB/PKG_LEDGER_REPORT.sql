CREATE OR REPLACE PACKAGE "PKG_LEDGER_REPORT" IS
  --Author: QuyVD
  --Email: quyvd@funix.edu.vn
  --Tel: 0983411183
  --Create date: 25-Sep-2016
  ---------------------------------------------
  /*
   Nhat ky chung
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_NKC('01-Jan-2016', '01-Nov-2016', 'DV0001') FROM DUAL;
  */
  FUNCTION FN_LEDGER_NKC(PFROMDATE DATE, PTODATE DATE, PDONVI VARCHAR) RETURN SYS_REFCURSOR;
  /*
   LAY SO TON CUA TAI KHOAN TAI NGAY BAT KY
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_TON('01-Jan-2016', 'DV0001', 111,'CUOI') FROM DUAL;
  */
  FUNCTION FN_LEDGER_THEKT(PFROMDATE DATE, PTODATE DATE, PDONVI VARCHAR, PTAIKHOAN VARCHAR) RETURN SYS_REFCURSOR;
  /*
   THE CHI TIET TAI KHOAN
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_THEKT('01-Jan-2016', '01-Nov-2016', 'DV0001', 111) FROM DUAL;
  */
  FUNCTION FN_LEDGER_TON(PDATE DATE, PDONVI VARCHAR, PTAIKHOAN VARCHAR, PTYPE VARCHAR) RETURN NUMBER;
END PKG_LEDGER_REPORT;
/
CREATE OR REPLACE PACKAGE BODY "PKG_LEDGER_REPORT" IS
  --Author: QuyVD
  --Email: quyvd@funix.edu.vn
  --Tel: 0983411183
  --Create date: 25-Sep-2016
  /*
   Nhat ky chung
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_NKC('01-Jan-2016', '01-Nov-2016', 'DV0001') FROM DUAL;
  */
  FUNCTION FN_LEDGER_NKC(PFROMDATE DATE, PTODATE DATE, PDONVI VARCHAR) RETURN SYS_REFCURSOR IS
    TYPE T_CURSOR IS REF CURSOR;
    V_CURSOR T_CURSOR;
    V_DONVI  VARCHAR(20) := PDONVI;
    D_FROMDT DATE := PFROMDATE;
    D_TODT   DATE := PTODATE;
    V_TENDONVI  VARCHAR2(100);
    V_DIACHI    VARCHAR2(200);
    V_DIENTHOAI VARCHAR2(20);
  BEGIN
    SELECT DV.DESCRIPTION, DV.PHONE_NO, DV.ADDRESS INTO V_TENDONVI, V_DIACHI, V_DIENTHOAI FROM DMDONVI DV WHERE DV.CUST_UNIT_CODE = V_DONVI;
  
    OPEN V_CURSOR FOR
      SELECT V_DONVI,
             HT.MACHUNGTU,
             HT.NGAY_CT,
             HT.NOIDUNG,
             HT.TK,
             HT.DRCR,
             HT.NO,
             HT.CO,
             V_TENDONVI,
             V_DIENTHOAI,
             V_DIACHI
        FROM (SELECT A.MACHUNGTU,
                     A.LOAICHUNGTU,
                     TRUNC(A.NGAYCHUNGTU) NGAY_CT,
                     A.NOIDUNG,
                     A.TKNO AS TK,
                     'NO' AS DRCR,
                     SUM(A.SOTIEN) AS NO,
                     0 AS CO
                FROM SOTONGHOP A
               WHERE A.UNITCODE = V_DONVI
                 AND A.NGAYCHUNGTU >= D_FROMDT
                 AND A.NGAYCHUNGTU <= D_TODT
               GROUP BY MACHUNGTU, A.LOAICHUNGTU, A.NGAYCHUNGTU, A.NOIDUNG, A.TKNO
              UNION ALL
              SELECT A.MACHUNGTU,
                     A.LOAICHUNGTU,
                     TRUNC(A.NGAYCHUNGTU) NGAY_CT,
                     A.NOIDUNG,
                     A.TKCO AS TK,
                     'CO' AS DRCR,
                     0 AS NO,
                     SUM(A.SOTIEN) AS CO
                FROM SOTONGHOP A
               WHERE A.UNITCODE = V_DONVI
                 AND A.NGAYCHUNGTU >= D_FROMDT
                 AND A.NGAYCHUNGTU <= D_TODT
               GROUP BY A.MACHUNGTU, A.LOAICHUNGTU, A.NGAYCHUNGTU, A.NOIDUNG, A.TKCO) HT
       ORDER BY MACHUNGTU, DRCR;
    RETURN V_CURSOR;
  END FN_LEDGER_NKC;

  ----------------------------------------------
  --Author: QuyVD
  --Email: quyvd@funix.edu.vn
  --Tel: 0983411183
  --Create date: 09-Oct-2016
  /*
   LAY SO TON CUA TAI KHOAN TAI NGAY BAT KY
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_TON('01-Jan-2016', 'DV0001', 111,'CUOI') FROM DUAL;
  */
  FUNCTION FN_LEDGER_TON(PDATE DATE, PDONVI VARCHAR, PTAIKHOAN VARCHAR, PTYPE VARCHAR) RETURN NUMBER IS
    V_LOAITK   VARCHAR(20);
    V_DONVI    VARCHAR2(100) := PDONVI;
    V_TAIKHOAN VARCHAR(20) := PTAIKHOAN;
    V_TYPE     VARCHAR(4) := PTYPE;
    D_TON      DATE := PDATE;
    D_TONKSDT  DATE;
    N_TON      NUMBER(20);
    N_DUCO     NUMBER(20);
    N_DUNO     NUMBER(20);
  BEGIN
    --Xac dinh loai tai khoan
    SELECT TK.MALOAITK
      INTO V_LOAITK
      FROM DMTAIKHOAN TK
     WHERE TK.UNITCODE = V_DONVI
       AND TK.MATK = V_TAIKHOAN;
    --Xac dinh ngay khoa so gan nhat
    SELECT MAX(KS.NGAYKHOASO)
      INTO D_TONKSDT
      FROM DCL_KHOASO KS
     WHERE KS.NGAYKHOASO <= D_TON
       AND KS.UNITCODE = V_DONVI;
    --Neu la ton cuoi thi lay ca ps cua ngay cuoi, ton dau ngay la ton cua ngay hom truoc
    IF V_TYPE = 'CUOI' THEN
      D_TON := D_TON + 1;
    END IF;
    --Xac dinh so ton
    --Xac dinh so PS tu ngay khoa so D_TONKSDT den ngay D_TON
    SELECT SUM(SODUCO), SUM(SODUNO)
      INTO N_DUCO, N_DUNO
      FROM (SELECT 0 SODUCO, NVL(T.SOTIEN, 0) SODUNO
              FROM DMTAIKHOAN D
              LEFT JOIN SOTONGHOP T
                ON D.MATK = T.TKNO
               AND D.UNITCODE = T.UNITCODE
               AND T.NGAYCHUNGTU > D_TONKSDT
               AND T.NGAYCHUNGTU < D_TON
               AND D.UNITCODE = V_DONVI
               AND D.MATK = V_TAIKHOAN
            UNION ALL
            SELECT NVL(T.SOTIEN, 0) SODUCO, 0 SODUNO
              FROM DMTAIKHOAN D
              LEFT JOIN SOTONGHOP T
                ON D.MATK = T.TKCO
               AND D.UNITCODE = T.UNITCODE
               AND T.NGAYCHUNGTU > D_TONKSDT
               AND T.NGAYCHUNGTU < D_TON
               AND D.UNITCODE = V_DONVI
               AND D.MATK = V_TAIKHOAN);
    -- Xac dinh so du tai ngay D_TON
    SELECT CASE V_LOAITK
             WHEN '34' THEN
              DCL.SODU_NO_CUOIKY + N_DUNO - N_DUCO
             WHEN '12' THEN
              DCL.SODU_CO_CUOIKY + N_DUCO - N_DUNO
             ELSE
              0
           END
      INTO N_TON
      FROM DCL_SODUCUOIKY DCL
     INNER JOIN DCL_KHOASO KS
        ON DCL.MAKHOASO = KS.MAKHOASO
       AND DCL.TAIKHOAN = V_TAIKHOAN
       AND KS.UNITCODE = V_DONVI
       AND KS.NGAYKHOASO = D_TONKSDT;
    RETURN N_TON;
  END FN_LEDGER_TON;

  ---------------------------------------------------  
  --Author: QuyVD
  --Email: quyvd@funix.edu.vn
  --Tel: 0983411183
  --Create date: 09-Oct-2016
  /*
   THE CHI TIET TAI KHOAN
   Cach goi ham: SELECT PKG_LEDGER_REPORT.FN_LEDGER_THEKT('01-Jan-2016', '01-Nov-2016', 'DV0001', 111) FROM DUAL;
  */
  FUNCTION FN_LEDGER_THEKT(PFROMDATE DATE, PTODATE DATE, PDONVI VARCHAR, PTAIKHOAN VARCHAR) RETURN SYS_REFCURSOR IS
    TYPE T_CURSOR IS REF CURSOR;
    V_CURSOR    T_CURSOR;
    V_DONVI     VARCHAR(20) := PDONVI;
    D_FROMDT    DATE := PFROMDATE;
    D_TODT      DATE := PTODATE;
    V_TAIKHOAN  VARCHAR(20) := PTAIKHOAN;
    V_TENTK     VARCHAR2(100);
    V_LOAITK    VARCHAR(20);
    V_TENDONVI  VARCHAR2(100);
    V_DIACHI    VARCHAR2(200);
    V_DIENTHOAI VARCHAR2(20);
    N_DUCO_FROM NUMBER(20);
    N_DUNO_FROM NUMBER(20);
    N_DUCO_TO   NUMBER(20);
    N_DUNO_TO   NUMBER(20);
    N_TONDAU    NUMBER(20);
    N_TONCUOI   NUMBER(20);
  BEGIN
    --Chuan bi tham so
    SELECT DV.DESCRIPTION, DV.PHONE_NO, DV.ADDRESS
      INTO V_TENDONVI, V_DIACHI, V_DIENTHOAI
      FROM DMDONVI DV
     WHERE DV.CUST_UNIT_CODE = V_DONVI;
    SELECT TK.MALOAITK, TK.TENTK
      INTO V_LOAITK, V_TENTK
      FROM DMTAIKHOAN TK
     WHERE TK.UNITCODE = V_DONVI
       AND TK.MATK = V_TAIKHOAN;
    --Xac dinh so du dau
    IF V_LOAITK = '34' THEN
      N_DUNO_FROM := FN_LEDGER_TON(D_FROMDT, V_DONVI, V_TAIKHOAN, 'DAU');
      N_DUCO_FROM := 0;
      N_TONDAU    := N_DUNO_FROM;
    ELSIF V_LOAITK = '12' THEN
      N_DUNO_FROM := 0;
      N_DUCO_FROM := FN_LEDGER_TON(D_FROMDT, V_DONVI, V_TAIKHOAN, 'DAU');
      N_TONDAU    := N_DUCO_FROM;
    ELSE
      N_DUNO_FROM := 0;
      N_DUCO_FROM := 0;
    END IF;
    --Xac dinh so du cuoi
    IF V_LOAITK = '34' THEN
      N_DUNO_TO := FN_LEDGER_TON(D_TODT, V_DONVI, V_TAIKHOAN, 'CUOI');
      N_DUCO_TO := 0;
      N_TONCUOI := N_DUNO_TO;
    ELSIF V_LOAITK = '12' THEN
      N_DUNO_TO := 0;
      N_DUCO_TO := FN_LEDGER_TON(D_TODT, V_DONVI, V_TAIKHOAN, 'CUOI');
      N_TONCUOI := N_DUCO_TO;
    ELSE
      N_DUNO_TO := 0;
      N_DUCO_TO := 0;
    END IF;
    --Goi Cursor
    OPEN V_CURSOR FOR
      SELECT V_DONVI AS DON_VI,
             A.MACHUNGTU,
             A.LOAICHUNGTU,
             TRUNC(A.NGAYCHUNGTU) AS NGAY_GHISO,
             TRUNC(A.NGAYCHUNGTU) AS NGAY_CT,
             N_DUNO_FROM AS DUNO_DAUKY,
             N_DUCO_FROM AS DUCO_DAUKY,
             N_TONDAU AS TON_DAUKY,
             N_DUNO_TO AS DUNO_CUOIKY,
             N_DUCO_TO AS DUCO_CUOIKY,
             N_TONCUOI AS TON_CUOIKY,
             V_TAIKHOAN AS TK,
             A.NOIDUNG,
             A.TKCO AS TK_DOIUNG,
             'NO' AS DRCR,
             SUM(A.SOTIEN) AS NO,
             0 AS CO
        FROM SOTONGHOP A
       WHERE A.TKNO = V_TAIKHOAN
         AND A.UNITCODE = V_DONVI
       GROUP BY V_DONVI,
                N_DUNO_FROM,
                N_DUCO_FROM,
                N_TONDAU,
                N_DUNO_TO,
                N_DUCO_TO,
                N_TONCUOI,
                MACHUNGTU,
                A.LOAICHUNGTU,
                A.NGAYCHUNGTU,
                A.NOIDUNG,
                V_TAIKHOAN,
                A.TKCO
      UNION ALL
      SELECT V_DONVI,
             A.MACHUNGTU,
             A.LOAICHUNGTU,
             TRUNC(A.NGAYCHUNGTU) NGAY_GHISO,
             TRUNC(A.NGAYCHUNGTU) NGAY_CT,
             N_DUNO_FROM,
             N_DUCO_FROM,
             N_TONDAU,
             N_DUNO_TO,
             N_DUCO_TO,
             N_TONCUOI,
             V_TAIKHOAN AS TK,
             A.NOIDUNG,
             A.TKNO AS TK_DOIUNG,
             'CO' AS DRCR,
             0 AS NO,
             SUM(A.SOTIEN) AS CO
        FROM SOTONGHOP A
       GROUP BY V_DONVI,
                N_DUNO_FROM,
                N_DUCO_FROM,
                N_TONDAU,
                N_DUNO_TO,
                N_DUCO_TO,
                N_TONCUOI,
                A.MACHUNGTU,
                A.LOAICHUNGTU,
                A.NGAYCHUNGTU,
                A.NOIDUNG,
                V_TAIKHOAN,
                A.TKNO
       ORDER BY NGAY_CT, MACHUNGTU, TK_DOIUNG, DRCR;
    RETURN V_CURSOR;
  END FN_LEDGER_THEKT;

END PKG_LEDGER_REPORT;
/
