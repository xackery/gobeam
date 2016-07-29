using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace gobeam
{
    class WinLibrary
    {
        //KeyName: HKEY_CURRENT_USER\\RegistrySetValueExample
        public static object GetRegistryValue(string keyName, string valueName, object defaultValue)
        {
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        //KeyName: HKEY_CURRENT_USER\\RegistrySetValueExample
        public static void SetRegistryValue(string keyName, string valueName, object value)
        {
            Registry.SetValue(keyName, valueName, value);
        }
    }
}
