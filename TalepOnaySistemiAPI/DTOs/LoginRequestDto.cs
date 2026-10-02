using System.ComponentModel.DataAnnotations;

namespace TalepOnaySistemiAPI.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        public string Password { get; set; } = string.Empty;
    }
}