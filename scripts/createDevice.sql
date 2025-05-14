CREATE TABLE Device (
                        Id VARCHAR(255) PRIMARY KEY,
                        Name VARCHAR(255),
                        IsEnabled BIT,
                        RowVer rowversion
);