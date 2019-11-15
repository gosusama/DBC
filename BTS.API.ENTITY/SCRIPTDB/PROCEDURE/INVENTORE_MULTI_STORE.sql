create or replace PROCEDURE  INVENTORE_MULTI_STORE (
    P_MAVATTU IN VARCHAR2, 
    P_UNITCODE IN VARCHAR2,
    P_TABLENAME IN VARCHAR2,
    CUR  OUT SYS_REFCURSOR) 
    IS
    QUERY_STR VARCHAR(32767);
    P_EXPRESSION  VARCHAR(32767):= '';
    QUERY_TMP VARCHAR(20):= '';
    CONDITION VARCHAR(2000);
    BEGIN
        DECLARE
            CURSOR UNIT IS SELECT UNITCODE FROM SYS_DONVI WHERE UNITCODE LIKE SUBSTR(''||P_UNITCODE||'',0,4) || '%' GROUP BY UNITCODE;
        BEGIN
            FOR u IN UNIT 
                 LOOP
                     SELECT '''' || u.UNITCODE || '''' || ' AS ' || REPLACE(''||u.UNITCODE||'','-','_')  INTO QUERY_TMP FROM DUAL;
                     P_EXPRESSION :=P_EXPRESSION || QUERY_TMP || ',';
                 END LOOP;
        END;   
            P_EXPRESSION := SUBSTR(P_EXPRESSION,0,LENGTH(P_EXPRESSION) - 1);
            
            DBMS_OUTPUT.PUT_LINE('P_EXPRESSION:' || P_EXPRESSION);
        BEGIN
        IF(P_MAVATTU IS NOT NULL) THEN CONDITION := ' AND 1=1 AND b.MAVATTU LIKE '''||P_MAVATTU||'%'' ';
        ELSE CONDITION := ' AND 1=1 ';
        END IF;
            QUERY_STR := 'SELECT * FROM (
                SELECT a.MA AS Ma, a.TEN AS Ten,TO_CHAR(a.TONCUOIKYSL) as TONCUOIKYSL,a.UNITCODE AS MADONVI
                FROM (
                SELECT  b.MAVATTU AS MA, b.TENVATTU AS TEN
                                          ,SUM(NVL(t.TONCUOIKYSL,0)) as TONCUOIKYSL
                                          ,t.UNITCODE 
                FROM '||P_TABLENAME||' t
                left join DMVATTU b on b.MAVATTU = t.MAVATTU  AND SUBSTR(b.UNITCODE,0,3) = SUBSTR(t.UNITCODE,0,3)
                WHERE
                    t.UNITCODE  LIKE SUBSTR('''||P_UNITCODE||''',0,3) || ''%'' '||CONDITION||'
                    group by  b.MAVATTU,b.TENVATTU, t.UNITCODE) a
                    )
            PIVOT(
                SUM(TONCUOIKYSL) 
                FOR MADONVI IN ('||P_EXPRESSION||')
            )';
        
        END;
        BEGIN
        DBMS_OUTPUT.PUT_LINE('QUERY_STR-->' || QUERY_STR);
            OPEN CUR FOR QUERY_STR;
            EXCEPTION
           WHEN NO_DATA_FOUND
           THEN
              DBMS_OUTPUT.put_line ('ERROR' || SQLERRM);
           WHEN OTHERS
           THEN
              DBMS_OUTPUT.put_line (QUERY_STR  || SQLERRM);
        END;
  END INVENTORE_MULTI_STORE;
  /