using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmotionAPI.Services
{
    public class FirebaseTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public bool CanValidateToken => true;

        private int _maximumTokenSizeInBytes = 2 * 1024 * 1024; // Define manualmente o limite de 2 MB

        public int MaximumTokenSizeInBytes
        {
            get => _maximumTokenSizeInBytes;
            set => _maximumTokenSizeInBytes = value;
        }

        public bool CanReadToken(string securityToken) => _tokenHandler.CanReadToken(securityToken);

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            var decodedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(securityToken).Result;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid)
            };

            var identity = new ClaimsIdentity(claims, "Firebase");
            return new ClaimsPrincipal(identity);
        }
    }
}
