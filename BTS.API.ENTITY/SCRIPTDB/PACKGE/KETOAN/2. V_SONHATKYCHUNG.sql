create or replace view v_sonhatkychung as
select nv.unitcode as unitcode,
       so.machungtupk || '*' as stt,
       to_char(so.machungtupk) as soctpk,
	   to_char(nv.machungtu) as soct,
       '' as tkno,
       '' as tkco,
       nv.lydo as noidung,
       null as ino,
       null as ico,
       nv.ngay as ngay
  from SOTONGHOP so,nvchungtu nv
where so.TRANGTHAI = 10
and so.machungtupk = nv.machungtupk
union
   select 
   nv.unitcode as unitcode,
   so.machungtupk || '*' || so.tkno as stt,
       '' as soctpk,
	   '' as soct,
       to_char(so.tkno) as tkno,
       '' as tkco,
       tk.tentk as noidung,
       so.sotien as ino,
       null as ico,
       nv.ngay as ngay
  from SOTONGHOP so, dmtaikhoan tk, nvchungtu nv
 where so.TRANGTHAI = 10
 and so.tkno = tk.matk
 and so.machungtupk = nv.machungtupk
-- Co
union
   select 
    nv.unitcode as unitcode,
   so.machungtupk || '*' || so.tkco as stt,
       '' as soct,
	   '' as soct,
       '' as tkno,
       to_char(so.tkco) as tkco,
       tk.tentk as noidung,
       null as ino,
       so.sotien as ico,
       nv.ngay as ngay
  from SOTONGHOP so, dmtaikhoan tk, nvchungtu nv
 where so.TRANGTHAI = 10
 and so.tkco = tk.matk
 and so.machungtupk = nv.machungtupk
 --uynhiemchi
 union
 select un.unitcode as unitcode,
       so.machungtupk || '*' as stt,
       to_char(so.machungtupk) as soctpk,
	   to_char(un.souynhiem) as soct,
       '' as tkno,
       '' as tkco,
       un.noidung as noidung,
       null as ino,
       null as ico,
       un.ngay as ngay
  from SOTONGHOP so,nvuynhiemchi un
where so.TRANGTHAI = 10
and so.machungtupk = un.machungtupk
union
   select 
   un.unitcode as unitcode,
   so.machungtupk || '*' || so.tkno as stt,
       '' as soctpk,
	   '' as soct,
       to_char(so.tkno) as tkno,
       '' as tkco,
       tk.tentk as noidung,
       so.sotien as ino,
       null as ico,
       un.ngay as ngay
  from SOTONGHOP so, dmtaikhoan tk, nvuynhiemchi un
 where so.TRANGTHAI = 10
 and so.tkno = tk.matk
 and so.machungtupk = un.machungtupk
-- Co
union
   select 
    un.unitcode as unitcode,
   so.machungtupk || '*' || so.tkco as stt,
       '' as soct,
	   '' as soct,
       '' as tkno,
       to_char(so.tkco) as tkco,
       tk.tentk as noidung,
       null as ino,
       so.sotien as ico,
       un.ngay as ngay
  from SOTONGHOP so, dmtaikhoan tk, nvuynhiemchi un
 where so.TRANGTHAI = 10
 and so.tkco = tk.matk
 and so.machungtupk = un.machungtupk
;
