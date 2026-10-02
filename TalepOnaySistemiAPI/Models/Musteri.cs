namespace TalepOnaySistemiAPI.Models
{
    public class Musteri
    {
        public int Id { get; set; }

        public string FirmaAdi { get; set; } = string.Empty;

        public string? VergiNo { get; set; }

        public string? Telefon { get; set; }

        public string? Email { get; set; }

        public string? Adres { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }
}