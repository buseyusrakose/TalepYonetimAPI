namespace TalepOnaySistemiAPI.Models
{
    public class Talep
    {
        public int Id { get; set; }

        public string TalepNo { get; set; } = string.Empty;

        public int MusteriId { get; set; }

        public int OlusturanKullaniciId { get; set; }

        public int TalepTipiId { get; set; }

        public int DurumId { get; set; }

        public string Baslik { get; set; } = string.Empty;

        public string Aciklama { get; set; } = string.Empty;

        public string Oncelik { get; set; } = "Normal";

        public DateTime TalepTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }

        public int? GuncelleyenKullaniciId { get; set; }
        public string? SonIslemAciklama { get; set; }

        // Navigasyon property'leri
        public Musteri? Musteri { get; set; }
        public Kullanici? OlusturanKullanici { get; set; }
        public TalepTipi? TalepTipi { get; set; }
        public TalepDurumu? Durum { get; set; }
    }
}