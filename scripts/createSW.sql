CREATE TABLE Smartwatch (
                            Id INT PRIMARY KEY,
                            BatteryPercentage INT,
                            DeviceId VARCHAR(255) FOREIGN KEY REFERENCES Device(Id) ON DELETE CASCADE
);