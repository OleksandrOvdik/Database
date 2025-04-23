CREATE TABLE SmartWatches (
                              DeviceId NVARCHAR(255) PRIMARY KEY,
                              BatteryPercent INT NOT NULL CHECK (BatteryPercent BETWEEN 0 AND 100),
                              FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
);