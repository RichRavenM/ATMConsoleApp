using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.Domain.Enums
{
    public enum AppMenu
    {
        CheckBalance = 1,
        PlaceDeposit = 2,
        MakeWithdrawal = 3,
        InternalTransfer = 4,
        ViewTransaction = 5,
        Logout = 6
    }
}
