using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDataVisualisation.Model
{
    public class Lap
    {
        public TimeSpan LapTime
        {
            get
            {
                DateTimeOffset mintime = Telemetry.Select(t => t.TimeStamp).Min();
                DateTimeOffset maxtime = Telemetry.Select(t => t.TimeStamp).Max();
                return maxtime.Subtract(mintime);
            }
        }

        public string LaptimeString
        {
            get
            {
                return LapTime.Minutes.ToString() + ":" + LapTime.Seconds.ToString() + "." + LapTime.Milliseconds.ToString("#000");
            }
        }


        public int LapNumber { get; set; }

        public List<TelemetryEntry> Telemetry { get; } = new List<TelemetryEntry>();

    }
}
