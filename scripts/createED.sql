CREATE TABLE Embedded (
                          Id INT PRIMARY KEY,
                          lpAddress VARCHAR(255),
                          NetworkName VARCHAR(255),
                          DeviceId VARCHAR(255) FOREIGN KEY REFERENCES Device(Id) ON DELETE CASCADE
);