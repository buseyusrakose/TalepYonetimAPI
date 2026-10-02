using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TalepOnaySistemiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("PirCloud Talep Sistemi API Çalışıyor");
        }

        [Authorize(Roles = "Yonetici")]
        [HttpGet("sadece-yonetici")]
        public IActionResult SadeceYonetici()
        {
            return Ok("Bu mesajı sadece Yönetici rolü görebilir.");
        }
    }
}