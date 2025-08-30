# Huion Inspiroy Keydial KD200 Support Implementation

## Device Information
- **Vendor ID**: 0x256C (9580)
- **Product ID**: 0x0064 (100)
- **Product Name**: Huion Tablet_KD200
- **Device Version**: 1.12
- **Serial Number**: 20200922

## USB Interface Configuration
- **Interface 0**: HID Mouse (Report Length: 18) - **IGNORED** with libinputoverride=1
- **Interface 1**: HID Mouse (Report Length: 93) - **Digitizer/Pen** (Primary input)
- **Interface 2**: HID Generic (Report Length: 148) - **Dial/Keyboard** (Auxiliary input)

## Linux evdev Devices
- `HUION Huion Tablet_KD200 Pen` (eventX) - EV=1b, ABS=d000003 (X/Y/Pressure), KEY=c03
- `System Multi Axis` (eventY) - REL=80 (Dial/Wheel)
- `Keyboard` (eventZ) - Large KEY bitfield (Express Keys)

## Current Implementation Status

### ✅ Implemented Features
- Basic device detection via VID/PID (0x256C/0x0064)
- Interface separation (1=Pen, 2=Dial/Keys, 0=ignored)
- Preliminary pen support with absolute positioning
- Pressure sensitivity (MaxPressure=8192)
- Side button detection (2 buttons)
- Dial scroll wheel emulation
- Express key support (handled as OS keyboard input)

### ⚠️ Temporary Values (Need HID Data Capture)
- **MaxX/MaxY**: 32767 (placeholder - needs actual resolution)
- **Width/Height**: 226×143 mm (active area - confirmed)
- **Pressure Curve**: Linear 0-8192 (needs verification)
- **Dial Sensitivity**: 1:1 mapping (needs calibration)
- **Express Keys**: Direct OS keyboard input (HID keyboard device, not OTD auxiliary)

## HID Data Capture Procedure (CRITICAL FOR FINAL IMPLEMENTATION)

### 1. OTD Tablet Debugger - Capture Raw Reports
```bash
# Run OpenTabletDriver with maximum debug logging
OpenTabletDriver.Daemon --log-level Trace

# Perform these actions systematically:
# 1. Hover pen: Move to all four corners and center
# 2. Pressure test: Light → heavy pressure on tip
# 3. Buttons: Press tip, then each side button separately
# 4. Dial: Rotate clockwise/counterclockwise multiple steps
# 5. Express keys: Press each key individually (handled as OS keyboard)
# 6. Save complete log: tablet-debug-log.txt

**Note**: Express keys are handled as direct OS keyboard input via Interface 2 HID device, not through OTD auxiliary button system.
```

### 2. USB HID Data Capture - Essential for Parser Development
```bash
# REQUIRED: Install usbhid-dump for detailed HID analysis
sudo apt install usbhid-dump

# CRITICAL: Capture Report Descriptors and Stream Data
# Interface 1 (Pen/Digitizer) - Report Length: 93
sudo usbhid-dump -d 256c:0064 -i 1 -e descriptor,stream > kd200_if1_hid_raw.txt

# Interface 2 (Dial/Keyboard) - Report Length: 148  
sudo usbhid-dump -d 256c:0064 -i 2 -e descriptor,stream > kd200_if2_hid_raw.txt

# Perform during data capture:
# - Move pen across full surface while capturing
# - Rotate dial in both directions
# - Press all buttons during capture
```

## HID Data Analysis - Parser Development Requirements

### Interface 1 (Pen) - Critical Offsets Needed:
- **Report Descriptor**: HID usage pages and report structure
- **X Position**: Exact byte offset and endianness (2 bytes)
- **Y Position**: Exact byte offset and endianness (2 bytes)  
- **Pressure**: Byte offset and bit depth (2 bytes)
- **Buttons**: Bit positions for tip/side buttons in report
- **Report ID**: Identifier byte for pen reports
- **Max Values**: Actual MaxX, MaxY, MaxPressure from descriptor

### Interface 2 (Dial/Keys) - Critical Offsets Needed:
- **Report Descriptor**: Dial and keyboard usage pages
- **Dial Data**: Byte offset for wheel values (signed byte)
- **Key Matrix**: Button bit positions and mapping
- **Report IDs**: Different report types (0xe0, 0xe3, 0xf1, etc.)
- **Endianness**: Byte order for multi-byte values

## Implementation Validation Checklist

### Basic Functionality Tests
- [ ] Tablet detected by OTD as "Huion Inspiroy Keydial KD200"
- [ ] Pen movement translates to cursor movement (absolute mode)
- [ ] Tip click generates left mouse click
- [ ] Pressure sensitivity works (0 → max) - REQUIRES HID OFFSETS
- [ ] Side buttons are detected - REQUIRES HID BUTTON BITS
- [ ] Dial rotation generates scroll events - REQUIRES HID OFFSET
- [ ] Express keys generate OS keyboard events (direct HID keyboard, not OTD auxiliary)
- [ ] No double cursor from Interface 0 (libinputoverride=1 working)

### Advanced Validation (After HID Analysis)
- [ ] Coordinate mapping matches physical dimensions (226×143mm)
- [ ] Pressure curve is smooth and linear - REQUIRES MAXPRESSURE
- [ ] Dial scroll direction is correct - REQUIRES HID ANALYSIS
- [ ] No input lag or jitter
- [ ] Multiple simultaneous inputs work
- [ ] All button bits correctly mapped from HID data

## TODO - Blocked by HID Data Capture

### Immediate Actions (BLOCKED - REQUIRES HID DATA)
1. **CAPTURE HID REPORTS**: usbhid-dump for both interfaces (MANDATORY)
2. **ANALYZE REPORT DESCRIPTORS**: Determine exact byte offsets
3. **EXTRACT CRITICAL VALUES**: MaxX, MaxY, MaxPressure from descriptor
4. **MAP BUTTON BITS**: Tip, side buttons, express keys from HID data
5. **CONFIRM DIAL OFFSET**: Verify data[5] position for wheel values

### Parser Improvements (REQUIRES HID DATA)
- [ ] Update `KD200ReportParser` with exact byte offsets from HID analysis
- [ ] Implement proper coordinate scaling based on actual MaxX/MaxY
- [ ] Add pressure curve based on actual MaxPressure value
- [ ] Map button bits to correct positions from HID report
- [ ] Confirm dial byte offset and direction from HID data

### Configuration Refinements (REQUIRES HID DATA)
- [ ] Set exact MaxX/MaxY values from HID descriptor analysis
- [ ] Configure proper MaxPressure value from HID data
- [ ] Add tilt support if available in HID reports
- [ ] Optimize initialization sequences based on HID analysis
- [ ] Verify interface assignments match HID descriptor structure

### Binding Recommendations
- **Tip**: Left mouse click
- **Side Button 1**: Right mouse click
- **Side Button 2**: Middle mouse click  
- **Dial**: Vertical scroll
- **Express Keys**: Application-specific shortcuts

## Troubleshooting

### Common Issues
- **No detection**: Check USB permissions, try different USB port
- **Double cursor**: Ensure libinputoverride=1 is working
- **Incorrect coordinates**: Verify MaxX/MaxY values from HID data
- **Missing inputs**: Check interface-specific report parsing

### Debug Commands
```bash
# List USB devices
lsusb | grep 256c

# Check kernel messages
dmesg | grep huion

# Test evdev events
evtest /dev/input/eventX  # Replace with actual event number
```

## References
- Linux HID documentation
- USB HID specification
- Existing Huion tablet configurations
- OTD developer documentation

## Support Contact
- OpenTabletDriver GitHub Issues
- Huion Linux support communities
- USB HID analysis tools documentation

*Last Updated: [Date] - Initial implementation, needs HID data capture*