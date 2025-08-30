# Huion Inspiroy Keydial KD200 - HID Data Capture Guide

## 📋 Critical Data Capture Required

This guide provides step-by-step instructions for capturing the HID data needed to complete the KD200 implementation. **Without this data, the parser cannot be finalized.**

## 🔧 Prerequisites

### Required Tools
```bash
# Install usbhid-dump on Debian/Ubuntu
sudo apt install usbhid-dump

# Install evtest for device verification
sudo apt install evtest
```

### Device Verification
```bash
# Check if KD200 is detected
lsusb | grep -i "256c:0064"

# Expected output:
# Bus XXX Device XXX: ID 256c:0064 Huion Tablet_KD200
```

## 📊 HID Data Capture Procedure

### 1. Complete HID Descriptor Capture (MOST IMPORTANT)
```bash
# Capture Interface 1 (Pen/Digitizer) - Report Length: 93
sudo usbhid-dump -d 256c:0064 -i 1.1 -e all > kd200_if1.1_hid_raw.txt

# Interface 1.2 (Dial/Keyboard) - Report Length: 10  
sudo usbhid-dump -d 256c:0064 -i 1.2 -e all > kd200_if1.2_hid_raw.txt

# Capture all interfaces for reference
sudo usbhid-dump -d 256c:0064 -a > kd200_all_interfaces.txt

# Alternative: Get descriptor and stream separately for each interface
# sudo usbhid-dump -d 256c:0064 -i 1.1 -e descriptor > kd200_if1.1_descriptor.txt
# sudo usbhid-dump -d 256c:0064 -i 1.1 -e stream >> kd200_if1.1_complete.txt
# sudo usbhid-dump -d 256c:0064 -i 2 -e descriptor > kd200_if2_descriptor.txt  
# sudo usbhid-dump -d 256c:0064 -i 2 -e stream >> kd200_if2_complete.txt
```

### 2. Structured Test Capture
Create separate capture files for each test scenario:

#### Pen Movement Test
```bash
# Move pen slowly across entire surface in grid pattern
sudo usbhid-dump -d 256c:0064 -i 1 -e stream --stream-timeout 30 > kd200_pen_movement.txt &
# Perform: Top-left → top-right → bottom-right → bottom-left → center
# Then kill the process: kill %1
```

#### Pressure Test
```bash
# Apply varying pressure on tip
sudo usbhid-dump -d 256c:0064 -i 1 -e stream --stream-timeout 20 > kd200_pressure_test.txt &
# Perform: Light pressure → medium → heavy → max → release
```

#### Button Test
```bash
# Test all pen buttons
sudo usbhid-dump -d 256c:0064 -i 1 -e stream --stream-timeout 15 > kd200_buttons_test.txt &
# Perform: Tip click → side button 1 → side button 2 → combinations
```

#### Dial Test
```bash
# Test dial rotation
sudo usbhid-dump -d 256c:0064 -i 2 -e stream --stream-timeout 15 > kd200_dial_test.txt &
# Perform: Clockwise rotation → counterclockwise → various speeds
```

#### Express Keys Test
```bash
# Test all keyboard keys
sudo usbhid-dump -d 256c:0064 -i 2 -e stream --stream-timeout 20 > kd200_keys_test.txt &
# Press each express key multiple times in sequence
```

## 🔍 Data Analysis Checklist

### From HID Descriptor (Interface 1 - Pen)
- [ ] **Report ID**: Identifier byte for pen reports
- [ ] **X Position**: Byte offset (2 bytes), endianness
- [ ] **Y Position**: Byte offset (2 bytes), endianness
- [ ] **Pressure**: Byte offset (2 bytes), value range
- [ ] **Tip Button**: Bit position (0/1 logic)
- [ ] **Side Button 1**: Bit position
- [ ] **Side Button 2**: Bit position
- [ ] **MaxX**: Maximum X value from descriptor
- [ ] **MaxY**: Maximum Y value from descriptor
- [ ] **MaxPressure**: Maximum pressure value

### From HID Descriptor (Interface 2 - Aux)
- [ ] **Report IDs**: 0xe0, 0xe3, 0xf1, etc.
- [ ] **Dial Data**: Byte offset for wheel values
- [ ] **Key Matrix**: Bit positions for express keys
- [ ] **Button States**: Press/release logic

### From Stream Data
- [ ] **Coordinate Range**: Min/Max X/Y values observed
- [ ] **Pressure Range**: Min/Max pressure values
- [ ] **Dial Values**: Range and increment size
- [ ] **Button Patterns**: Bit patterns for each button

## 🧪 OTD Debug Log Capture

```bash
# Run OTD with maximum logging
OpenTabletDriver.Daemon --log-level Trace 2>&1 | tee kd200_otd_debug.log

# Perform this test sequence:
# 1. Hover pen at (0,0) - wait 2 seconds
# 2. Hover pen at (max,max) - wait 2 seconds
# 3. Apply light pressure → medium → heavy
# 4. Click tip button 5 times
# 5. Click side buttons individually
# 6. Rotate dial 5 steps clockwise
# 7. Rotate dial 5 steps counterclockwise
# 8. Press each express key 3 times
```

## 📝 Expected Output Analysis

### Interface 1 (Pen) Reports Should Show:
- Consistent report length: 93 bytes
- Changing X/Y values during movement
- Pressure values from 0 to maximum
- Bit changes for button presses

### Interface 2 (Aux) Reports Should Show:
- Consistent report length: 148 bytes
- Changing values for dial rotation
- Bit changes for express key presses
- Multiple report types (0xe0, 0xe3, 0xf1)

## 🚨 Troubleshooting

### If usbhid-dump fails:
```bash
# Check device permissions
ls -la /dev/bus/usb/*/* | grep 256c

# Try different USB port
# Restart USB services: sudo service udev restart
```

### If no data appears:
- Ensure device is properly connected
- Check dmesg for errors: `dmesg | grep huion`
- Try different USB cable if available

## 📋 File Naming Convention

Use this naming pattern for captured files:
- `kd200_if1_descriptor.txt` - Interface 1 descriptor
- `kd200_if2_stream_buttons.txt` - Interface 2 button tests
- `kd200_otd_debug_full.txt` - Complete OTD debug log
- `kd200_analysis_notes.md` - Your analysis findings

## 📊 Data Submission

Please provide these files for parser completion:
1. `kd200_if1_complete.txt` - Interface 1 full capture
2. `kd200_if2_complete.txt` - Interface 2 full capture
3. `kd200_otd_debug.log` - OTD debug output
4. Any additional test captures

## ⚠️ Important Notes

- Capture data with the device on a flat surface
- Perform tests slowly and deliberately
- Document any unusual behavior observed
- Multiple captures are better than single attempts
- Save raw output without filtering or editing

## 📞 Support

If you encounter issues:
1. Check system logs: `dmesg | tail -20`
2. Verify USB detection: `lsusb -v -d 256c:0064`
3. Test basic functionality: `evtest /dev/input/eventX`

**This data capture is essential for completing the KD200 implementation!**