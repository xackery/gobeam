using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace gobeam
{


    class w32
    {
        //Get a list of open processes (optionally by a filter)
        public static Process[] GetProcessList(string filter)
        {
            if (filter == "")
            {
                return Process.GetProcesses();
            }
            return Process.GetProcessesByName(filter);
        }


        public const byte VK_LEFT = 0x25;
        public const byte VK_D = 0x44;
        public const byte VK_RETURN = 0x0D;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        //Attach to a process
        const int PROCESS_WM_READ = 0x0010;
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public static IntPtr AttachProcess(Process process)
        {
            return OpenProcess(PROCESS_WM_READ, true, process.Id);
        }

        //Read process memory
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static int ReadProcessMemory(IntPtr handle, int address)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            var didRead = ReadProcessMemory((int)handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static string ReadProcessMemoryByte(IntPtr handle, int address)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[1];

            var didRead = ReadProcessMemory((int)handle, address, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToString(buffer);
        }


        public static int ReadHighScore(IntPtr handle, pointerset ps, int finalAdjust)
        {

            //Start with the base address's value
            var addr = ps.GetBaseAddress();
            //   Console.WriteLine("b:", addr.ToString("X"));
            if (ps.Offsets == null) return 0;
            for (var i = 0; i < ps.Offsets.Length; i++)
            {

                var preOffset = ps.Offsets[i];

                if ((ps.Offsets.Length - 1) == i)
                {
                    preOffset += finalAdjust;

                    string highScore = "";
                    for (var x = 0; x < 6; x++)
                    {
                        string newScore = ReadProcessMemoryByte(handle, addr + preOffset + x);
                        highScore += newScore.Substring(1);
                    }
                    return Convert.ToInt32(highScore);
                }

                //Get each offset
                // Console.WriteLine((i + 1) + "NextAddr:" + (addr + preOffset).ToString("X"));
                var offset = ReadProcessMemory(handle, addr + preOffset);



                //  Console.WriteLine((i + 1) + "Offset:" + offset.ToString("X"));
                //Set the offset
                addr = offset;
                //Console.WriteLine((i + 1) + "Addr:" + addr.ToString("X"));
            }
            //  Console.WriteLine("Last addr:" + addr.ToString("X"));

            return 0;
        }


        public static int ReadBallByte(IntPtr handle, pointerset ps, int finalAdjust)
        {

            //Start with the base address's value
            var addr = ps.GetBaseAddress();
            //   Console.WriteLine("b:", addr.ToString("X"));
            if (ps.Offsets == null) return 0;
            for (var i = 0; i < ps.Offsets.Length; i++)
            {

                var preOffset = ps.Offsets[i];

                if ((ps.Offsets.Length - 1) == i)
                {
                    preOffset += finalAdjust;

                    string newScore = ReadProcessMemoryByte(handle, addr + preOffset);

                    return Convert.ToInt32(newScore);
                }

                //Get each offset
                // Console.WriteLine((i + 1) + "NextAddr:" + (addr + preOffset).ToString("X"));
                var offset = ReadProcessMemory(handle, addr + preOffset);



                //  Console.WriteLine((i + 1) + "Offset:" + offset.ToString("X"));
                //Set the offset
                addr = offset;
                //Console.WriteLine((i + 1) + "Addr:" + addr.ToString("X"));
            }
            //  Console.WriteLine("Last addr:" + addr.ToString("X"));

            return 0;
        }

        public static int ReadProcessMemoryOffset(IntPtr handle, pointerset ps, int finalAdjust)
        {
            //Start with the base address's value
            var addr = ps.GetBaseAddress(); // ReadProcessMemory(handle, ps.baseAddress);
            Console.WriteLine("b:", addr.ToString("X"));
            for (var i = 0; i < ps.Offsets.Length; i++)
            {

                var preOffset = ps.Offsets[i];

                if ((ps.Offsets.Length - 1) == i)
                {
                    preOffset += finalAdjust;
                }

                //Get each offset
                // Console.WriteLine((i + 1) + "NextAddr:" + (addr + preOffset).ToString("X"));
                var offset = ReadProcessMemory(handle, addr + preOffset);

                Console.WriteLine((i + 1) + "Offset:" + offset.ToString("X"));
                //Set the offset
                addr = offset;
                Console.WriteLine((i + 1) + "Addr:" + addr.ToString("X"));
            }
            Console.WriteLine("Last addr:" + addr.ToString("X"));
            return addr;
        }

        //Send Input
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            int dx;
            int dy;
            uint mouseData;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }

        [DllImport("User32.dll")]
        protected static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] input, int structSize);

        //Send a raw input array
        public static bool SendInput(INPUT[] inputs)
        {
            uint ret = SendInput((uint)inputs.Length, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            return (ret == 1);
        }

        //Send keyboard based input, use PressKey for a simpler handler.
        public static bool SendInput(KEYBDINPUT keyboardInput)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ki.dwFlags = keyboardInput.dwFlags;
            inputs[0].ki.wScan = keyboardInput.wScan;
            return SendInput(inputs);
        }

        //Press the given keycode for delayMilliseconds duration, then release.
        public static bool PressKey(UInt16 keyCode, int delayMilliseconds)
        {
            KEYBDINPUT keyboardInput = new KEYBDINPUT();
            keyboardInput.wScan = keyCode;
            keyboardInput.dwFlags = 0x0008; //Press hold key
            keyboardInput.time = 0;
            var keyState = SendInput(keyboardInput);
            if (!keyState) return false;
            //time.sleep(delayMilliseconds);
            keyboardInput.dwFlags = 0x0002 | 0x0008; //release hold key
            return SendInput(keyboardInput);
        }


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
  
    }
}
