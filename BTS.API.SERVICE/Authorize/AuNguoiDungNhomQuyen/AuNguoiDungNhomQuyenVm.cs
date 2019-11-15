using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using System.Collections.Generic;

namespace BTS.API.SERVICE.Authorize.AuNguoiDungNhomQuyen
{
    public class AuNguoiDungNhomQuyenVm
    {
        public class ViewModel:DataInfoEntity
        {
            public string USERNAME { get; set; }
            public string MANHOMQUYEN { get; set; }
            public string TENNHOMQUYEN { get; set; }
        }
        public class ConfigModel
        {
            public string USERNAME { get; set; }
            public List<AU_NGUOIDUNG_NHOMQUYEN> LstAdd { get; set; }
            public List<AU_NGUOIDUNG_NHOMQUYEN> LstDelete { get; set; }
        }
    }
}
