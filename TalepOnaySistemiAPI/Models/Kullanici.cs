namespace TalepOnaySistemiAPI.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        public int? MusteriId { get; set; }

        public string AdSoyad { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string SifreHash { get; set; } = string.Empty;

        public int RolId { get; set; }

        public bool AktifMi { get; set; }

        public DateTime? SonGirisTarihi { get; set; }

        public DateTime KayitTarihi { get; set; }

        // Navigasyon property'leri
        public Rol? Rol { get; set; }
        public Musteri? Musteri { get; set; }
    }
}