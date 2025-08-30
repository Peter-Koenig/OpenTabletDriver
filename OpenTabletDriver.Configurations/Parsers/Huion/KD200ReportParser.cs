using OpenTabletDriver.Configurations.Parsers.UCLogic;
using OpenTabletDriver.Tablet;
using System.Numerics;

namespace OpenTabletDriver.Configurations.Parsers.Huion
{
    public class KD200ReportParser : IReportParser<IDeviceReport>
    {
        // TODO: Add configurable dial inversion attribute
        // private bool _invertDialDirection = false;

        public IDeviceReport Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new DeviceReport(data);

            // Safe bounds check before accessing data[0]
            if (data.Length < 1)
                return new DeviceReport(data);

            // Switch on report ID from data[0] instead of data[1]
            switch (data[0])
            {
                case 0xe0:
                case 0xe3:
                    // Auxiliary buttons report - use standard parser
                    return new UCLogicAuxReport(data);
                case 0xf1:
                    // Wheel/dial data - handle with bounds checking
                    return ParseDialReport(data);
                case 0x00:
                    return new OutOfRangeReport(data);
            }

            // For pen reports, use neutral handling until HID data is available
            // BLOCKED BY HID DUMPS - Need actual report structure analysis
            if (data.Length == 93) // Interface 1 reports
            {
                return HandlePenReportNeutral(data);
            }

            if (data.Length == 148) // Interface 2 reports
            {
                return HandleAuxiliaryReportNeutral(data);
            }

            // Default to generic device report for unknown formats
            return new DeviceReport(data);
        }

        private IDeviceReport HandlePenReportNeutral(byte[] data)
        {
            // BLOCKED BY HID DUMPS - Neutral handling until HID analysis
            // TODO: Replace with actual byte offsets from usbhid-dump analysis
            // X/Y/Pressure/Button offsets need to be determined from HID descriptor

            // For now, return basic tablet report without assumptions
            return new TabletReport
            {
                Raw = data,
                Position = new Vector2(0, 0), // BLOCKED - Will be updated after HID analysis
                Pressure = 0,                 // BLOCKED - Will be updated after HID analysis
                PenButtons = new bool[2]      // BLOCKED - Neutral button state
                {
                    false, // Tip button - TODO: determine actual bit position from HID dump
                    false  // Side button - TODO: determine actual bit position from HID dump
                }
            };
        }

        private IDeviceReport HandleAuxiliaryReportNeutral(byte[] data)
        {
            // Neutral handling for auxiliary reports (Interface 2)
            // This includes both dial and keyboard events

            // For now, check for potential dial data with safe bounds
            if (data.Length > 5 && data[0] == 0xf1)
            {
                return ParseDialReport(data);
            }

            // Default to auxiliary button report for other cases
            return new UCLogicAuxReport(data);
        }

        private IDeviceReport ParseDialReport(byte[] data)
        {
            try
            {
                // Safe bounds checking for dial data
                if (data.Length < 6)
                    return new DeviceReport(data);

                // Extract dial value from data[5] (confirmed position)
                sbyte dialValue = (sbyte)data[5];

                // TODO: Implement dial direction inversion based on config attribute
                // if (_invertDialDirection)
                //     dialValue = (sbyte)(-dialValue);

                // Create relative report for scroll events
                return new RelativeTabletReport
                {
                    Raw = data,
                    Relative = new Vector2(0, dialValue)
                };
            }
            catch
            {
                return new DeviceReport(data);
            }
        }
    }
}
