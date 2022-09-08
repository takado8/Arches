using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Arches.service
{
    internal class Timer
    {
        private Stopwatch? watch;
        public void measureTimeStart()
        {
            watch = Stopwatch.StartNew();
        }

        public long measureTimeStop()
        {
            if (watch != null)
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                watch.Reset();
                return elapsedMs;
            }
            else
            {
                Console.WriteLine("Error: timer not initialized.");
                return -1;
            }
        }
    }
}
