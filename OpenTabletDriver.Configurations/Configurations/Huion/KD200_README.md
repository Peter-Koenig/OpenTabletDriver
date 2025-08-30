# Huion Inspiroy Keydial KD200 Support Implementation

## Device Information
- **Vendor ID**: 0x256C (9580)
- **Product ID**: 0x0064 (100)
- **Product Name**: Huion Tablet_KD200
- **Device Version**: 1.12
- **Serial Number**: 20200922
- **Detection Method**: VID/PID + Interface + Report Length (String-independent matching)

## USB Interface Configuration
- **Interface 1.0**: HID Mouse (Report Length: 18) - **IGNORED** via libinputoverride="1" (prevents double cursor)
- **Interface 1.1**: Digitizer (Report Length: 10) - **Pen Input** (Absolute positioning, pressure, buttons)
- **Interface 1.2**: Auxiliary (Report Length: 10) - **Dial & Keyboard** (Relative scroll, express keys)

## Linux evdev Devices
- `HUION Huion Tablet_KD200 Pen` (eventX) - EV=1b, ABS=d000003 (X/Y/Pressure), KEY=c03
- `System Multi Axis` (eventY) - REL=80 (Dial/Wheel)
- `Keyboard` (eventZ) - Large KEY bitfield (Express Keys)

## Current Implementation Status

### ✅ Implemented Features
- Robust device detection via VID/PID + Interface + Report Length (no string dependency)
- Interface separation (1.1=Pen, 1.2=Dial/Keys, 1.0=ignored via libinputoverride)
- Basic pen support with coordinate parsing from 10-byte reports
- Report ID 0x0A handling for pen input
- String-independent matching (works despite "Unable to retrieve device's supported langId" errors)

### ⚠️ Temporary Values - BLOCKED BY HID DATA CAPTURE
- **MaxX/MaxY**: 32767 (placeholder - TODO: extract from HID descriptor analysis)
- **Width/Height**: 226×143 mm (preliminary - TODO: verify from actual coordinate ranges)
- **MaxPressure**: 8192 (placeholder - TODO: determine actual maximum from HID reports)
- **Coordinate Offsets**: X/Y positions parsed from bytes 1-4 - TODO: verify exact mapping from HID analysis
- **Pressure Offset**: Currently 0 - TODO: locate pressure byte position in 10-byte reports
- **Button Bits**: Neutral state - TODO: map tip/side button bit positions from HID data
- **Report Format**: 10-byte reports with ID 0x0A - needs complete HID descriptor analysis

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

# CRITICAL: Capture Complete HID Data for Parser Completion
# Interface 1 (Pen/Digitizer) - Report Length: 93
sudo usbhid-dump -d 256c:0064 -i 1 -e all > kd200_if1_hid_raw.txt

# Interface 2 (Dial/Keyboard) - Report Length: 148  
sudo usbhid-dump -d 256c:0064 -i 2 -e all > kd200_if2_hid_raw.txt

# Alternative: Capture descriptor and stream separately
# sudo usbhid-dump -d 256c:0064 -i 1.1 -e descriptor > kd200_if1.1_descriptor.txt
# sudo usbhid-dump -d 256c:0064 -i 1.1 -e stream >> kd200_if1.1_complete.txt

# Note: "descriptor,stream" is invalid syntax; use "-e all" or separate commands

# Perform during data capture:
# - Move pen slowly across entire surface (record min/max coordinates)
# - Apply varying pressure levels (light → medium → heavy → max)
# - Click all pen buttons individually and in combinations
# - Rotate dial clockwise and counterclockwise multiple steps
# - Press express keys systematically
```

## HID Data Analysis - Critical for Parser Completion

### Interface 1.1 (Pen) - Critical Offsets Needed:
- **Report Descriptor**: HID usage pages and complete report structure
- **X Position**: Exact byte offset and endianness (2 bytes) - TODO: verify from HID dump (currently bytes 1-2)
- **Y Position**: Exact byte offset and endianness (2 bytes) - TODO: verify from HID dump (currently bytes 3-4)  
- **Pressure**: Byte offset and bit depth (2 bytes) - TODO: locate in 10-byte reports
- **Buttons**: Bit positions for tip/side buttons - TODO: map from button test captures
- **Report ID**: Currently 0x0A - TODO: confirm all report types from HID analysis
- **Max Values**: Actual MaxX, MaxY, MaxPressure from descriptor - TODO: extract from HID data

### Interface 1.2 (Dial/Keys) - Critical Offsets Needed:
- **Report Descriptor**: Dial and keyboard usage pages - TODO: analyze descriptor
- **Dial Data**: Byte offset for wheel values (signed byte) - TODO: locate in 10-byte reports
- **Key Matrix**: Button bit positions and mapping - TODO: map express key bits
- **Report IDs**: Currently 0x0A - TODO: identify different report types from HID analysis
- **Endianness**: Byte order for multi-byte values - TODO: determine from HID analysis

## Implementation Validation Checklist - Current Status

### Basic Functionality Tests
- [✅] Tablet detected by OTD via VID/PID+Interface matching (string-independent)
- [⚠️] Pen movement: Neutral positioning (0,0) - TODO: implement actual coordinate offsets
- [⚠️] Tip click: Neutral state - TODO: implement button bit mapping from HID data
- [⚠️] Pressure sensitivity: Currently 0 - TODO: implement pressure offset from HID analysis
- [⚠️] Side buttons: Neutral state - TODO: map button bits from HID reports
- [✅] Dial rotation: Generates REL_WHEEL events (0xf1 reports processed)
- [✅] Express keys: Handled as OS keyboard input (direct HID device)
- [✅] No double cursor: Interface 0 ignored via libinputoverride="1"

### Advanced Validation (BLOCKED - REQUIRES HID ANALYSIS)
- [ ] Coordinate mapping matches physical dimensions (226×143mm) - TODO: after HID offsets
- [ ] Pressure curve is smooth and linear - TODO: after MaxPressure and offset determination
- [ ] Dial scroll direction is correct - TODO: verify dial byte offset and sign from HID data
- [ ] No input lag or jitter - TODO: test with complete implementation
- [ ] Multiple simultaneous inputs work - TODO: test pen+buttons+dial combinations
- [ ] All button bits correctly mapped from HID data - TODO: after button bit analysis

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