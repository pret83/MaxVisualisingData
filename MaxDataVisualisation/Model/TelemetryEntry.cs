using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDataVisualisation.Model
{
    public class TelemetryEntry
    {
        public TelemetryEntry(
            DateTimeOffset timeStamp,
            double latitude,
            double longitude,
            int gear,
            double throttlePos,
            double brakePos,
            double gLong,
            double gLat,
            double engineRpm,
            double vCar,
            double sLap)
        {
            TimeStamp = timeStamp;
            Latitude = latitude;
            Longitude = longitude;
            Gear = gear;
            ThrottlePosition = throttlePos;
            BrakePosition = brakePos;
            GLong = gLong;
            GLat = gLat;
            EngineRpm = engineRpm;
            LapDist = sLap;
            VCar = vCar;
        }

        public DateTimeOffset TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Gear { get; set; }
        public double ThrottlePosition { get; set; }
        public double BrakePosition { get; set; }
        public double GLong { get; set; }
        public double GLat { get; set; }
        public double EngineRpm { get; set; }
        public double LapDist { get; set; }
        public double VCar { get; set; }
    }
}
