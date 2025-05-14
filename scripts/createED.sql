CREATE TABLE Embedded (
                          Id INT Identity(1,1) PRIMARY KEY,
                          IpAddress VARCHAR(255),
                          NetworkName VARCHAR(255),
                          DeviceId VARCHAR(255) FOREIGN KEY REFERENCES Device(Id) ON DELETE CASCADE,
                          RowVer rowversion
);