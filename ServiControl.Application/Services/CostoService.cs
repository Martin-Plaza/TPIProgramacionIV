using ServiControl.Application.Authorization;
using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

// Modulo: Costos
// Capa: Application
// Responsabilidad: Registra costos sobre trabajos existentes usando reglas del dominio.
public class CostoService : ICostoService
{
    private readonly ICostoRepository _costoRepository;
    private readonly ITrabajoRepository _trabajoRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IOperationalUserResolver _operationalUserResolver;

    public CostoService(
        ICostoRepository costoRepository,
        ITrabajoRepository trabajoRepository,
        ICurrentUserContext currentUserContext,
        IOperationalUserResolver operationalUserResolver)
    {
        _costoRepository = costoRepository;
        _trabajoRepository = trabajoRepository;
        _currentUserContext = currentUserContext;
        _operationalUserResolver = operationalUserResolver;
    }

//en el DTO costoEstimado y costoFinal son nulos porque pueden cargarse un trabajo sin registrar costo
    public async Task<CostoResponse> RegistrarCostoEstimadoAsync(
        RegisterCostoEstimadoRequest request,
        CancellationToken cancellationToken = default)
    {
        //esta funcion asincronica esta declarada al final de este modulo, con su inyeccion de repo de trabajos correspondiente
        //esa funcion tiene un throw, eso detiene el flujo si es que no encuentra ID de trabajo existente
        await ValidarTrabajoAccesibleAsync(request.TrabajoId, cancellationToken);

        var costo = await _costoRepository.GetByTrabajoIdAsync(request.TrabajoId, cancellationToken);

        //si no hay costo crea una instancia de Costo.cs
        if (costo is null)
        {
            costo = new Costo(request.TrabajoId, request.CostoEstimado);
            var created = await _costoRepository.AddAsync(costo, cancellationToken);

            return MapToResponse(created);
        }
        //si ya tiene un registro de costo se actualiza
        //registrarCostoEstimado es un metodo de la clase Costo.cs
        costo.RegistrarCostoEstimado(request.CostoEstimado);
        await _costoRepository.UpdateAsync(costo, cancellationToken);

        return MapToResponse(costo);
    }

    public async Task<CostoResponse> RegistrarCostoFinalAsync(
        RegisterCostoFinalRequest request,
        CancellationToken cancellationToken = default)
    {

        //esta funcion asincronica esta declarada al final de este modulo, con su inyeccion de repo de trabajos correspondiente
        //esa funcion tiene un throw, eso detiene el flujo si es que no encuentra ID de trabajo existente
        await ValidarTrabajoAccesibleAsync(request.TrabajoId, cancellationToken);

        //si sigue este flujo quiere decir que hay un ID de trabajo, y aca quiere capturar el costo de ese trabajo en la variable costo
        var costo = await _costoRepository.GetByTrabajoIdAsync(request.TrabajoId, cancellationToken);

        //si no hay costo (porque se puede registrar un trabajo sin costo) crea uno
        if (costo is null)
        {
            costo = new Costo(request.TrabajoId, costoFinal: request.CostoFinal);
            var created = await _costoRepository.AddAsync(costo, cancellationToken);

            return MapToResponse(created);
        }

        //si ya tiene un registro de costo se actualiza
        //registrarCostoFinal es un metodo de la clase Costo.cs
        costo.RegistrarCostoFinal(request.CostoFinal);
        await _costoRepository.UpdateAsync(costo, cancellationToken);

        return MapToResponse(costo);
    }

    //funcion 
    private async Task ValidarTrabajoAccesibleAsync(
        int trabajoId,
        CancellationToken cancellationToken)
    {
        var usuarioOperativoId = await _operationalUserResolver
            .ObtenerUsuarioOperativoIdAsync(cancellationToken);
        //si es admin puede ver todos los trabajos, cualquiera por id, sino busca por id y por usuario operativo 
        var trabajo = _currentUserContext.IsAdmin
            ? await _trabajoRepository.GetByIdAsync(trabajoId, cancellationToken)
            : await _trabajoRepository.GetByIdAndUsuarioAsync(
                trabajoId,
                usuarioOperativoId,
                cancellationToken);

        if (trabajo is null)
        {
            throw new ArgumentException("El trabajo indicado no existe.", nameof(trabajoId));
        }
    }

    private static CostoResponse MapToResponse(Costo costo)
    {
        return new CostoResponse(
            costo.Id,
            costo.TrabajoId,
            costo.CostoEstimado,
            costo.CostoFinal);
    }
}
