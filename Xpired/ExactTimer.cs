using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xpired.Win32;

namespace Xpired
{
    public class ExactTimer
    {
        static long _f = 0;
        static long InitTick = -1;
        static long Tick = 0;

        public static int GetDotNetTick()
        {
            if (InitTick < 0)
            {
                InitTick = DateTime.Now.Ticks;
                return 0;
            }
            else
            {
                Tick = DateTime.Now.Ticks;
                int rev = (int)(Tick / 10000 - InitTick / 10000);
                return rev;
            }
        }

        public static int GetTick()
        {
            if(InitTick<0)
            {
                InitTick = GetTickCount();
                return 0;
            }
            else
            {
                Tick = GetTickCount();
                int rev = (int)(Tick - InitTick);
                return rev;
            }
        }

        static public long GetTickCount()
        {
            long f = _f;

            if (f == 0)
            {
                if (NativeMethods.QueryPerformanceFrequency(ref f))
                {
                    _f = f;
                }
                else
                {
                    _f = -1;
                }
            }
            if (f == -1)
            {
                return Environment.TickCount * 10000;
            }
            long c = 0;
            NativeMethods.QueryPerformanceCounter(ref c);
            return (long)(((double)c) * 1000 * 10000 / ((double)f));
        }
    }
}
