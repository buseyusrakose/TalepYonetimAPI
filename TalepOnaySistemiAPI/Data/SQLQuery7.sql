CREATE PROCEDURE SP_AylikTalepSayisi
AS
BEGIN
    SELECT 
        YEAR(TalepTarihi) AS Yil,
        MONTH(TalepTarihi) AS Ay,
        COUNT(*) AS TalepSayisi
    FROM Talepler
    GROUP BY YEAR(TalepTarihi), MONTH(TalepTarihi)
    ORDER BY Yil, Ay;
END;
GO