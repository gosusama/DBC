--------------------------------------------------------
--  File created - Tuesday-August-09-2016   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package BTSOFT_PCK_BAOCAOKETOAN
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "TBNETERP"."BTSOFT_PCK_BAOCAOKETOAN" IS
  procedure thbc_sonhatkychung(p_thang in varchar2, p_nam in varchar2);
  procedure thbc_sochitiettaikhoan(p_taikhoan in varchar2,
                                   p_thang    in varchar2,
                                   p_nam      in varchar2);
End BTSOFT_PCK_BAOCAOKETOAN;

/
