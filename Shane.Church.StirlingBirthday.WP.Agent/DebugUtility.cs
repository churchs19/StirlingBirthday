using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.WP.Agent
{
    public static class DebugUtility
    {
        private const long MaximumMilliseconds = 25000L;
        private static readonly Stopwatch _stopwatch = new Stopwatch();

        public static void DebugOutputMemoryUsage(string label = null)
        {
#if DEBUG
            var limit = DeviceStatus.ApplicationMemoryUsageLimit;
            var current = DeviceStatus.ApplicationCurrentMemoryUsage;
            var remaining = limit - current;
            var peak = DeviceStatus.ApplicationPeakMemoryUsage;
            var safetyMargin = limit - peak;

            if (label != null)
            {
                Debug.WriteLine(label);
            }
            Debug.WriteLine(string.Format("Memory limit (bytes): " + limit));
            Debug.WriteLine(string.Format("Current memory usage: {0} bytes ({1} bytes remaining)", current, remaining));
            Debug.WriteLine(string.Format("Peak memory usage: {0} bytes ({1} bytes safety margin)", peak, safetyMargin));
#endif
        }

        public static void DebugStartStopwatch()
        {
#if DEBUG
            _stopwatch.Start();
#endif
        }
        public static void DebugOutputElapsedTime(string label = null)
        {
#if DEBUG
            var milliSeconds = _stopwatch.ElapsedMilliseconds;
            var remaining = MaximumMilliseconds - milliSeconds;

            if (label != null)
            {
                Debug.WriteLine(label);
            }
            Debug.WriteLine(string.Format("Running time: {0} milliseconds", milliSeconds));
            Debug.WriteLine(string.Format("Remaining time (max): {0} milliseconds", remaining));
#endif
        }
    }
}
