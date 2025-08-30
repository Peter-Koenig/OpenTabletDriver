# Huion Inspiroy Keydial KD200 - Detection Log Example

## Expected OTD Detection Output

When the KD200 is properly connected and detected, OpenTabletDriver should log messages similar to:

### Successful Detection Log
```
[INFO] OpenTabletDriver.Daemon: Starting OpenTabletDriver 0.7.0.0
[INFO] OpenTabletDriver.Daemon: Scanning for tablets...
[DEBUG] Detect: Found device at /dev/hidrawX: 256C:0064 (Interface 1)
[DEBUG] Detect: Found device at /dev/hidrawY: 256C:0064 (Interface 2) 
[DEBUG] Detect: Found device at /dev/hidrawZ: 256C:0064 (Interface 0)
[DEBUG] RootHub: Processing device 256C:0064 (Huion Tablet_KD200)
[WARN] HidSharpDeviceRootHub: Unable to retrieve device's supported langId: OK
[DEBUG] DeviceIdentifier: Checking device 256C:0064 against configurations (string-independent matching)
[DEBUG] DeviceIdentifier: Matched VID/PID/Interface: 9580/100/1 → Digitizer (93 bytes)
[DEBUG] DeviceIdentifier: Matched VID/PID/Interface: 9580/100/2 → Auxiliary (148 bytes)
[DEBUG] DeviceIdentifier: Ignoring Interface 0 (libinputoverride=1 active)
[INFO] DeviceIdentifier: Detected tablet: Huion Inspiroy Keydial KD200 (256C:0064)
[DEBUG] DeviceIdentifier: Interface assignment successful: IF1=Pen, IF2=Dial/Keys, IF0=Ignored
[DEBUG] DeviceIdentifier: Applying attributes: libinputoverride=1, FeatureInitDelayMs=100
[INFO] TabletManager: Initializing tablet: Huion Inspiroy Keydial KD200
[DEBUG] TabletManager: Using parser: OpenTabletDriver.Configurations.Parsers.Huion.KD200ReportParser
[INFO] TabletManager: Tablet initialized successfully
```

### Interface Separation Confirmation
```
[DEBUG] DeviceIdentifier: Ignoring Interface 0 (HID Mouse) via libinputoverride=1 (preventing double cursor)
[DEBUG] DeviceIdentifier: Assigning Interface 1 to digitizer: ReportLength=93, Parser=KD200ReportParser
[DEBUG] DeviceIdentifier: Assigning Interface 2 to auxiliary: ReportLength=148, Parser=KD200ReportParser
[DEBUG] DeviceIdentifier: String-independent matching successful despite HID string I/O errors
```

### Parser Initialization
```
[DEBUG] ReportParserProvider: Loading KD200ReportParser for Huion Inspiroy Keydial KD200
[DEBUG] KD200ReportParser: Parser initialized - neutral pen handling, dial events active
[DEBUG] KD200ReportParser: String-independent operation enabled (no manufacturer/product dependency)
[DEBUG] KD200ReportParser: Dial inversion attribute: false (default)
```

## Expected Error-Free Operation

After successful detection, the log should show normal operation:

```
[DEBUG] KD200ReportParser: Processing Interface 1 report (93 bytes) - Pen data (neutral positioning)
[DEBUG] KD200ReportParser: Processing Interface 2 report (148 bytes) - Auxiliary data
[DEBUG] KD200ReportParser: Dial report detected (0xf1) - value=+1
[DEBUG] InputManager: Converting dial to REL_WHEEL event (Y-scroll: +1)
[DEBUG] KD200ReportParser: Interface 2 report handled - UCLogicAuxReport for button events
```

## Verification Steps

To confirm successful string-independent detection:

1. **Check for detection message**: Look for "Detected tablet: Huion Inspiroy Keydial KD200"
2. **Verify string-independent matching**: "Matched VID/PID/Interface" without manufacturer strings
3. **Check interface assignment**: Confirm IF1=Pen (93 bytes), IF2=Dial/Keys (148 bytes), IF0=Ignored
4. **Verify libinputoverride**: "Ignoring Interface 0" and "libinputoverride=1 active"
5. **Check parser operation**: Dial events processed despite neutral pen handling

## Troubleshooting

If string-independent detection fails, check for:

- Configuration file not in build output directory
- Parser compilation errors in KD200ReportParser.cs
- USB permission issues for /dev/hidraw* devices
- Conflicting libinput device filtering
- Interface number or report length mismatches

This log example represents the expected successful string-independent detection pattern for the KD200 implementation, working despite "Unable to retrieve device's supported langId" HID string I/O errors.