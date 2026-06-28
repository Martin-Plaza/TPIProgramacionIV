using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;

namespace ServiControl.Presentation.Controllers;

// Modulo: Usuarios
// Capa: Presentation
// Responsabilidad: Expone la administracion de usuarios y roles solo para Admin.
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponseDto>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.ObtenerTodosAsync(cancellationToken);
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> ObtenerPorId(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _usuarioService.ObtenerPorIdAsync(id, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Actualizar(
        int id,
        UpdateUsuarioRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _usuarioService.ActualizarAsync(id, request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}/rol")]
    public async Task<ActionResult<UserResponseDto>> CambiarRol(
        int id,
        UpdateUsuarioRolRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _usuarioService.CambiarRolAsync(id, request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _usuarioService.EliminarAsync(id, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
