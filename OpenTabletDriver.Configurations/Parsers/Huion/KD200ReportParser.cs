using OpenTabletDriver.Configurations.Parsers.UCLogic;
using OpenTabletDriver.Tablet;
using System;
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
                return new DeviceReport(Array.Empty<byte>());

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
                Position = new Vector2(0, 0), // TODO: Replace with actual X/Y offsets from HID analysis
                Pressure = 0,                 // TODO: Replace with actual pressure offset from HID analysis
                PenButtons = new bool[2]      // TODO: Replace with actual button bit positions from HID dump
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

            // Safe bounds check before accessing data[0]
            if (data.Length > 0)
            {
                // Handle dial reports specifically
                if (data[0] == 0xf1 && data.Length > 5)
                {
                    return ParseDialReport(data);
                }

                // Handle other auxiliary report types (buttons, etc.)
                if (data[0] == 0xe0 || data[0] == 0xe3)
                {
                    return new UCLogicAuxReport(data);
                }
            }

            // Default to generic device report for unknown formats
            return new DeviceReport(data);
        }


        // KD200ReportParser.cs

        public IDeviceReport Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new DeviceReport(Array.Empty<byte>());

            if (data.Length < 1)
                return new DeviceReport(data);

            switch (data)
            {
                case 0xe0:
                case 0xe3:
                    return new UCLogicAuxReport(data); // Aux-Buttons
                case 0xf1:
                    // Vorläufig neutral behandeln (wie Inspiroy), bis Dial-Struktur geklärt ist
                    return new DeviceReport(data);
                case 0x00:
                    return new OutOfRangeReport(data);
            }

            // Rest unverändert
            if (data.Length == 93) return HandlePenReportNeutral(data);
            if (data.Length == 148) return HandleAuxiliaryReportNeutral(data);
            return new DeviceReport(data);
        }
    }
}
