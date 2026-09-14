using System.Net;
using WsUtaDinardap.Api.Infrastructure.Security;
using Xunit;

namespace WsUtaDinardap.Api.Tests;

public class IpNetworkMatcherTests
{
    [Theory]
    [InlineData("10.102.12.83", "10.102.12.0/24", true)]
    [InlineData("10.102.12.255", "10.102.12.0/24", true)]
    [InlineData("10.102.13.1", "10.102.12.0/24", false)]
    [InlineData("10.102.12.83", "10.102.12.83", true)] // IP exacta, sin CIDR
    [InlineData("10.102.12.84", "10.102.12.83", false)]
    [InlineData("192.168.1.10", "192.168.1.0/28", true)]
    [InlineData("192.168.1.20", "192.168.1.0/28", false)] // fuera del rango /28 (0-15)
    public void IsInAnyNetwork_EvaluaCidrCorrectamente(string ip, string network, bool esperado)
    {
        var address = IPAddress.Parse(ip);

        var resultado = IpNetworkMatcher.IsInAnyNetwork(address, new[] { network });

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void IsInAnyNetwork_ConVariasRedes_CoincideConCualquieraDeElas()
    {
        var address = IPAddress.Parse("172.16.5.5");
        var redes = new[] { "10.102.12.0/24", "172.16.5.0/24" };

        Assert.True(IpNetworkMatcher.IsInAnyNetwork(address, redes));
    }

    [Fact]
    public void IsInAnyNetwork_ListaVacia_NuncaConfia()
    {
        var address = IPAddress.Parse("10.102.12.83");

        Assert.False(IpNetworkMatcher.IsInAnyNetwork(address, Array.Empty<string>()));
    }

    [Fact]
    public void IsInAnyNetwork_DireccionNull_NuncaConfia()
    {
        Assert.False(IpNetworkMatcher.IsInAnyNetwork(null, new[] { "10.102.12.0/24" }));
    }
}
