create or replace package body BTSOFT_PCK_BAOCAOKETOAN is

  /*
      Aut: NhuanNH
      CD: 03/08/2016
      Des: Tong hop bao cao len so nhat ky chung
      Par: p_thang in varchar2, p_nam in varchar2
  */
  procedure thbc_sonhatkychung(p_thang in varchar2, p_nam in varchar2) is
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
                 and to_char(ngay, 'MMyyyy') = p_thang || p_nam
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
                                   p_thang    in varchar2,
                                   p_nam      in varchar2) is
  Begin
    insert into tab_tonghopbc_tmp tmp
      (ngayct, STT, SOCT, NOIDUNG, TAIKHOAN, NO, CO)
      select to_char(ngay, 'dd-MM-yyyy'), stt, soct, noidung, tk, ino, ico
        from (select stt,
                     soct,
                     noidung,
                     tkno || tkco as tk,
                     ngay,
                     sum(ino) ino,
                     sum(ico) ico
                from V_SONHATKYCHUNG t
               where 1 = 1
                 and to_char(ngay, 'MMyyyy') = p_thang || p_nam
                 and tkno || tkco in
                     (select matk
                        from dmtaikhoan dmtk
                       where dmtk.matkcha = p_taikhoan)
               group by ngay, stt, soct, tkno, tkco, noidung)
       order by ngay, stt, ico, ino;
  End;
end BTSOFT_PCK_BAOCAOKETOAN;
