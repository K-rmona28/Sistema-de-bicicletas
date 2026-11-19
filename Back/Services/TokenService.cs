using System.Security.Cryptography;
using System.Text;
using ApiCycleOn.Common;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ApiCycleOn.Services;

public interface ITokenService
{
    (string Token, DateTime Expiracion) GenerarAcceso(int idUsuario, string email, string nombreCompleto, string rol);
    string GenerarTokenRestablecimiento(int idUsuario, string passHash);
    Task<(int IdUsuario, string Huella)?> LeerTokenRestablecimientoAsync(string token);
    string HuellaPass(string passHash);
}

public sealed class TokenService(IConfiguration config) : ITokenService
{
    private static readonly JsonWebTokenHandler Handler = new();

    private readonly SymmetricSecurityKey _key = new(Encoding.UTF8.GetBytes(
        config["Jwt:Key"] ?? throw new InvalidOperationException("Falta la sección Jwt:Key en appsettings.json.")));
    private readonly string _issuer = config["Jwt:Issuer"] ?? "ApiCycleOn";
    private readonly string _audience = config["Jwt:Audience"] ?? "CycleOnClientes";
    private readonly int _minutos = config.GetValue("Jwt:ExpiracionMinutos", 60);

    // Audiencia distinta: un token de restablecimiento NUNCA sirve como token de acceso
    private string AudienciaReset => _audience + ":reset";

    public (string Token, DateTime Expiracion) GenerarAcceso(int idUsuario, string email, string nombreCompleto, string rol)
    {
        var expira = DateTime.UtcNow.AddMinutes(_minutos);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Expires = expira,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = idUsuario.ToString(),
                [JwtRegisteredClaimNames.Email] = email,
                ["name"] = nombreCompleto,
                [ClaimsExtensions.ClaimRol] = rol
            }
        };
        return (Handler.CreateToken(descriptor), expira);
    }

    public string GenerarTokenRestablecimiento(int idUsuario, string passHash)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = AudienciaReset,
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = idUsuario.ToString(),
                ["purpose"] = "reset",
                ["ph"] = HuellaPass(passHash)   // cambia al usar el token => queda de un solo uso
            }
        };
        return Handler.CreateToken(descriptor);
    }

    public async Task<(int IdUsuario, string Huella)?> LeerTokenRestablecimientoAsync(string token)
    {
        var resultado = await Handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = _issuer,
            ValidateAudience = true, ValidAudience = AudienciaReset,
            ValidateLifetime = true, ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true, IssuerSigningKey = _key
        });

        if (!resultado.IsValid) return null;

        var claims = resultado.Claims;
        if (!claims.TryGetValue("purpose", out var proposito) || proposito?.ToString() != "reset") return null;
        if (!claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var sub) || !int.TryParse(sub?.ToString(), out var id)) return null;

        claims.TryGetValue("ph", out var huella);
        return (id, huella?.ToString() ?? string.Empty);
    }

    public string HuellaPass(string passHash) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(passHash)))[..16];
}
