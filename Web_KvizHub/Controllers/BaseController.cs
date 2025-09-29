using Microsoft.AspNetCore.Mvc;

namespace Web_KvizHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        // Ovo svojstvo će automatski čitati userId iz HttpContext
        protected int? UserId
        {
            get
            {
                // Uzmi userId koji je JWTMiddleware postavio
                var userId = HttpContext.Items["UserId"]?.ToString();
                if (int.TryParse(userId, out int id))
                {
                    return id;
                }
                return null; // Ako nema userId, korisnik nije ulogovan
            }
        }

        // Zajednička metoda za sve kontrolere
        protected IActionResult Unauthorized(string message = "Nemate pristup ovom resursu.")
        {
            return StatusCode(401, new { message });
        }
        protected bool IsAdmin
        {
            get
            {
                // Privremeno - svako je admin. Kasnije ćemo dodati pravu proveru
                return UserId.HasValue && (UserId.Value == 1 || UserId.Value == 2); // admin i marko su admini
            }
        }
    }
}
