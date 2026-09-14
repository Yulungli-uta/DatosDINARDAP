using WsUtaDinardap.Api.Domain;

namespace WsUtaDinardap.Api.Application.Interfaces;

/// <summary>
/// Interpreta la ficha generica de un paquete DINARDAP y la convierte al modelo de dominio
/// especifico de ese paquete (mapeo de codigos de campo). Es la unica pieza que cambia al
/// agregar un paquete nuevo - nunca IDinardapGateway.
/// </summary>
public interface IDinardapPackageHandler<TResult>
{
    DinardapPackage Package { get; }

    TResult Parse(FichaGeneralDto ficha);
}
