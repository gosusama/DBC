using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY
{
    public enum TypePromotion
    {
        ByItemGetItem = 1, //Mua hàng tặng hang
        MerchandiseType = 2, //Chiết khấu theo hàng
        MerchandiseGroup = 3, //Tích điểm cho khách
        Sponsor = 4
    }
    public enum LoaiKhuyenMai
    {
        ChietKhau = 1,
        DongGia =2,
        Buy1Get1 = 3,
        Combo = 4,
        TinhTien =5,
        TichDiem =6,
        Voucher = 7,
    }
}
