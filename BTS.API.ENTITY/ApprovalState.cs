using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY
{
    public enum ApprovalState
    {
        IsNotApproval = 0,
        IsApproval = 20,
        IsComplete = 10,
        IsExpired = 30,
        IsUnClosingOut = 40
    }
}
