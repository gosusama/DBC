CREATE OR REPLACE PACKAGE BTSOFT_PCK_BAOCAOKETOAN IS
  procedure thbc_sonhatkychung(p_tungay   in date,
                               p_denngay  in date,
                               p_unitcode in varchar2);
  procedure thbc_sochitiettaikhoan(p_taikhoan in varchar2,
                                   p_tungay   in date,
                                   p_denngay  in date,
                                   p_unitcode in varchar2);
End BTSOFT_PCK_BAOCAOKETOAN;
/
create or replace package body BTSOFT_PCK_BAOCAOKETOAN is

  /*
      Aut: NhuanNH
      CD: 03/08/2016
      Des: Tong hop bao cao len so nhat ky chung
      Par: p_tungay  in date, p_denngay in date, p_boso in varchar2
  */
  procedure thbc_sonhatkychung(p_tungay   in date,
                               p_denngay  in date,
                               p_unitcode in varchar2) is
  Begin
    insert into tab_tonghopbc_tmp tmp
      (ngayct, STT, SOCT, NOIDUNG, TAIKHOAN, NO, CO)
      select to_char(ngay, 'dd-MM-yyyy'), stt, soct, noidung, tk, ino, ico
        from (select stt,
                     soct,
                     tkno || tkco as tk,
                     noidung,
                     ngay,
                     sum(ino) ino,
                     sum(ico) ico
                from V_SONHATKYCHUNG t
               where 1 = 1
                 and t.unitcode = p_unitcode
                 and (ngay between p_tungay and p_denngay)
               group by ngay, stt, soct, tkno, tkco, noidung)
       order by ngay, stt, ico, ino;
  End;

  /* 
      Aut: NhuanNH
      CD: 08/08/2016
      Des: Tong hop bao cao len so chi tiet tai khoan
      Par: p_taikhoan in varchar2, p_thang in varchar2, p_nam in varchar2
  */
  procedure thbc_sochitiettaikhoan(p_taikhoan in varchar2,
                                   p_tungay   in date,
                                   p_denngay  in date,
                                   p_unitcode in varchar2) is
  Begin
    insert into tab_tonghopbc_tmp tmp
      (ngayct, SOCT , NOIDUNG, TAIKHOAN, TAIKHOANDOI,NO, CO)
      select to_char(ngaychungtu, 'dd-MM-yyyy'), soct, noidung, taikhoan, taikhoandoi, no, co
        from (
          select T.ngaychungtu, CT.machungtu soct, CT.lydo as noidung, T.tkno as taikhoan, T.tkco as taikhoandoi,  T.sotien as no, 0 as co
        from SOTONGHOP T, NVCHUNGTU CT
    where (CT.ngay BETWEEN  p_tungay and p_denngay) and  CT.Unitcode = p_unitcode and T.machungtupk = CT.machungtupk and T.Trangthai = 10 and T.tkno in (select matk from dmtaikhoan where matk = p_taikhoan or matkcha = p_taikhoan)
    UNION ALL
          select T.ngaychungtu, CT.machungtu , CT.lydo, T.tkco as taikhoan, T.tkno as taikhoandoi, 0 as no, T.sotien as co
        from SOTONGHOP T, NVCHUNGTU CT
    where (CT.ngay BETWEEN  p_tungay and p_denngay) and CT.Unitcode = p_unitcode and T.machungtupk = CT.machungtupk and T.Trangthai = 10 and T.tkco in (select matk from dmtaikhoan where matk = p_taikhoan or matkcha = p_taikhoan)
    UNION ALL
          select T.ngaychungtu, UN.souynhiem , UN.noidung as noidung, T.tkno as taikhoan ,T.tkco as taikhoandoi,  T.sotien as no, 0 as co
        from SOTONGHOP T, NVUYNHIEMCHI UN
    where (UN.ngay BETWEEN  p_tungay and p_denngay) and UN.Unitcode = p_unitcode and T.machungtupk = UN.machungtupk and T.Trangthai = 10 and T.tkco in (select matk from dmtaikhoan where matk = p_taikhoan or matkcha = p_taikhoan)
        UNION ALL
          select T.ngaychungtu, UN.souynhiem , UN.noidung as noidung, T.tkco as taikhoan, T.tkno as taikhoandoi,  0 as no, T.sotien as co
        from SOTONGHOP T, NVUYNHIEMCHI UN
    where (UN.ngay BETWEEN  p_tungay and p_denngay) and UN.Unitcode = p_unitcode and T.machungtupk = UN.machungtupk and T.Trangthai = 10 and T.tkno in (select matk from dmtaikhoan where matk = p_taikhoan or matkcha = p_taikhoan)
    )
       order by ngaychungtu;
  End;
end BTSOFT_PCK_BAOCAOKETOAN;
/
