CREATE PROCEDURE SP_EnAktifMusteriler
AS
BEGIN
    SELECT TOP 5
        m.FirmaAdi,
        COUNT(t.Id) AS TalepSayisi
    FROM Musteriler m
    INNER JOIN Talepler t ON t.MusteriId = m.Id
    GROUP BY m.FirmaAdi
    ORDER BY TalepSayisi DESC;
END;
GO