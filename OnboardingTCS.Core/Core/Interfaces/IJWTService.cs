using OnboardingTCS.Core.Entities;
using OnboardingTCS.Core.Core.DTOs;
using System.Security.Claims;

namespace OnboardingTCS.Core.Core.Interfaces
{
    public interface IJWTService
    {
        /// <summary>
        /// Genera un JWT token para el usuario
        /// </summary>
        /// <param name="usuario">Usuario para el cual generar el token</param>
        /// <returns>Token JWT como string</returns>
        string GenerateJwtToken(Usuario usuario);

        /// <summary>
        /// Genera un refresh token
        /// </summary>
        /// <returns>Refresh token como string</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Valida un JWT token
        /// </summary>
        /// <param name="token">Token a validar</param>
        /// <returns>ClaimsPrincipal si el token es válido, null si no</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Obtiene el user ID de un token
        /// </summary>
        /// <param name="token">Token del cual extraer el user ID</param>
        /// <returns>User ID como string</returns>
        string? GetUserIdFromToken(string token);

        /// <summary>
        /// Verifica si un token ha expirado
        /// </summary>
        /// <param name="token">Token a verificar</param>
        /// <returns>True si ha expirado, false si no</returns>
        bool IsTokenExpired(string token);
    }
}