using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalepOnaySistemiAPI.Data;
using TalepOnaySistemiAPI.DTOs;

namespace TalepOnaySistemiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Dashboard/musteri — sadece Müşteri
        [Authorize(Roles = "Musteri")]
        [HttpGet("musteri")]
        public async Task<IActionResult> MusteriDashboard()
        {
            var musteriIdStr = User.FindFirst("MusteriId")?.Value;
            if (musteriIdStr == null)
                return Unauthorized("Token içinde müşteri bilgisi bulunamadı.");

            int musteriId = int.Parse(musteriIdStr);

            var talepler = _context.Talepler
                .Include(t => t.Durum)
                .Where(t => t.MusteriId == musteriId);

            var sonuc = new
            {
                ToplamTalep = await talepler.CountAsync(),
                BekleyenTalep = await talepler.CountAsync(t => t.Durum!.Ad == "Beklemede"),
                OnaylananTalep = await talepler.CountAsync(t => t.Durum!.Ad == "Onaylandi"),
                ReddedilenTalep = await talepler.CountAsync(t => t.Durum!.Ad == "Reddedildi")
            };

            return Ok(sonuc);
        }

        // GET /api/Dashboard/yetkili — Yetkili ve Yönetici
        [Authorize(Roles = "Yetkili,Yonetici")]
        [HttpGet("yetkili")]
        public async Task<IActionResult> YetkiliDashboard()
        {
            var talepler = _context.Talepler.Include(t => t.Durum).AsQueryable();
            var bugun = DateTime.UtcNow.Date;

            var sonuc = new
            {
                BekleyenTalepler = await talepler.CountAsync(t => t.Durum!.Ad == "Beklemede"),
                BugunGelenTalepler = await talepler.CountAsync(t => t.TalepTarihi.Date == bugun),
                OnaylananTalepler = await talepler.CountAsync(t => t.Durum!.Ad == "Onaylandi"),
                ReddedilenTalepler = await talepler.CountAsync(t => t.Durum!.Ad == "Reddedildi")
            };

            return Ok(sonuc);
        }

        // GET /api/Dashboard/yonetici — sadece Yönetici
        [Authorize(Roles = "Yonetici")]
        [HttpGet("yonetici")]
        public async Task<IActionResult> YoneticiDashboard()
        {
            int toplamMusteri = await _context.Musteriler.CountAsync(m => m.AktifMi);
            int toplamTalep = await _context.Talepler.CountAsync();

            var talepler = _context.Talepler.Include(t => t.Durum).AsQueryable();
            int onaylanan = await talepler.CountAsync(t => t.Durum!.Ad == "Onaylandi");
            int reddedilen = await talepler.CountAsync(t => t.Durum!.Ad == "Reddedildi");

            double onayOrani = toplamTalep == 0 ? 0 : Math.Round((double)onaylanan / toplamTalep * 100, 1);
            double redOrani = toplamTalep == 0 ? 0 : Math.Round((double)reddedilen / toplamTalep * 100, 1);

            int aktifYetkiliSayisi = await _context.Kullanicilar
                .Include(k => k.Rol)
                .CountAsync(k => k.Rol!.RolAdi == "Yetkili" && k.AktifMi);

            var sonuc = new
            {
                ToplamMusteri = toplamMusteri,
                ToplamTalep = toplamTalep,
                OnayOrani = onayOrani,
                RedOrani = redOrani,
                AktifYetkiliSayisi = aktifYetkiliSayisi
            };

            return Ok(sonuc);
        }

        // GET /api/Dashboard/aylik-talep — sadece Yönetici
        [Authorize(Roles = "Yonetici")]
        [HttpGet("aylik-talep")]
        public async Task<IActionResult> AylikTalep()
        {
            var sonuc = await _context.Database
                .SqlQuery<AylikTalepDto>($"EXEC SP_AylikTalepSayisi")
                .ToListAsync();

            return Ok(sonuc);
        }

        // GET /api/Dashboard/aktif-musteriler — sadece Yönetici
        [Authorize(Roles = "Yonetici")]
        [HttpGet("aktif-musteriler")]
        public async Task<IActionResult> AktifMusteriler()
        {
            var sonuc = await _context.Database
                .SqlQuery<AktifMusteriDto>($"EXEC SP_EnAktifMusteriler")
                .ToListAsync();

            return Ok(sonuc);
        }

        // GET /api/Dashboard/talep-detay-view — VW_TalepDetay view'ından veri çeker
        [Authorize(Roles = "Yetkili,Yonetici")]
        [HttpGet("talep-detay-view")]
        public async Task<IActionResult> TalepDetayView()
        {
            var sonuc = await _context.Database
                .SqlQuery<TalepDetayViewDto>($"SELECT * FROM VW_TalepDetay")
                .ToListAsync();

            return Ok(sonuc);
        }
    }
}