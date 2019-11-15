﻿create or replace PROCEDURE REPORT_CUSTOMER_BY_BIRTHDAY(P_FROMDATE IN DATE,P_TODATE IN DATE, UNITCODE NVARCHAR2,P_WAREHOUSE NVARCHAR2, FROM_MONEY NUMBER, TO_MONEY NUMBER, P_CUSTOMERCODES NVARCHAR2 ,P_CUR OUT SYS_REFCURSOR) 
AS
QUERY_STR VARCHAR(32767);
B_FROMDATE DATE;
E_FROMDATE DATE;
BEGIN
    IF(P_FROMDATE IS NULL) THEN 
        B_FROMDATE := SYSDATE;      
    ELSIF (P_TODATE IS NULL) THEN
        E_FROMDATE := SYSDATE;
    END IF;
    QUERY_STR := 'SELECT B.NGAYSINH AS NGAYSINHNHAT,B.MAKH AS MAKHACHHANG,B.MATHE AS MATHEVIP,
        B.TENKH AS TENKHACHHANG,B.DIENTHOAI AS SODIENTHOAI,
        A.MADONVI AS MADONVI,SUM(NVL(A.TTIENCOVAT,0)) AS SOTIEN 
        FROM NVGDQUAY_ASYNCCLIENT A RIGHT JOIN DMKHACHHANG B 
        ON A.MAKHACHHANG = B.MAKH 
        WHERE TO_DATE(A.NGAYPHATSINH,''DD-MM-YY'') = TO_DATE(B.NGAYSINH,''DD-MM-YY'') 
        AND TO_DATE(A.NGAYPHATSINH, ''DD-MM-YY'') BETWEEN TO_DATE('''||P_FROMDATE||''', ''DD/MM/YY'') AND TO_DATE('''||P_TODATE||''', ''DD/MM/YY'') AND B.UNITCODE LIKE ''%'||UNITCODE||'%''';
        
        

        IF(FROM_MONEY <> 0) THEN  QUERY_STR := QUERY_STR || ' AND A.TTIENCOVAT >= '||FROM_MONEY||''; END IF;
        IF(TO_MONEY <> 0) THEN  QUERY_STR := QUERY_STR || ' AND A.TTIENCOVAT <= '||TO_MONEY||''; END IF;
        
        
        IF (P_CUSTOMERCODES IS NOT NULL) THEN
            QUERY_STR := QUERY_STR || ' AND B.MAKH IN ('||P_CUSTOMERCODES||')';
        END IF;
        
        QUERY_STR:= QUERY_STR || ' GROUP BY B.NGAYSINH,B.MAKH,B.MATHE,B.TENKH,B.DIENTHOAI,A.MADONVI';
        
        
        BEGIN
            OPEN P_CUR FOR QUERY_STR;
            DBMS_OUTPUT.PUT_LINE(QUERY_STR);
            EXCEPTION
               WHEN NO_DATA_FOUND
               THEN
                  DBMS_OUTPUT.put_line ('<your message>' || SQLERRM);
               WHEN OTHERS
               THEN
                  DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);
        END;
END REPORT_CUSTOMER_BY_BIRTHDAY;