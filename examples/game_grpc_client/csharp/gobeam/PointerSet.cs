using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gobeam
{
    class pointerset
    {
            public string ModuleName;

            //Base address is derived from moduleName+baseOffset
            private int BaseAddress;
            public int[] Offsets;

            public void SetBaseAddress(int address)
            {
                BaseAddress = address;
            }

            public int GetBaseAddress()
            {
                return BaseAddress;
            }
    }
}
