# Huion Inspiroy Keydial KD200 - Detection Log Example

## Expected OTD Detection Output

When the KD200 is properly connected and detected, OpenTabletDriver should log messages similar to:

### Successful Detection Log
```
[INFO]  OpenTabletDriver.Daemon: Starting OpenTabletDriver 0.7.0.0
[INFO]  OpenTabletDriver.Daemon: Scanning for tablets...
[DEBUG] DeviceIdentifier: Found USB device: VID=0x256C, PID=0x0064, Manufacturer=Huion, Product=Tablet_KD200
[DEBUG] DeviceIdentifier: Checking device 256C:0064 against configurations
[DEBUG] DeviceIdentifier: Matched 'Huion Inspiroy Keydial KD200' to device 256C:0064
[INFO]  DeviceIdentifier: Detected tablet: Huion Inspiroy Keydial KD200 (256C:0064)
[DEBUG] DeviceIdentifier: Interface assignment: IF1=Digitizer (93 bytes), IF2=Auxiliary (148 bytes), IF0=Ignored
[DEBUG] DeviceIdentifier: Applying attributes: libinputoverride=1, FeatureInitDelayMs=100
[INFO]  TabletManager: Initializing tablet: Huion Inspiroy Keydial KD200
[DEBUG] TabletManager: Using parser: OpenTabletDriver.Configurations.Parsers.Huion.KD200ReportParser
[INFO]  TabletManager: Tablet initialized successfully
```

### Interface Separation Confirmation
```
[DEBUG] DeviceIdentifier: Ignoring interface 0 (HID Mouse) due to libinputoverride=1
[DEBUG] DeviceIdentifier: Assigning interface 1 to digitizer (Report length: 93)
[DEBUG] DeviceIdentifier: Assigning interface 2 to auxiliary device (Report length: 148)
[DEBUG] DeviceIdentifier: No double cursor protection active for interface 0
```

### Parser Initialization
```
[DEBUG] ReportParserProvider: Loading KD200ReportParser for Huion Inspiroy Keydial KD200
[DEBUG] KD200ReportParser: Parser initialized with neutral handling
[DEBUG] KD200ReportParser: Dial inversion: false (default)
```

## Expected Error-Free Operation

After successful detection, the log should show normal operation:

```
[DEBUG] KD200ReportParser: Processing Interface 1 report (93 bytes) - Pen data
[DEBUG] KD200ReportParser: Processing Interface 2 report (148 bytes) - Dial data
[DEBUG] KD200ReportParser: Dial event detected: value=+1
[DEBUG] InputManager: Converting dial to REL_WHEEL event
```

## Verification Steps

To confirm successful detection:

1. **Check for detection message**: Look for "Detected tablet: Huion Inspiroy Keydial KD200"
2. **Verify interface assignment**: Confirm IF1=Digitizer, IF2=Auxiliary, IF0=Ignored
3. **Check parser loading**: Ensure KD200ReportParser is loaded
4. **Verify attributes**: libinputoverride=1 should be applied

## Troubleshooting

If detection fails, check for:

- Missing configuration file in build output
- Parser compilation errors
- USB permission issues
- Conflicting device drivers

This log example represents the expected successful detection pattern for the KD200 implementation.