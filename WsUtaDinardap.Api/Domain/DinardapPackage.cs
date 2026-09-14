namespace WsUtaDinardap.Api.Domain;

/// <summary>
/// Paquetes DINARDAP soportados. Agregar un paquete nuevo = agregar un valor aqui
/// + un IDinardapPackageHandler nuevo; nunca requiere tocar IDinardapGateway/DinardapSoapGateway.
/// </summary>
public enum DinardapPackage
{
    RegistroCivil = 113,
    Tce = 114,
    Titulos = 115
}
