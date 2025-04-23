CREATE TABLE EmbeddedDevices (
                                 DeviceId NVARCHAR(255) PRIMARY KEY,
                                 IpAddress NVARCHAR(15) NOT NULL,
                                 NetworkName NVARCHAR(255) NOT NULL,
                                 FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
);