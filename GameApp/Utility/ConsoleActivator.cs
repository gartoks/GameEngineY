using System;
using System.Runtime.InteropServices;

namespace GameApp.Utility {
    internal static class ConsoleActivator {
        //[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //static extern IntPtr GetStdHandle(int nStdHandle);

        //[DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //static extern int AllocConsole();

        //private const int STD_OUTPUT_HANDLE = -11;
        //private const int MY_CODE_PAGE = 437;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        internal static bool ShowConsole {
            set => ShowWindow(GetConsoleWindow(), value ? SW_SHOW : SW_HIDE);
        }
    }
}