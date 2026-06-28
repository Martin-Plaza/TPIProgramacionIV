using ServiControl.Application.Interfaces;

namespace ServiControl.Infrastructure.Security;

// Modulo: Seguridad
// Capa: Infrastructure
// Responsabilidad: Hashea y verifica passwords usando BCrypt.
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La password es obligatoria.", nameof(password));
        }

        // Nunca se guarda la password en texto plano; solo se persiste el hash.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    //primero se fija si estan vacios, despues compara el password con el password hasheado (hashea el primero para comparar)
    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
