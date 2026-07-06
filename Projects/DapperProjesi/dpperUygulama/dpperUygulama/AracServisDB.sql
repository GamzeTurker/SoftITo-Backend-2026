-- AracServis Veritabanı ve Tablo Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AracServis')
BEGIN
    ALTER DATABASE AracServis SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AracServis;
END
GO

CREATE DATABASE AracServis;
GO

USE AracServis;
GO

-- 1. Customer (Müşteriler) Tablosu
CREATE TABLE Customer (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL
);
GO

-- 2. Employee (Personel) Tablosu
CREATE TABLE Employee (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeName NVARCHAR(100) NOT NULL,
    JobTitle NVARCHAR(100) NULL
);
GO

-- 3. Vehicle (Araçlar) Tablosu
CREATE TABLE Vehicle (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    Brand NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NULL,
    Plate NVARCHAR(20) NOT NULL,
    CustomerId INT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId) ON DELETE CASCADE
);
GO

-- 4. ServiceRecord (Servis Kayıtları) Tablosu
CREATE TABLE ServiceRecord (
    ServiceId INT IDENTITY(1,1) PRIMARY KEY,
    VehicleId INT NOT NULL,
    EmployeeId INT NOT NULL,
    ServiceDate DATETIME NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Cost DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (VehicleId) REFERENCES Vehicle(VehicleId) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId) ON DELETE CASCADE
);
GO


-- ========================================================
-- TEST VERİLERİ (SEED DATA)
-- ========================================================

-- Müşteriler
INSERT INTO Customer (FullName, Phone, Address) VALUES 
(N'Ahmet Yılmaz', '05321112233', N'Kadıköy, İstanbul'),
(N'Mehmet Kaya', '05432223344', N'Çankaya, Ankara'),
(N'Ayşe Demir', '05553334455', N'Bornova, İzmir'),
(N'Fatma Çelik', '05054445566', N'Nilüfer, Bursa');
GO

-- Personel
INSERT INTO Employee (EmployeeName, JobTitle) VALUES 
(N'Usta Ali Tekin', N'Motor Ustası'),
(N'Teknisyen Can Yılmaz', N'Oto Elektrik'),
(N'Usta Murat Şahin', N'Kaporta Ustası'),
(N'Asistan Ömer Koç', N'Çırak');
GO

-- Araçlar
INSERT INTO Vehicle (Brand, Model, Plate, CustomerId) VALUES 
('Toyota', 'Corolla', '34ABC123', 1),
('Ford', 'Focus', '06DEF456', 2),
('Renault', 'Clio', '35GHI789', 3),
('Volkswagen', 'Golf', '16JKL012', 4),
('BMW', '320i', '34XYZ999', 1);
GO

-- Servis Kayıtları
INSERT INTO ServiceRecord (VehicleId, EmployeeId, ServiceDate, Description, Cost) VALUES 
(1, 1, '2026-06-15 10:00:00', N'Periyodik motor bakımı ve yağ değişimi', 2500.00),
(2, 2, '2026-06-18 14:30:00', N'Akü değişimi ve far ayarları yapıldı', 1800.00),
(3, 3, '2026-06-20 09:15:00', N'Ön çamurluk boya ve kaporta düzeltme', 5000.00),
(4, 1, '2026-06-25 11:00:00', N'Buji değişimi ve enjektör temizliği', 1500.00),
(5, 2, '2026-06-28 16:00:00', N'Klima gazı dolumu ve elektrik tesisat kontrolü', 1200.00),
(1, 3, '2026-07-02 13:00:00', N'Fren balatası ve disk değişimi', 3200.00);
GO


-- ========================================================
-- STORED PROCEDURE'LER
-- ========================================================

-- MÜŞTERİ PROCEDÜRLERİ
CREATE PROCEDURE CustomerViewAll
AS
BEGIN
    SELECT * FROM Customer ORDER BY CustomerId DESC;
END
GO

CREATE PROCEDURE CustomerViewById
    @CustomerId INT
AS
BEGIN
    SELECT * FROM Customer WHERE CustomerId = @CustomerId;
END
GO

CREATE PROCEDURE CustomerEY
    @CustomerId INT,
    @FullName NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Address NVARCHAR(500)
AS
BEGIN
    IF @CustomerId = 0
    BEGIN
        INSERT INTO Customer (FullName, Phone, Address)
        VALUES (@FullName, @Phone, @Address);
    END
    ELSE
    BEGIN
        UPDATE Customer
        SET FullName = @FullName,
            Phone = @Phone,
            Address = @Address
        WHERE CustomerId = @CustomerId;
    END
END
GO

CREATE PROCEDURE CustomerDelete
    @CustomerId INT
AS
BEGIN
    DELETE FROM Customer WHERE CustomerId = @CustomerId;
END
GO

CREATE PROCEDURE CustomerSearch
    @SearchText NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Customer
    WHERE FullName LIKE '%' + @SearchText + '%'
       OR Phone LIKE '%' + @SearchText + '%'
       OR Address LIKE '%' + @SearchText + '%'
    ORDER BY CustomerId DESC;
END
GO


-- PERSONEL PROCEDÜRLERİ
CREATE PROCEDURE EmployeeViewAll
AS
BEGIN
    SELECT * FROM Employee ORDER BY EmployeeId DESC;
END
GO

CREATE PROCEDURE EmployeeViewById
    @EmployeeId INT
AS
BEGIN
    SELECT * FROM Employee WHERE EmployeeId = @EmployeeId;
END
GO

CREATE PROCEDURE EmployeeEY
    @EmployeeId INT,
    @EmployeeName NVARCHAR(100),
    @JobTitle NVARCHAR(100)
AS
BEGIN
    IF @EmployeeId = 0
    BEGIN
        INSERT INTO Employee (EmployeeName, JobTitle)
        VALUES (@EmployeeName, @JobTitle);
    END
    ELSE
    BEGIN
        UPDATE Employee
        SET EmployeeName = @EmployeeName,
            JobTitle = @JobTitle
        WHERE EmployeeId = @EmployeeId;
    END
END
GO

CREATE PROCEDURE EmployeeDelete
    @EmployeeId INT
AS
BEGIN
    DELETE FROM Employee WHERE EmployeeId = @EmployeeId;
END
GO

CREATE PROCEDURE EmployeeSearch
    @SearchText NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Employee
    WHERE EmployeeName LIKE '%' + @SearchText + '%'
       OR JobTitle LIKE '%' + @SearchText + '%'
    ORDER BY EmployeeId DESC;
END
GO


-- ARAÇ PROCEDÜRLERİ
CREATE PROCEDURE VehicleViewAll
AS
BEGIN
    SELECT * FROM Vehicle ORDER BY VehicleId DESC;
END
GO

CREATE PROCEDURE VehicleViewById
    @VehicleId INT
AS
BEGIN
    SELECT * FROM Vehicle WHERE VehicleId = @VehicleId;
END
GO

CREATE PROCEDURE VehicleEY
    @VehicleId INT,
    @Brand NVARCHAR(100),
    @Model NVARCHAR(100),
    @Plate NVARCHAR(20),
    @CustomerId INT
AS
BEGIN
    IF @VehicleId = 0
    BEGIN
        INSERT INTO Vehicle (Brand, Model, Plate, CustomerId)
        VALUES (@Brand, @Model, @Plate, @CustomerId);
    END
    ELSE
    BEGIN
        UPDATE Vehicle
        SET Brand = @Brand,
            Model = @Model,
            Plate = @Plate,
            CustomerId = @CustomerId
        WHERE VehicleId = @VehicleId;
    END
END
GO

CREATE PROCEDURE VehicleDelete
    @VehicleId INT
AS
BEGIN
    DELETE FROM Vehicle WHERE VehicleId = @VehicleId;
END
GO

CREATE PROCEDURE VehicleSearch
    @SearchText NVARCHAR(100)
AS
BEGIN
    SELECT v.* 
    FROM Vehicle v
    LEFT JOIN Customer c ON v.CustomerId = c.CustomerId
    WHERE v.Brand LIKE '%' + @SearchText + '%'
       OR v.Model LIKE '%' + @SearchText + '%'
       OR v.Plate LIKE '%' + @SearchText + '%'
       OR c.FullName LIKE '%' + @SearchText + '%'
    ORDER BY v.VehicleId DESC;
END
GO


-- SERVİS PROCEDÜRLERİ
CREATE PROCEDURE ServiceViewAll
AS
BEGIN
    SELECT sr.ServiceId, sr.VehicleId, sr.EmployeeId, sr.ServiceDate, sr.Description, sr.Cost,
           v.Brand, v.Plate, e.EmployeeName, c.FullName
    FROM ServiceRecord sr
    INNER JOIN Vehicle v ON sr.VehicleId = v.VehicleId
    INNER JOIN Employee e ON sr.EmployeeId = e.EmployeeId
    INNER JOIN Customer c ON v.CustomerId = c.CustomerId
    ORDER BY sr.ServiceId DESC;
END
GO

CREATE PROCEDURE ServiceViewById
    @ServiceId INT
AS
BEGIN
    SELECT sr.ServiceId, sr.VehicleId, sr.EmployeeId, sr.ServiceDate, sr.Description, sr.Cost,
           v.Brand, v.Plate, e.EmployeeName, c.FullName
    FROM ServiceRecord sr
    INNER JOIN Vehicle v ON sr.VehicleId = v.VehicleId
    INNER JOIN Employee e ON sr.EmployeeId = e.EmployeeId
    INNER JOIN Customer c ON v.CustomerId = c.CustomerId
    WHERE sr.ServiceId = @ServiceId;
END
GO

CREATE PROCEDURE ServiceEY
    @ServiceId INT,
    @VehicleId INT,
    @EmployeeId INT,
    @ServiceDate DATETIME,
    @Description NVARCHAR(MAX),
    @Cost DECIMAL(18,2)
AS
BEGIN
    IF @ServiceId = 0
    BEGIN
        INSERT INTO ServiceRecord (VehicleId, EmployeeId, ServiceDate, Description, Cost)
        VALUES (@VehicleId, @EmployeeId, @ServiceDate, @Description, @Cost);
    END
    ELSE
    BEGIN
        UPDATE ServiceRecord
        SET VehicleId = @VehicleId,
            EmployeeId = @EmployeeId,
            ServiceDate = @ServiceDate,
            Description = @Description,
            Cost = @Cost
        WHERE ServiceId = @ServiceId;
    END
END
GO

CREATE PROCEDURE ServiceDelete
    @ServiceId INT
AS
BEGIN
    DELETE FROM ServiceRecord WHERE ServiceId = @ServiceId;
END
GO

CREATE PROCEDURE ServiceSearch
    @SearchText NVARCHAR(100)
AS
BEGIN
    SELECT sr.ServiceId, sr.VehicleId, sr.EmployeeId, sr.ServiceDate, sr.Description, sr.Cost,
           v.Brand, v.Plate, e.EmployeeName, c.FullName
    FROM ServiceRecord sr
    INNER JOIN Vehicle v ON sr.VehicleId = v.VehicleId
    INNER JOIN Employee e ON sr.EmployeeId = e.EmployeeId
    INNER JOIN Customer c ON v.CustomerId = c.CustomerId
    WHERE v.Brand LIKE '%' + @SearchText + '%'
       OR v.Plate LIKE '%' + @SearchText + '%'
       OR e.EmployeeName LIKE '%' + @SearchText + '%'
       OR c.FullName LIKE '%' + @SearchText + '%'
       OR sr.Description LIKE '%' + @SearchText + '%'
    ORDER BY sr.ServiceId DESC;
END
GO


-- RAPOR PROCEDÜRLERİ
CREATE PROCEDURE ServiceReport
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM ServiceRecord) AS TotalService,
        (SELECT ISNULL(SUM(Cost), 0) FROM ServiceRecord) AS TotalIncome;
END
GO

CREATE PROCEDURE TopVehicleReport
AS
BEGIN
    SELECT TOP 1 v.Brand, v.Plate, COUNT(sr.ServiceId) AS ServiceCount
    FROM ServiceRecord sr
    INNER JOIN Vehicle v ON sr.VehicleId = v.VehicleId
    GROUP BY v.Brand, v.Plate
    ORDER BY ServiceCount DESC;
END
GO

CREATE PROCEDURE TopEmployeeReport
AS
BEGIN
    SELECT TOP 1 e.EmployeeName, COUNT(sr.ServiceId) AS ServiceCount
    FROM ServiceRecord sr
    INNER JOIN Employee e ON sr.EmployeeId = e.EmployeeId
    GROUP BY e.EmployeeName
    ORDER BY ServiceCount DESC;
END
GO

CREATE PROCEDURE TotalIncomeReport
AS
BEGIN
    SELECT (SELECT ISNULL(SUM(Cost), 0) FROM ServiceRecord) AS TotalIncome;
END
GO
