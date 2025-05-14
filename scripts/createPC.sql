CREATE TABLE PersonalComputer (
                                  Id INT Identity(1,1) PRIMARY KEY,
                                  OperationSystem VARCHAR(255),
                                  DeviceId VARCHAR(255) FOREIGN KEY REFERENCES Device(Id) ON DELETE CASCADE,
                                  RowVer rowversion
);