namespace TalepOnaySistemiAPI.DTOs
{
    public class TalepDetayViewDto
    {
        public int Id { get; set; }
        public string TalepNo { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Oncelik { get; set; } = string.Empty;
        public DateTime TalepTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public string FirmaAdi { get; set; } = string.Empty;
        public string? MusteriEmail { get; set; }
        public string TalepTipiAdi { get; set; } = string.Empty;
        public string DurumAdi { get; set; } = string.Empty;
    }
}