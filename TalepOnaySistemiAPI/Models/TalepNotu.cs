namespace TalepOnaySistemiAPI.Models
{
    public class TalepNotu
    {
        public int Id { get; set; }

        public int TalepId { get; set; }

        public int KullaniciId { get; set; }

        public string NotMetni { get; set; } = string.Empty;

        public DateTime Tarih { get; set; }

        public Talep? Talep { get; set; }
        public Kullanici? Kullanici { get; set; }
    }
}