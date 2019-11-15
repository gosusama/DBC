using BTS.API.ENTITY;
using System;
using System.Collections.Generic;
namespace BTS.API.SERVICE.Authorize.AuNguoiDungQuyen
{
    public class AuNguoiDungQuyenVm
    {
        public class ViewModel:DataInfoEntity
        {
            public string USERNAME { get; set; }
            public string MACHUCNANG { get; set; }
            public string TENCHUCNANG { get; set; }
            public string SOTHUTU { get; set; }
            public bool XEM { get; set; }
            public bool THEM { get; set; }
            public bool SUA { get; set; }
            public bool XOA { get; set; }
            public bool DUYET { get; set; }
            public bool GIAMUA { get; set; }
            public bool GIABAN { get; set; }
            public bool GIAVON { get; set; }
            public bool TYLELAI { get; set; }
            public bool BANCHIETKHAU { get; set; }
            public bool BANBUON { get; set; }
            public bool BANTRALAI { get; set; }
            public string MACHUCNANGCHA { get; set; }

            public ViewModel()
            {
                Id = Guid.NewGuid().ToString();
                XEM = false;
                THEM = false;
                SUA = false;
                XOA = false;
                DUYET = false;
                GIAMUA = false;
                GIABAN = false;
                GIAVON = false;
                TYLELAI = false;
                BANCHIETKHAU = false;
                BANBUON = false;
                BANTRALAI = false;
            }
        }
        public class ConfigModel
        {
            public string USERNAME { get; set; }
            public List<ViewModel> LstAdd { get; set; }
            public List<ViewModel> LstEdit { get; set; }
            public List<ViewModel> LstDelete { get; set; }
        }
    }

}
