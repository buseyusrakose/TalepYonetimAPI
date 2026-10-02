CREATE VIEW VW_TalepDetay
AS
SELECT 
    t.Id,
    t.TalepNo,
    t.Baslik,
    t.Aciklama,
    t.Oncelik,
    t.TalepTarihi,
    t.GuncellenmeTarihi,
    m.FirmaAdi,
    m.Email AS MusteriEmail,
    tt.Ad AS TalepTipiAdi,
    td.Ad AS DurumAdi
FROM Talepler t
INNER JOIN Musteriler m ON t.MusteriId = m.Id
INNER JOIN TalepTipleri tt ON t.TalepTipiId = tt.Id
INNER JOIN TalepDurumlari td ON t.DurumId = td.Id;