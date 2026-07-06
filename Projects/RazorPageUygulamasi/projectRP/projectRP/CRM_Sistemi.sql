-- CRM_Sistemi Veritabanı ve Tablo Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'CRM_Sistemi')
BEGIN
    ALTER DATABASE CRM_Sistemi SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CRM_Sistemi;
END
GO

CREATE DATABASE CRM_Sistemi;
GO

USE CRM_Sistemi;
GO

-- 1. Yoneticiler Tablosu
CREATE TABLE Yoneticiler (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad NVARCHAR(100) NOT NULL,
    Eposta NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20) NOT NULL
);
GO

-- 2. Calisanlar Tablosu
CREATE TABLE Calisanlar (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad NVARCHAR(100) NOT NULL,
    Departman NVARCHAR(100) NOT NULL,
    Pozisyon NVARCHAR(100) NOT NULL,
    Maas DECIMAL(18,2) NOT NULL,
    Telefon NVARCHAR(20) NOT NULL
);
GO

-- 3. Musteriler Tablosu
CREATE TABLE Musteriler (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad NVARCHAR(100) NOT NULL,
    Eposta NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20) NOT NULL,
    Sehir NVARCHAR(50) NOT NULL,
    Notlar NVARCHAR(255) NULL
);
GO


-- ========================================================
-- TEST VERİLERİ (SEED DATA) - TÜRKÇE KARAKTERLERLE
-- ========================================================

-- Yoneticiler
INSERT INTO Yoneticiler (AdSoyad, Eposta, Telefon) VALUES 
(N'Hakan Çelik', 'hakan@crm.com', '05321112233'),
(N'Selin Yıldız', 'selin@crm.com', '05324445566');
GO

-- Calisanlar
INSERT INTO Calisanlar (AdSoyad, Departman, Pozisyon, Maas, Telefon) VALUES 
(N'Mert Yılmaz', N'Satış & Pazarlama', N'Satış Temsilcisi', 32000.00, '05551112233'),
(N'Elif Şahin', N'İnsan Kaynakları', N'IK Uzmanı', 35000.00, '05554445566'),
(N'Murat Aydın', N'Teknoloji', N'Sistem Yöneticisi', 48000.00, '05557778899');
GO

-- Musteriler
INSERT INTO Musteriler (AdSoyad, Eposta, Telefon, Sehir, Notlar) VALUES 
(N'Caner Demiroğ', 'caner@mail.com', '05051112233', N'İstanbul', N'Kurumsal büyük müşteri, VIP statüsünde.'),
(N'Ayşe Kaya', 'ayse@mail.com', '05054445566', N'Ankara', N'Yeni üye, eğitim sektörü temsilcisi.'),
(N'Onur Öztürk', 'onur@mail.com', '05057778899', N'İzmir', N'Eski müşteri, düzenli ödeme yapıyor.');
GO
