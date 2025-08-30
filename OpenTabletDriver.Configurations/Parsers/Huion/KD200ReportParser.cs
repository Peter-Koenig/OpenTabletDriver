using OpenTabletDriver.Configurations.Parsers.UCLogic;
using OpenTabletDriver.Tablet;
using System;
using System.Numerics;

namespace OpenTabletDriver.Configurations.Parsers.Huion
{
    public class KD200ReportParser : IReportParser<IDeviceReport>
    {
        public IDeviceReport Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new DeviceReport(Array.Empty<byte>());

            // Safe bounds check before accessing data[0]
            if (data.Length < 1)
                return new DeviceReport(data);

            byte reportId = data[0];

            // Handle 10-byte reports from both interfaces
            if (data.Length == 10)
            {
                switch (reportId)
                {
                    case 0x0A: // Main pen/digitizer report
                        return ParsePenReport(data);

                    // Add other report IDs as needed based on HID analysis
                    default:
                        return new DeviceReport(data);
                }
            }

            // Default to generic device report for unknown formats
            return new DeviceReport(data);
        }

        private IDeviceReport ParsePenReport(byte[] data)
        {
            try
            {
                if (data.Length < 10)
                    return new DeviceReport(data);

                // Parse coordinates from the HID dump data pattern
                // Sample data: 0A C0 23 42 18 56 00 00 00 00
                // Need to analyze actual coordinate mapping from HID descriptor

                // Temporary placeholder parsing - needs HID descriptor analysis
                ushort x = (ushort)((data[2] << 8) | data[1]);  // Example: 0x23C0
                ushort y = (ushort)((data[4] << 8) | data[3]);  // Example: 0x4218

                // Pressure and buttons need HID analysis
                ushort pressure = 0;
                bool[] penButtons = new bool[2] { false, false };

                // Check if pen is in range (non-zero coordinates)
                bool inRange = x > 0 || y > 0;

                if (!inRange)
                    return new OutOfRangeReport(data);

                return new TabletReport
                {
                    Raw = data,
                    Position = new Vector2(x, y),
                    Pressure = pressure,
                    PenButtons = penButtons
                };
            }
            catch
            {
                // Fallback to neutral report on any parsing error
                return new DeviceReport(data);
            }
        }
    }
}
