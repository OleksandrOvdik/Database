CREATE TABLE Devices (
                         Id NVARCHAR(255) PRIMARY KEY,
                         Name NVARCHAR(255),
                         IsDeviceTurned BIT,
                         Type NVARCHAR(2) NOT NULL CHECK (Type IN ('ED', 'PC', 'SW'))
);