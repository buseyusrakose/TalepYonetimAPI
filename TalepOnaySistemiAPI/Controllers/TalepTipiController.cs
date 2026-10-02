using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalepOnaySistemiAPI.Data;

namespace TalepOnaySistemiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TalepTipiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TalepTipiController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/TalepTipi — giriş yapmış her rol çağırabilir
        [HttpGet]
        public async Task<IActionResult> Listele()
        {
            var tipler = await _context.TalepTipleri
                .Select(t => new { t.Id, t.Ad })
                .ToListAsync();

            return Ok(tipler);
        }
    }
}