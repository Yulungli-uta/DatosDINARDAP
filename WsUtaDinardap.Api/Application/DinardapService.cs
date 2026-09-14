using WsUtaDinardap.Api.Application.Errors;
using WsUtaDinardap.Api.Application.Interfaces;
using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application;

public interface IDinardapService
{
    Task<RegistroCivilData> ConsultarRegistroCivilAsync(string numeroIdentificacion, CancellationToken ct);
    Task<TceData> ConsultarTceAsync(string numeroIdentificacion, CancellationToken ct);
    Task<IReadOnlyList<TituloData>> ConsultarTitulosAsync(string numeroIdentificacion, CancellationToken ct);
}

/// <summary>
/// Orquesta gateway + handler por paquete. No conoce SOAP ni JWT/permisos - solo coordina.
/// Agregar un paquete nuevo = agregar un metodo Consultar*Async que reutiliza el mismo
/// ConsultarInternoAsync generico; el gateway y la validacion de identificacion no cambian.
/// </summary>
public sealed class DinardapService : IDinardapService
{
    private readonly IDinardapGateway _gateway;
    private readonly IDinardapPackageHandler<RegistroCivilData> _registroCivilHandler;
    private readonly IDinardapPackageHandler<TceData> _tceHandler;
    private readonly IDinardapPackageHandler<IReadOnlyList<TituloData>> _titulosHandler;

    public DinardapService(
        IDinardapGateway gateway,
        IDinardapPackageHandler<RegistroCivilData> registroCivilHandler,
        IDinardapPackageHandler<TceData> tceHandler,
        IDinardapPackageHandler<IReadOnlyList<TituloData>> titulosHandler)
    {
        _gateway = gateway;
        _registroCivilHandler = registroCivilHandler;
        _tceHandler = tceHandler;
        _titulosHandler = titulosHandler;
    }

    public Task<RegistroCivilData> ConsultarRegistroCivilAsync(string numeroIdentificacion, CancellationToken ct) =>
        ConsultarInternoAsync(numeroIdentificacion, _registroCivilHandler, ct);

    public Task<TceData> ConsultarTceAsync(string numeroIdentificacion, CancellationToken ct) =>
        ConsultarInternoAsync(numeroIdentificacion, _tceHandler, ct);

    public Task<IReadOnlyList<TituloData>> ConsultarTitulosAsync(string numeroIdentificacion, CancellationToken ct) =>
        ConsultarInternoAsync(numeroIdentificacion, _titulosHandler, ct);

    private async Task<TResult> ConsultarInternoAsync<TResult>(
        string numeroIdentificacion,
        IDinardapPackageHandler<TResult> handler,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(numeroIdentificacion) || numeroIdentificacion.Length is < 10 or > 13)
            throw new DinardapException(DinardapErrorCode.InvalidIdentification,
                "El numero de identificacion es requerido y debe tener entre 10 y 13 caracteres.");

        var ficha = await _gateway.ConsultarAsync(numeroIdentificacion, handler.Package, ct);
        return handler.Parse(ficha);
    }
}
