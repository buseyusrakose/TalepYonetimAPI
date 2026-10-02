using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalepOnaySistemiAPI.Data;
using TalepOnaySistemiAPI.DTOs;
using TalepOnaySistemiAPI.Models;

namespace TalepOnaySistemiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GEÇİCİ ENDPOINT — artık kullanılmıyor, bcrypt hash'leri zaten üretildi
        // [HttpGet("hashle")]
        // public IActionResult HashUret(string sifre)
        // {
        //     var hash = BCrypt.Net.BCrypt.HashPassword(sifre);
        //     return Ok(hash);
        // }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var kullanici = await _context.Kullanicilar
                .Include(k => k.Rol)
                .FirstOrDefaultAsync(k => k.Email == request.Email);

            if (kullanici == null || !kullanici.AktifMi)
                return Unauthorized("Kullanıcı adı veya şifre hatalı");

            bool sifreDogruMu = BCrypt.Net.BCrypt.Verify(request.Password, kullanici.SifreHash);

            if (!sifreDogruMu)
                return Unauthorized("Kullanıcı adı veya şifre hatalı");

            var token = TokenUret(kullanici);

            var response = new LoginResponseDto
            {
                Token = token,
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol?.RolAdi ?? ""
            };

            return Ok(response);
        }

        private string TokenUret(Kullanici kullanici)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Email, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol?.RolAdi ?? ""),
            };

            if (kullanici.MusteriId.HasValue)
                claims.Add(new Claim("MusteriId", kullanici.MusteriId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}