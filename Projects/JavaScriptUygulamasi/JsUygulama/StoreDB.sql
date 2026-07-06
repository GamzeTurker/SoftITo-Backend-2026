-- StoreDB Veritabanı ve Tablo Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'StoreDB')
BEGIN
    ALTER DATABASE StoreDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE StoreDB;
END
GO

CREATE DATABASE StoreDB;
GO

USE StoreDB;
GO

-- 1. Admins Tablosu
CREATE TABLE Admins (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Username NVARCHAR(50) NOT NULL
);
GO

-- 2. Customers Tablosu
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Address NVARCHAR(200) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME2 NOT NULL,
    Email NVARCHAR(100) NULL,
    Name NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(15) NOT NULL
);
GO

-- 3. Employees Tablosu
CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    Position NVARCHAR(50) NOT NULL,
    Salary DECIMAL(18,2) NOT NULL
);
GO

-- 4. Stores Tablosu
CREATE TABLE Stores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    City NVARCHAR(50) NOT NULL,
    EmployeeCapacity INT NULL,
    Phone NVARCHAR(15) NULL,
    StoreName NVARCHAR(100) NOT NULL
);
GO

-- 5. Users Tablosu
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Username NVARCHAR(50) NOT NULL
);
GO


-- ========================================================
-- TEST VERİLERİ (SEED DATA) - TÜRKÇE KARAKTERLERLE
-- ========================================================

-- Admins
INSERT INTO Admins (Email, Name, Password, Username) VALUES 
('admin@store.com', N'Admin Kullanıcı', '123456', 'admin'),
('sezgin@store.com', N'Sezgin Öztürk', 'sezgin123', 'sezgin');
GO

-- Customers
INSERT INTO Customers (Name, Address, City, CreatedDate, Email, Phone) VALUES 
(N'Mert Yılmaz', N'Kadıköy Moda Caddesi No:4', N'İstanbul', '2025-01-15 00:00:00', 'mert@mail.com', '05551112233'),
(N'Elif Şahin', N'Çankaya Atatürk Bulvarı No:120', N'Ankara', '2025-02-10 00:00:00', 'elif@mail.com', '05554445566'),
(N'Caner Demiroğ', N'Karşıyaka 1712. Sokak No:8', N'İzmir', '2025-03-01 00:00:00', 'caner@mail.com', '05557778899');
GO

-- Employees
INSERT INTO Employees (Name, Phone, Position, Salary) VALUES 
(N'Hakan Çelik', '05321112233', N'Mağaza Müdürü', 45000.00),
(N'Selin Yıldız', '05324445566', N'Satış Danışmanı', 28000.00),
(N'Murat Aydın', '05327778899', N'Depo Görevlisi', 26000.00);
GO

-- Stores
INSERT INTO Stores (StoreName, City, EmployeeCapacity, Phone) VALUES 
(N'Kadıköy Merkez Mağazası', N'İstanbul', 15, '02161112233'),
(N'Çankaya Şubesi', N'Ankara', 10, '03121112233'),
(N'Alsancak Şubesi', N'İzmir', 8, '02321112233');
GO

-- Users
INSERT INTO Users (Email, Name, Password, Username) VALUES 
('user@store.com', N'Standart Kullanıcı', 'user123', 'user'),
('ahmet@store.com', N'Ahmet Yılmaz', 'ahmet123', 'ahmet');
GO
