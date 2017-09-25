using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Xpired.Win32
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter([In, Out] ref long lpPerformanceCount);
        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency([In, Out] ref long lpFrequency);
    }
}
