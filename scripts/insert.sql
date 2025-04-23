INSERT INTO Devices (Id, Name, IsDeviceTurned, Type)
VALUES
    ('ED-123', 'Security Camera', 1, 'ED'),
    ('PC-456', 'Office PC', 0, 'PC'),
    ('SW-789', 'Fitness Tracker', 1, 'SW');

INSERT INTO EmbeddedDevices (DeviceId, IpAddress, NetworkName)
VALUES ('ED-123', '192.168.0.100', 'MD Ltd HQ');

INSERT INTO PersonalComputers (DeviceId, OperatingSystem)
VALUES ('PC-456', 'Ubuntu 22.04');

INSERT INTO SmartWatches (DeviceId, BatteryPercent)
VALUES ('SW-789', 75);