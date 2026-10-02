namespace TalepOnaySistemiAPI.Models
{
    public class TalepGecmisi
    {
        public int Id { get; set; }

        public int TalepId { get; set; }

        public int KullaniciId { get; set; }

        public string Islem { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public DateTime Tarih { get; set; }
    }
}