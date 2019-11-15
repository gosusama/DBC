using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY
{
    public enum OrderState
    {
        IsNotApproval = 0,
        IsRecieved = 30,
        IsApproval = 20,
        IsComplete = 10,
    }
    public enum TrangThaiThanhToan
    {
        ChuaThanhToan = 0,
        DaThanhToan = 10,
    }
}
