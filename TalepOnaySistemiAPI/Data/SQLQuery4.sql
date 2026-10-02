CREATE TRIGGER TR_Talepler_DurumDegisti
ON Talepler
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(DurumId)
    BEGIN
        INSERT INTO TalepGecmisi (TalepId, KullaniciId, Islem, Aciklama, Tarih)
        SELECT 
            i.Id,
            i.GuncelleyenKullaniciId,
            td.Ad,
            i.SonIslemAciklama,
            GETDATE()
        FROM inserted i
        INNER JOIN deleted d ON i.Id = d.Id
        INNER JOIN TalepDurumlari td ON td.Id = i.DurumId
        WHERE i.DurumId <> d.DurumId
          AND i.GuncelleyenKullaniciId IS NOT NULL;
    END
END;
GO