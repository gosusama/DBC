using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY
{
    public enum TypeMasterData
    {
        VATTU,
        LOAIHANG,
        KH,
        NCC,
        MN,
        KEHANG,
        BOHANG,
        DV,
        CH,
        BAOBI,
        CANDIENTU,
        BH,
        DVT,
        MAPHIEUKIEMKE,
        SIZE,
        COLOR,
        KHO,
        XUATXU,
        NV
    }

    public enum StateMerchandise
    {
        ACTIVE = 10,
        DEACTIVE = 0,
        CANDIENTU = 30
    }
}
