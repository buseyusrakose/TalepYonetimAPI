using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalepOnaySistemiAPI.Data;
using TalepOnaySistemiAPI.DTOs;
using TalepOnaySistemiAPI.Models;

namespace TalepOnaySistemiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TalepController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TalepController(AppDbContext context)
        {
            _context = context;
        }

        // POST /api/Talep — sadece Müşteri talep oluşturabilir
        [Authorize(Roles = "Musteri")]
        [HttpPost]
        public async Task<IActionResult> Olustur(TalepOlusturDto dto)
        {
            var musteriIdStr = User.FindFirst("MusteriId")?.Value;
            var kullaniciIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (musteriIdStr == null || kullaniciIdStr == null)
                return Unauthorized("Token içinde müşteri bilgisi bulunamadı.");

            int musteriId = int.Parse(musteriIdStr);
            int kullaniciId = int.Parse(kullaniciIdStr);

            var beklemedeDurumu = await _context.TalepDurumlari
                .FirstOrDefaultAsync(d => d.Ad == "Beklemede");

            if (beklemedeDurumu == null)
                return BadRequest("Sistemde 'Beklemede' durumu tanımlı değil.");

            int mevcutTalepSayisi = await _context.Talepler.CountAsync();
            string talepNo = $"TLP-{DateTime.UtcNow.Year}-{(mevcutTalepSayisi + 1):D4}";

            var talep = new Talep
            {
                TalepNo = talepNo,
                MusteriId = musteriId,
                OlusturanKullaniciId = kullaniciId,
                TalepTipiId = dto.TalepTipiId,
                DurumId = beklemedeDurumu.Id,
                Baslik = dto.Baslik,
                Aciklama = dto.Aciklama,
                Oncelik = dto.Oncelik,
                TalepTarihi = DateTime.UtcNow
            };

            _context.Talepler.Add(talep);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Talep oluşturuldu", talep.TalepNo });
        }

        // GET /api/Talep — Müşteri sadece kendi taleplerini, Yetkili/Yönetici hepsini görür
        [HttpGet]
        public async Task<IActionResult> Listele()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            var sorgu = _context.Talepler
                .Include(t => t.Durum)
                .Include(t => t.TalepTipi)
                .Include(t => t.Musteri)
                .AsQueryable();

            if (rol == "Musteri")
            {
                var musteriIdStr = User.FindFirst("MusteriId")?.Value;
                if (musteriIdStr == null)
                    return Unauthorized("Token içinde müşteri bilgisi bulunamadı.");

                int musteriId = int.Parse(musteriIdStr);
                sorgu = sorgu.Where(t => t.MusteriId == musteriId);
            }

            var sonuc = await sorgu
                .OrderByDescending(t => t.TalepTarihi)
                .Select(t => new TalepListeDto
                {
                    Id = t.Id,
                    TalepNo = t.TalepNo,
                    Baslik = t.Baslik,
                    DurumAdi = t.Durum!.Ad,
                    TalepTipiAdi = t.TalepTipi!.Ad,
                    Oncelik = t.Oncelik,
                    TalepTarihi = t.TalepTarihi,
                    FirmaAdi = t.Musteri!.FirmaAdi
                })
                .ToListAsync();

            return Ok(sonuc);
        }

        // GET /api/Talep/{id} — detay, Müşteri sadece kendi talebini görebilir
        [HttpGet("{id}")]
        public async Task<IActionResult> Detay(int id)
        {
            var talep = await _context.Talepler
                .Include(t => t.Durum)
                .Include(t => t.TalepTipi)
                .Include(t => t.Musteri)
                .Include(t => t.OlusturanKullanici)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (talep == null)
                return NotFound("Talep bulunamadı.");

            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (rol == "Musteri")
            {
                var musteriIdStr = User.FindFirst("MusteriId")?.Value;
                if (musteriIdStr == null || talep.MusteriId != int.Parse(musteriIdStr))
                    return Forbid();
            }

            return Ok(talep);
        }

        // PUT /api/Talep/{id}/onayla — sadece Yetkili ve Yönetici çağırabilir
        [Authorize(Roles = "Yetkili,Yonetici")]
        [HttpPut("{id}/onayla")]
        public async Task<IActionResult> Onayla(int id)
        {
            var talep = await _context.Talepler.FirstOrDefaultAsync(t => t.Id == id);

            if (talep == null)
                return NotFound("Talep bulunamadı.");

            var onaylandiDurumu = await _context.TalepDurumlari
                .FirstOrDefaultAsync(d => d.Ad == "Onaylandi");
            var reddedildiDurumu = await _context.TalepDurumlari
                .FirstOrDefaultAsync(d => d.Ad == "Reddedildi");

            if (onaylandiDurumu == null || reddedildiDurumu == null)
                return BadRequest("Sistemde gerekli durum tanımları eksik.");

            if (talep.DurumId == onaylandiDurumu.Id || talep.DurumId == reddedildiDurumu.Id)
                return BadRequest("Bu talep zaten sonuçlandırılmış, tekrar işlem yapılamaz.");

            var kullaniciIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int kullaniciId = int.Parse(kullaniciIdStr!);

            // Not: TalepGecmisi kaydı artık SQL trigger (TR_Talepler_DurumDegisti) tarafından
            // otomatik olarak atılıyor, burada manuel ekleme yapmıyoruz.
            talep.DurumId = onaylandiDurumu.Id;
            talep.GuncellenmeTarihi = DateTime.UtcNow;
            talep.GuncelleyenKullaniciId = kullaniciId;
            talep.SonIslemAciklama = "Talep onaylandı.";

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Talep onaylandı.", talep.TalepNo });
        }

        // PUT /api/Talep/{id}/reddet — sadece Yetkili ve Yönetici çağırabilir
        [Authorize(Roles = "Yetkili,Yonetici")]
        [HttpPut("{id}/reddet")]
        public async Task<IActionResult> Reddet(int id, [FromBody] ReddetDto dto)
        {
            var talep = await _context.Talepler.FirstOrDefaultAsync(t => t.Id == id);

            if (talep == null)
                return NotFound("Talep bulunamadı.");

            var onaylandiDurumu = await _context.TalepDurumlari
                .FirstOrDefaultAsync(d => d.Ad == "Onaylandi");
            var reddedildiDurumu = await _context.TalepDurumlari
                .FirstOrDefaultAsync(d => d.Ad == "Reddedildi");

            if (onaylandiDurumu == null || reddedildiDurumu == null)
                return BadRequest("Sistemde gerekli durum tanımları eksik.");

            if (talep.DurumId == onaylandiDurumu.Id || talep.DurumId == reddedildiDurumu.Id)
                return BadRequest("Bu talep zaten sonuçlandırılmış, tekrar işlem yapılamaz.");

            var kullaniciIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int kullaniciId = int.Parse(kullaniciIdStr!);

            // Not: TalepGecmisi kaydı artık SQL trigger (TR_Talepler_DurumDegisti) tarafından
            // otomatik olarak atılıyor, burada manuel ekleme yapmıyoruz.
            talep.DurumId = reddedildiDurumu.Id;
            talep.GuncellenmeTarihi = DateTime.UtcNow;
            talep.GuncelleyenKullaniciId = kullaniciId;
            talep.SonIslemAciklama = dto.Sebep;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Talep reddedildi.", talep.TalepNo });
        }

        // POST /api/Talep/{id}/not-ekle — Yetkili ve Yönetici not ekleyebilir
        [Authorize(Roles = "Yetkili,Yonetici")]
        [HttpPost("{id}/not-ekle")]
        public async Task<IActionResult> NotEkle(int id, [FromBody] NotEkleDto dto)
        {
            var talep = await _context.Talepler.FirstOrDefaultAsync(t => t.Id == id);

            if (talep == null)
                return NotFound("Talep bulunamadı.");

            var kullaniciIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int kullaniciId = int.Parse(kullaniciIdStr!);

            var not = new TalepNotu
            {
                TalepId = id,
                KullaniciId = kullaniciId,
                NotMetni = dto.NotMetni,
                Tarih = DateTime.UtcNow
            };

            _context.TalepNotlari.Add(not);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Not eklendi." });
        }

        // GET /api/Talep/{id}/notlar — bir talebin tüm notlarını listeler
        [HttpGet("{id}/notlar")]
        public async Task<IActionResult> NotlariListele(int id)
        {
            var talep = await _context.Talepler.FirstOrDefaultAsync(t => t.Id == id);

            if (talep == null)
                return NotFound("Talep bulunamadı.");

            // Müşteri sadece kendi talebinin notlarını görebilir
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            if (rol == "Musteri")
            {
                var musteriIdStr = User.FindFirst("MusteriId")?.Value;
                if (musteriIdStr == null || talep.MusteriId != int.Parse(musteriIdStr))
                    return Forbid();
            }

            var notlar = await _context.TalepNotlari
                .Include(n => n.Kullanici)
                .Where(n => n.TalepId == id)
                .OrderByDescending(n => n.Tarih)
                .Select(n => new
                {
                    n.Id,
                    n.NotMetni,
                    n.Tarih,
                    EkleyenKullanici = n.Kullanici!.AdSoyad
                })
                .ToListAsync();

            return Ok(notlar);
        }
    }
}