using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Web_KvizHub.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // 1. Uzmi token iz Authorization headera
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                // 2. Pročitaj token i postavi userId
                await AttachUserToContext(context, token);
            }

            // 3. Nastavi sa sledećim middleware-om
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                // 4. Dekodiraj JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // 5. Uzmi userId iz tokena
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                // 6. Postavi userId u HttpContext (memorija za trenutni zahtev)
                if (int.TryParse(userId, out int id))
                {
                    context.Items["UserId"] = id;
                }
            }
            catch
            {
                // Ako token nije validan, ignoriši - korisnik nije ulogovan
            }
        }
    }
}
