using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WsUtaDinardap.Api.Infrastructure.Dinardap.Proxy;

// Puerto del proxy WCF generado en DatosDINARDAP/Service References/ServiceReferenceDINARDAP/Reference.cs
// (mismo WSDL: https://interoperabilidad.dinardap.gob.ec:7979/interoperador?wsdl), adaptado a
// System.ServiceModel.Primitives/Http (soporte de cliente WCF para .NET moderno). Mismos nombres
// de elemento XML/orden que el original para no romper la serializacion contra DINARDAP.
//
// CS8981 deshabilitado a proposito: los nombres en minuscula (institucion/registro/item) son
// exactamente los nombres de elemento XML del WSDL de DINARDAP, no una eleccion de estilo.
#pragma warning disable CS8981

[ServiceContract(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", ConfigurationName = "IInteroperador")]
public interface IInteroperador
{
    [OperationContract(Action = "", ReplyAction = "*")]
    [XmlSerializerFormat(SupportFaults = true)]
    Task<getFichaGeneralResponse> getFichaGeneralAsync(getFichaGeneral request);
}

[MessageContract(WrapperName = "getFichaGeneral", WrapperNamespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", IsWrapped = true)]
public sealed class getFichaGeneral
{
    [MessageBodyMember(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", Order = 0)]
    public string numeroIdentificacion = string.Empty;

    [MessageBodyMember(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", Order = 1)]
    public string codigoPaquete = string.Empty;

    public getFichaGeneral() { }

    public getFichaGeneral(string numeroIdentificacion, string codigoPaquete)
    {
        this.numeroIdentificacion = numeroIdentificacion;
        this.codigoPaquete = codigoPaquete;
    }
}

[MessageContract(WrapperName = "getFichaGeneralResponse", WrapperNamespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", IsWrapped = true)]
public sealed class getFichaGeneralResponse
{
    [MessageBodyMember(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/", Order = 0)]
    public fichaGeneral? @return;

    public getFichaGeneralResponse() { }

    public getFichaGeneralResponse(fichaGeneral @return) => this.@return = @return;
}

[System.Xml.Serialization.XmlType(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/")]
public sealed class fichaGeneral
{
    [System.Xml.Serialization.XmlElement(Order = 0)]
    public string? codigoPaquete { get; set; }

    [System.Xml.Serialization.XmlArray(Order = 1), System.Xml.Serialization.XmlArrayItem("registros")]
    public institucion[]? instituciones { get; set; }

    [System.Xml.Serialization.XmlElement(Order = 2)]
    public string? mensaje { get; set; }
}

[System.Xml.Serialization.XmlType(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/")]
public sealed class institucion
{
    [System.Xml.Serialization.XmlArray(Order = 0), System.Xml.Serialization.XmlArrayItem("registros")]
    public registro[]? datosPrincipales { get; set; }

    [System.Xml.Serialization.XmlArray(Order = 1), System.Xml.Serialization.XmlArrayItem("items")]
    public item[]? detalle { get; set; }

    [System.Xml.Serialization.XmlElement(Order = 2)]
    public string? mensaje { get; set; }

    [System.Xml.Serialization.XmlElement(Order = 3)]
    public string? nombre { get; set; }
}

[System.Xml.Serialization.XmlType(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/")]
public sealed class registro
{
    [System.Xml.Serialization.XmlElement(Order = 0)]
    public string? campo { get; set; }

    [System.Xml.Serialization.XmlElement(Order = 1)]
    public string? codigo { get; set; }

    [System.Xml.Serialization.XmlElement(Order = 2)]
    public string? valor { get; set; }
}

[System.Xml.Serialization.XmlType(Namespace = "http://servicio.interoperadorws.interoperacion.dinardap.gob.ec/")]
public sealed class item
{
    [System.Xml.Serialization.XmlElement(Order = 0)]
    public string? nombre { get; set; }

    [System.Xml.Serialization.XmlArray(Order = 1), System.Xml.Serialization.XmlArrayItem("registros")]
    public registro[]? registros { get; set; }
}

public sealed class InteroperadorClient : ClientBase<IInteroperador>, IInteroperador
{
    public InteroperadorClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }

    public Task<getFichaGeneralResponse> getFichaGeneralAsync(getFichaGeneral request) =>
        Channel.getFichaGeneralAsync(request);

    public async Task<fichaGeneral?> GetFichaGeneralAsync(string numeroIdentificacion, string codigoPaquete)
    {
        var response = await getFichaGeneralAsync(new getFichaGeneral(numeroIdentificacion, codigoPaquete));
        return response.@return;
    }
}
