using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SetPoint.BLL._02.UsersManagement.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SetPoint.BLL._1.Security
{
    public class TokenService : ITokenService
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        #endregion


        #region Constructors

        public TokenService(IConfiguration config)
        {
            _config = config;

            var secretKey = _config["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey not configured");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        #endregion


        #region Methods
        public string CreateToken(UserReadDto user)
        {
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var expire = int.Parse(_config["JwtSettings:ExpireMinutes"] ?? "7");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expire),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);

            return _tokenHandler.WriteToken(token);
        }

        #endregion
    }
}
