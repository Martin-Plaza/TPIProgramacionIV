using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiControl.Application.Authorization;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Infrastructure.Authentication;

// Modulo: Autenticacion
// Capa: Infrastructure
// Responsabilidad: Genera JWT firmados usando configuracion externa.
// Nota: Application solo conoce IJwtTokenGenerator, no detalles de librerias JWT.
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuario usuario)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expirationMinutesValue = _configuration["Jwt:ExpirationMinutes"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("La clave JWT no esta configurada.");
        }

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("El issuer JWT no esta configurado.");
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("El audience JWT no esta configurado.");
        }

        if (!int.TryParse(expirationMinutesValue, out var expirationMinutes) || expirationMinutes <= 0)
        {
            throw new InvalidOperationException("La expiracion JWT debe ser mayor a cero.");
        }

        // JWT permite proteger endpoints sin guardar estado de sesion en el servidor.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(AuthClaimTypes.UsuarioId, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
