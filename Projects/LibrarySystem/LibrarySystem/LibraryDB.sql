-- LibraryDB Veritabanı, Tablo ve Stored Procedure Kurulum Script'i
USE master;
GO

-- Eğer veritabanı varsa sil ve yeniden oluştur
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'LibraryDB')
BEGIN
    ALTER DATABASE LibraryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryDB;
END
GO

CREATE DATABASE LibraryDB;
GO

USE LibraryDB;
GO

-- ========================================================
-- 1. TABLOLARIN OLUŞTURULMASI
-- ========================================================

-- Admins Tablosu
CREATE TABLE Admins (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Name NVARCHAR(100) NOT NULL
);
GO

-- Yazarlar Tablosu
CREATE TABLE Yazarlar (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad NVARCHAR(100) NOT NULL,
    Biyografi NVARCHAR(500) NULL,
    DogumTarihi DATETIME2 NULL
);
GO

-- Kitaplar Tablosu
CREATE TABLE Kitaplar (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    YazarId INT NOT NULL,
    Baslik NVARCHAR(150) NOT NULL,
    ISBN NVARCHAR(20) NOT NULL,
    BasimYili INT NOT NULL,
    SayfaSayisi INT NOT NULL,
    FOREIGN KEY (YazarId) REFERENCES Yazarlar(Id) ON DELETE CASCADE
);
GO

-- Uyeler Tablosu
CREATE TABLE Uyeler (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad NVARCHAR(100) NOT NULL,
    Eposta NVARCHAR(100) NOT NULL UNIQUE,
    Telefon NVARCHAR(20) NOT NULL,
    KayitTarihi DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Emanetler (Kitap Alışverişleri) Tablosu
CREATE TABLE Emanetler (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KitapId INT NOT NULL,
    UyeId INT NOT NULL,
    EmanetTarihi DATETIME2 NOT NULL DEFAULT GETDATE(),
    TeslimTarihi DATETIME2 NULL,
    Durum NVARCHAR(50) NOT NULL, -- 'Emanette', 'Teslim Edildi', 'Gecikmiş'
    FOREIGN KEY (KitapId) REFERENCES Kitaplar(Id) ON DELETE CASCADE,
    FOREIGN KEY (UyeId) REFERENCES Uyeler(Id) ON DELETE CASCADE
);
GO


-- ========================================================
-- 2. STORED PROCEDURE'LARIN OLUŞTURULMASI
-- ========================================================

-- Admin Doğrulama SP
CREATE PROCEDURE sp_ValidateAdmin
    @Username NVARCHAR(50),
    @Password NVARCHAR(100)
AS
BEGIN
    SELECT Id, Username, Name FROM Admins WHERE Username = @Username AND Password = @Password;
END
GO

-- Yazarlar SP
CREATE PROCEDURE sp_GetYazarlar
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    IF @SearchTerm IS NULL OR @SearchTerm = ''
        SELECT * FROM Yazarlar ORDER BY AdSoyad;
    ELSE
        SELECT * FROM Yazarlar 
        WHERE AdSoyad LIKE '%' + @SearchTerm + '%' OR Biyografi LIKE '%' + @SearchTerm + '%'
        ORDER BY AdSoyad;
END
GO

CREATE PROCEDURE sp_GetYazarById
    @Id INT
AS
BEGIN
    SELECT * FROM Yazarlar WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_InsertYazar
    @AdSoyad NVARCHAR(100),
    @Biyografi NVARCHAR(500),
    @DogumTarihi DATETIME2
AS
BEGIN
    INSERT INTO Yazarlar (AdSoyad, Biyografi, DogumTarihi) 
    VALUES (@AdSoyad, @Biyografi, @DogumTarihi);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_UpdateYazar
    @Id INT,
    @AdSoyad NVARCHAR(100),
    @Biyografi NVARCHAR(500),
    @DogumTarihi DATETIME2
AS
BEGIN
    UPDATE Yazarlar 
    SET AdSoyad = @AdSoyad, Biyografi = @Biyografi, DogumTarihi = @DogumTarihi 
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteYazar
    @Id INT
AS
BEGIN
    DELETE FROM Yazarlar WHERE Id = @Id;
END
GO

-- Kitaplar SP
CREATE PROCEDURE sp_GetKitaplar
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    IF @SearchTerm IS NULL OR @SearchTerm = ''
        SELECT k.*, y.AdSoyad AS YazarAdSoyad 
        FROM Kitaplar k
        INNER JOIN Yazarlar y ON k.YazarId = y.Id
        ORDER BY k.Baslik;
    ELSE
        SELECT k.*, y.AdSoyad AS YazarAdSoyad 
        FROM Kitaplar k
        INNER JOIN Yazarlar y ON k.YazarId = y.Id
        WHERE k.Baslik LIKE '%' + @SearchTerm + '%' OR k.ISBN LIKE '%' + @SearchTerm + '%' OR y.AdSoyad LIKE '%' + @SearchTerm + '%'
        ORDER BY k.Baslik;
END
GO

CREATE PROCEDURE sp_GetKitapById
    @Id INT
AS
BEGIN
    SELECT * FROM Kitaplar WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_InsertKitap
    @YazarId INT,
    @Baslik NVARCHAR(150),
    @ISBN NVARCHAR(20),
    @BasimYili INT,
    @SayfaSayisi INT
AS
BEGIN
    INSERT INTO Kitaplar (YazarId, Baslik, ISBN, BasimYili, SayfaSayisi)
    VALUES (@YazarId, @Baslik, @ISBN, @BasimYili, @SayfaSayisi);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_UpdateKitap
    @Id INT,
    @YazarId INT,
    @Baslik NVARCHAR(150),
    @ISBN NVARCHAR(20),
    @BasimYili INT,
    @SayfaSayisi INT
AS
BEGIN
    UPDATE Kitaplar 
    SET YazarId = @YazarId, Baslik = @Baslik, ISBN = @ISBN, BasimYili = @BasimYili, SayfaSayisi = @SayfaSayisi
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteKitap
    @Id INT
AS
BEGIN
    DELETE FROM Kitaplar WHERE Id = @Id;
END
GO

-- Üyeler SP
CREATE PROCEDURE sp_GetUyeler
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    IF @SearchTerm IS NULL OR @SearchTerm = ''
        SELECT * FROM Uyeler ORDER BY AdSoyad;
    ELSE
        SELECT * FROM Uyeler 
        WHERE AdSoyad LIKE '%' + @SearchTerm + '%' OR Eposta LIKE '%' + @SearchTerm + '%' OR Telefon LIKE '%' + @SearchTerm + '%'
        ORDER BY AdSoyad;
END
GO

CREATE PROCEDURE sp_GetUyeById
    @Id INT
AS
BEGIN
    SELECT * FROM Uyeler WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_InsertUye
    @AdSoyad NVARCHAR(100),
    @Eposta NVARCHAR(100),
    @Telefon NVARCHAR(20),
    @KayitTarihi DATETIME2
AS
BEGIN
    INSERT INTO Uyeler (AdSoyad, Eposta, Telefon, KayitTarihi)
    VALUES (@AdSoyad, @Eposta, @Telefon, @KayitTarihi);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_UpdateUye
    @Id INT,
    @AdSoyad NVARCHAR(100),
    @Eposta NVARCHAR(100),
    @Telefon NVARCHAR(20),
    @KayitTarihi DATETIME2
AS
BEGIN
    UPDATE Uyeler 
    SET AdSoyad = @AdSoyad, Eposta = @Eposta, Telefon = @Telefon, KayitTarihi = @KayitTarihi
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteUye
    @Id INT
AS
BEGIN
    DELETE FROM Uyeler WHERE Id = @Id;
END
GO

-- Emanetler SP
CREATE PROCEDURE sp_GetEmanetler
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    IF @SearchTerm IS NULL OR @SearchTerm = ''
        SELECT e.*, k.Baslik AS KitapBaslik, u.AdSoyad AS UyeAdSoyad 
        FROM Emanetler e
        INNER JOIN Kitaplar k ON e.KitapId = k.Id
        INNER JOIN Uyeler u ON e.UyeId = u.Id
        ORDER BY e.EmanetTarihi DESC;
    ELSE
        SELECT e.*, k.Baslik AS KitapBaslik, u.AdSoyad AS UyeAdSoyad 
        FROM Emanetler e
        INNER JOIN Kitaplar k ON e.KitapId = k.Id
        INNER JOIN Uyeler u ON e.UyeId = u.Id
        WHERE k.Baslik LIKE '%' + @SearchTerm + '%' OR u.AdSoyad LIKE '%' + @SearchTerm + '%' OR e.Durum LIKE '%' + @SearchTerm + '%'
        ORDER BY e.EmanetTarihi DESC;
END
GO

CREATE PROCEDURE sp_GetEmanetById
    @Id INT
AS
BEGIN
    SELECT * FROM Emanetler WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_InsertEmanet
    @KitapId INT,
    @UyeId INT,
    @EmanetTarihi DATETIME2,
    @TeslimTarihi DATETIME2 = NULL,
    @Durum NVARCHAR(50)
AS
BEGIN
    INSERT INTO Emanetler (KitapId, UyeId, EmanetTarihi, TeslimTarihi, Durum)
    VALUES (@KitapId, @UyeId, @EmanetTarihi, @TeslimTarihi, @Durum);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_UpdateEmanet
    @Id INT,
    @KitapId INT,
    @UyeId INT,
    @EmanetTarihi DATETIME2,
    @TeslimTarihi DATETIME2 = NULL,
    @Durum NVARCHAR(50)
AS
BEGIN
    UPDATE Emanetler 
    SET KitapId = @KitapId, UyeId = @UyeId, EmanetTarihi = @EmanetTarihi, TeslimTarihi = @TeslimTarihi, Durum = @Durum
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteEmanet
    @Id INT
AS
BEGIN
    DELETE FROM Emanetler WHERE Id = @Id;
END
GO


-- ========================================================
-- 3. SEED DATA (TÜRKÇE KARAKTERLERLE)
-- ========================================================

-- Admins
INSERT INTO Admins (Username, Password, Name) VALUES 
('admin', 'admin123', N'Yönetici Sezgin'),
('sezgin', 'sezgin123', N'Sezgin Öztürk');
GO

-- Yazarlar
INSERT INTO Yazarlar (AdSoyad, Biyografi, DogumTarihi) VALUES 
(N'Ömer Seyfettin', N'Türk hikayeciliğinin öncülerinden olan yazar, realist akımın temsilcisidir.', '1884-03-11'),
(N'Sabahattin Ali', N'Cumhuriyet dönemi Türk edebiyatının en önemli romancı ve öykücülerindendir.', '1907-02-25'),
(N'Reşat Nuri Güntekin', N'Çalıkuşu, Dudaktan Kalbe gibi ölümsüz eserlerin sahibi büyük romancı.', '1889-11-25');
GO

-- Kitaplar
INSERT INTO Kitaplar (YazarId, Baslik, ISBN, BasimYili, SayfaSayisi) VALUES 
(1, N'Forsa ve Falaka Hikayeleri', '978-975-11-1234-5', 2021, 120),
(2, N'Kürk Mantolu Madonna', '978-975-363-802-9', 2018, 160),
(2, N'Kuyucaklı Yusuf', '978-975-363-801-2', 2019, 220),
(3, N'Çalıkuşu', '978-975-10-0255-6', 2020, 400);
GO

-- Uyeler
INSERT INTO Uyeler (AdSoyad, Eposta, Telefon, KayitTarihi) VALUES 
(N'Mert Yılmaz', 'mert@mail.com', '05551112233', '2025-01-10'),
(N'Elif Şahin', 'elif@mail.com', '05554445566', '2025-02-15'),
(N'Hakan Çelik', 'hakan@mail.com', '05557778899', '2025-03-01');
GO

-- Emanetler
INSERT INTO Emanetler (KitapId, UyeId, EmanetTarihi, TeslimTarihi, Durum) VALUES 
(2, 1, '2025-06-01', NULL, N'Emanette'),
(4, 2, '2025-05-10', '2025-05-25', N'Teslim Edildi'),
(1, 3, '2025-04-01', NULL, N'Gecikmiş');
GO
