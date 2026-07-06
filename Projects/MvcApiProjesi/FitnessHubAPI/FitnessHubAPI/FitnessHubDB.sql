-- FitnessHubDB Veritabanı ve Tablo Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'FitnessHubDB')
BEGIN
    ALTER DATABASE FitnessHubDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FitnessHubDB;
END
GO

CREATE DATABASE FitnessHubDB;
GO

USE FitnessHubDB;
GO

-- 1. Equipments Tablosu
CREATE TABLE Equipments (
    EquipmentId INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentName NVARCHAR(150) NOT NULL,
    Brand NVARCHAR(100) NOT NULL,
    PurchaseDate DATETIME2 NULL,
    Quantity INT NOT NULL
);
GO

-- 2. Members Tablosu
CREATE TABLE Members (
    MemberId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Age INT NOT NULL,
    JoinDate DATETIME2 NOT NULL,
    MembershipType NVARCHAR(50) NOT NULL
);
GO

-- 3. Trainers Tablosu
CREATE TABLE Trainers (
    TrainerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Specialty NVARCHAR(100) NOT NULL,
    ExperienceYear INT NOT NULL,
    Salary DECIMAL(18,2) NOT NULL
);
GO


-- ========================================================
-- TEST VERİLERİ (SEED DATA) - TÜRKÇE KARAKTERLERLE
-- ========================================================

-- Equipments
INSERT INTO Equipments (EquipmentName, Brand, PurchaseDate, Quantity) VALUES 
(N'Koşu Bandı Pro', N'ProForm', '2025-01-15 10:00:00', 5),
(N'Ayarlanabilir Dumbbell Seti', N'Bowflex', '2025-02-10 11:30:00', 12),
(N'Kondisyon Bisikleti', N'Schwinn', '2024-11-05 09:15:00', 8),
(N'Profesyonel Ağırlık Sehpası', N'Adidas', '2025-03-20 14:00:00', 6),
(N'Kaydırmaz Egzersiz Matı', N'Nike', '2025-04-01 08:00:00', 25);
GO

-- Members
INSERT INTO Members (FullName, Age, JoinDate, MembershipType) VALUES 
(N'Mert Yılmaz', 28, '2025-01-05 00:00:00', N'Premium'),
(N'Ayşe Kaya', 34, '2025-02-12 00:00:00', N'Standart'),
(N'Can Demir', 22, '2025-03-01 00:00:00', N'VIP'),
(N'Elif Şahin', 30, '2025-01-20 00:00:00', N'Standart'),
(N'Onur Öztürk', 45, '2024-12-15 00:00:00', N'Gold');
GO

-- Trainers
INSERT INTO Trainers (FullName, Specialty, ExperienceYear, Salary) VALUES 
(N'Hakan Çelik', N'Vücut Geliştirme ve Güç', 8, 45000.00),
(N'Selin Yıldız', N'Pilates ve Yoga', 5, 38000.00),
(N'Murat Aydın', N'Kardiyo ve Kondisyon', 12, 55000.00);
GO
