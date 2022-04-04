using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mochj.IDE.Helpers
{
    static class KeyboardHelper
    {
        public static bool IsAlNum(Key key)
        {
            int keyValue = (int)key;
            if ((keyValue >= 0x30 && keyValue <= 0x39) // numbers
             || (keyValue >= 0x41 && keyValue <= 0x5A) // letters
             || (keyValue >= 0x60 && keyValue <= 0x69)) // numpad
            {
                return true;
            }
            return false;
        }
    }
}
