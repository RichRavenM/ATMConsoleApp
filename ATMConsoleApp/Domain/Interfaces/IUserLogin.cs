using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.Domain.Interfaces
{
    public interface IUserLogin
    {
        void CheckUserCardNumberAndPassword();
    }
}
