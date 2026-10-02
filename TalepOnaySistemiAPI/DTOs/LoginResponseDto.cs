namespace TalepOnaySistemiAPI.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public string AdSoyad { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;
    }
}