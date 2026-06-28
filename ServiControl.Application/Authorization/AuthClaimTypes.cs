namespace ServiControl.Application.Authorization;
//definimos el como se va a llamar el claim para id usuarios
//en vez de poner new Claim("IdUsuario", usuario.Id.ToString());
//llamamos directamente new Claim(AuthClaimTypes.UsuarioId, usuario.Id.ToString());
//para no poner distintos nombres
public static class AuthClaimTypes
{
    public const string UsuarioId = "IdUsuario";
}
