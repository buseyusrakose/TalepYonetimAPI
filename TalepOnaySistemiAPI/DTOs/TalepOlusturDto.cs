using System.ComponentModel.DataAnnotations;

namespace TalepOnaySistemiAPI.DTOs
{
    public class TalepOlusturDto
    {
        [Required(ErrorMessage = "Talep tipi seçilmelidir.")]
        public int TalepTipiId { get; set; }

        [Required(ErrorMessage = "Başlık boş bırakılamaz.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Başlık 5-250 karakter arasında olmalıdır.")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Açıklama en az 10 karakter olmalıdır.")]
        public string Aciklama { get; set; } = string.Empty;

        public string Oncelik { get; set; } = "Normal";
    }
}