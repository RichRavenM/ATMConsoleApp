using ATMConsoleApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.App
{
    class Entry
    {
        static void Main(string[] args)
        {
            AppScreen.Welcome();
            ATMApp atmApp = new ATMApp();
            atmApp.InitialiseData();
            atmApp.CheckUserCardNumberAndPassword();
            atmApp.Welcome();
            Utility.PressEnterToContinue();
        }
    }
}
