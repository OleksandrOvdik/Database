INSERT INTO Device (Id, Name, IsEnabled)
VALUES
    ('ED-123', 'Security Camera', 1),
    ('PC-456', 'Office PC', 0),
    ('SW-789', 'Fitness Tracker', 1);

INSERT INTO Embedded (lpAddress, NetworkName, DeviceId)
VALUES ('192.168.0.100', 'MD Ltd HQ', 'ED-123');

INSERT INTO PersonalComputer (OperationSystem, DeviceId)
VALUES ('Ubuntu 22.04', 'PC-456');

INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
VALUES (75, 'SW-789');