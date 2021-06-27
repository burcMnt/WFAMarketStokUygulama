USE master;
GO

DROP DATABASE KuzeyYeli1;
CREATE DATABASE KuzeyYeli1;
GO

USE KuzeyYeli1;
GO

CREATE TABLE Kategoriler
(
	Id INT PRIMARY KEY IDENTITY,
	KategoriAd NVARCHAR(50) NOT NULL,
	UstKategoriId INT FOREIGN KEY REFERENCES Kategoriler(Id) NULL
);
GO

CREATE TABLE Urunler
(
	Id INT PRIMARY KEY IDENTITY,
	KategoriId INT FOREIGN KEY REFERENCES Kategoriler(Id) ON DELETE CASCADE NOT NULL,
	UrunAd NVARCHAR(50) NOT NULL,
	BirimFiyat DECIMAL(18, 2) NOT NULL,
	StokAdet INT NOT NULL,
	Resim VARBINARY(MAX) NULL
);
GO

INSERT INTO Kategoriler(KategoriAd, UstKategoriId) VALUES
(N'İçecekler', NULL), (N'Alkolsüz İçecekler',1), (N'Alkollü İçecekler', 1), (N'Süt ve Süt Ürünleri', NULL), (N'Peynir', 4), (N'Yoğurt', 4), 
(N'Meyve/Sebze', NULL), (N'Temizlik Ürünleri', NULL), (N'Genel Temizlik Ürünleri', 8), (N'Kişisel Bakım',8),
(N'Elektronik', NULL), (N'Cep Telefonu', 11), (N'Bilgisayar', 11), (N'Televizyon',11), (N'Gıda Dışı ',NULL), (N'Kedi Maması',15), (N'Gıda Dışılar',NULL),
(N'Kişisel Bakım',NULL);

INSERT INTO Urunler(KategoriId, UrunAd, BirimFiyat, StokAdet) VALUES
(2, N'Kola', 4, 20),(2, N'Fanta', 4, 40),(2, N'Redbull', 6, 0),(3, N'Viski', 25, 10),
(4, N'Süt', 4.5, 50),(5, N'Kaşar Peynir', 25, 10),(5, N'Beyaz Peynir', 22, 10),(6, N'Sütaş Yoğurt', 5, 10),(6, N'İçim Yoğurt', 5, 15),
(7, N'Elma', 3.95, 60),(7, N'Ananas', 10, 0),(7, N'Domates', 25, 10),
(9, N'Çamaşır Suyu', 8.95, 15),(9, N'Sıvı Sabun', 12.49, 20),(10, N'Erkek Şampuan', 25, 10),(10, N'Deodorant', 25, 10),(10, N'Kadın Şampuan', 25, 10),
(12, N'Samsung Galaxy M31', 3671.33, 5),(12, N'XiaomiRedmi Note 9 Pro', 3769.96, 4);

SELECT * FROM Kategoriler;
SELECT * FROM Urunler;
