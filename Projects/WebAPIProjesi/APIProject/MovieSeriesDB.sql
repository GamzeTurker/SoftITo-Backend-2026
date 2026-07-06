-- MovieSeriesAPI Veritabanı ve Tablo Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MovieSeriesAPI')
BEGIN
    ALTER DATABASE MovieSeriesAPI SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MovieSeriesAPI;
END
GO

CREATE DATABASE MovieSeriesAPI;
GO

USE MovieSeriesAPI;
GO

-- 1. Adminler Tablosu
CREATE TABLE Adminler (
    AminId INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciAdi NVARCHAR(MAX) NOT NULL,
    Sifre NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(MAX) NOT NULL
);
GO

-- 2. CizgiFilmler Tablosu
CREATE TABLE CizgiFilmler (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(MAX) NOT NULL,
    Tur NVARCHAR(MAX) NOT NULL,
    BolumSayisi INT NOT NULL,
    YasAraligi NVARCHAR(MAX) NOT NULL
);
GO

-- 3. Diziler Tablosu
CREATE TABLE Diziler (
    DiziId INT IDENTITY(1,1) PRIMARY KEY,
    DiziAd NVARCHAR(MAX) NOT NULL,
    Tur NVARCHAR(MAX) NOT NULL,
    BolumSayisi INT NOT NULL,
    YapimYili INT NOT NULL
);
GO

-- 4. Filmler Tablosu
CREATE TABLE Filmler (
    FilmId INT IDENTITY(1,1) PRIMARY KEY,
    FilmAd NVARCHAR(MAX) NOT NULL,
    Tur NVARCHAR(MAX) NOT NULL,
    Sure INT NOT NULL,
    YapimYili INT NOT NULL
);
GO


-- ========================================================
-- TEST VERİLERİ (SEED DATA) - TÜRKÇE KARAKTERLERLE
-- ========================================================

-- Adminler
INSERT INTO Adminler (KullaniciAdi, Sifre, Email) VALUES 
('admin', '123456', 'admin@movieseries.com'),
('sezgin', 'sezgin123', 'sezgin@mail.com');
GO

-- CizgiFilmler
INSERT INTO CizgiFilmler (Ad, Tur, BolumSayisi, YasAraligi) VALUES 
(N'Keloğlan Masalları', N'Macera, Fantastik', 120, '4-10'),
(N'Kral Şakir', N'Komedi, Macera', 200, '6-12'),
(N'Rafadan Tayfa', N'Eğitici, Kültür', 150, '5-11'),
(N'Pepee', N'Müzikal, Eğitici', 80, '2-6'),
(N'Niloya', N'Müzikal, Eğitici', 95, '3-7');
GO

-- Diziler
INSERT INTO Diziler (DiziAd, Tur, BolumSayisi, YapimYili) VALUES 
(N'Kurtlar Vadisi', N'Aksiyon, Dram, Politik', 97, 2003),
(N'Ezel', N'Dram, Gizem, Suç', 71, 2009),
(N'Leyla ile Mecnun', N'Absürd Komedi, Dram', 104, 2011),
(N'Şahsiyet', N'Polisiye, Gerilim, Gizem', 12, 2018),
(N'Muhteşem Yüzyıl', N'Tarihi, Dram', 139, 2011);
GO

-- Filmler
INSERT INTO Filmler (FilmAd, Tur, Sure, YapimYili) VALUES 
(N'Babam ve Oğlum', N'Dram, Aile', 112, 2005),
(N'G.O.R.A.', N'Bilim Kurgu, Komedi', 127, 2004),
(N'Ayla', N'Biyografi, Dram, Savaş', 125, 2017),
(N'Kış Uykusu', N'Dram, Sanat', 196, 2014),
(N'Dağ II', N'Aksiyon, Savaş, Dram', 135, 2016);
GO
