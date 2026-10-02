namespace TalepOnaySistemiAPI.DTOs
{
    public class TalepListeDto
    {
        public int Id { get; set; }
        public string TalepNo { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string DurumAdi { get; set; } = string.Empty;
        public string TalepTipiAdi { get; set; } = string.Empty;
        public string Oncelik { get; set; } = string.Empty;
        public DateTime TalepTarihi { get; set; }
        public string? FirmaAdi { get; set; } // Yetkili/Yönetici için, Müşteri için gerekmez ama zararı yok
    }
}