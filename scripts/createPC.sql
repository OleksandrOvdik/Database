CREATE TABLE PersonalComputers (
                                   DeviceId NVARCHAR(255) PRIMARY KEY,
                                   OperatingSystem NVARCHAR(255),
                                   FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
);