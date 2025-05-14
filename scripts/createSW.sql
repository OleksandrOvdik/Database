CREATE TABLE Smartwatch (
                            Id INT Identity(1,1) PRIMARY KEY,
                            BatteryPercentage INT,
                            DeviceId VARCHAR(255) FOREIGN KEY REFERENCES Device(Id) ON DELETE CASCADE,
                            RowVer rowversion
);