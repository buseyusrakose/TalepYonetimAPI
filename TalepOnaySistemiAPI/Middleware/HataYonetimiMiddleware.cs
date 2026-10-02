using System.Net;
using System.Text.Json;

namespace TalepOnaySistemiAPI.Middleware
{
    public class HataYonetimiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HataYonetimiMiddleware> _logger;

        public HataYonetimiMiddleware(RequestDelegate next, ILogger<HataYonetimiMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var hataYaniti = new
                {
                    mesaj = "Sunucuda beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                    detay = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                        ? ex.Message
                        : null
                };

                var json = JsonSerializer.Serialize(hataYaniti);
                await context.Response.WriteAsync(json);
            }
        }
    }
}