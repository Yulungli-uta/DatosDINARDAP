using System.Net;
using System.Net.Sockets;

namespace WsUtaDinardap.Api.Infrastructure.Security;

/// <summary>Compara una IP contra una lista de IPs exactas o rangos CIDR ("10.102.12.0/24").</summary>
public static class IpNetworkMatcher
{
    public static bool IsInAnyNetwork(IPAddress? address, IReadOnlyCollection<string> networks)
    {
        if (address is null || networks.Count == 0) return false;

        if (address.IsIPv4MappedToIPv6)
            address = address.MapToIPv4();

        foreach (var network in networks)
        {
            if (Matches(address, network))
                return true;
        }
        return false;
    }

    private static bool Matches(IPAddress address, string network)
    {
        var parts = network.Split('/', 2);

        if (!IPAddress.TryParse(parts[0].Trim(), out var networkAddress))
            return false;

        if (networkAddress.IsIPv4MappedToIPv6)
            networkAddress = networkAddress.MapToIPv4();

        if (address.AddressFamily != networkAddress.AddressFamily)
            return false;

        if (parts.Length == 1)
            return address.Equals(networkAddress);

        if (!int.TryParse(parts[1], out var prefixLength))
            return false;

        var addressBytes = address.GetAddressBytes();
        var networkBytes = networkAddress.GetAddressBytes();

        if (prefixLength < 0 || prefixLength > addressBytes.Length * 8)
            return false;

        var fullBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;

        for (var i = 0; i < fullBytes; i++)
            if (addressBytes[i] != networkBytes[i])
                return false;

        if (remainingBits > 0)
        {
            var mask = (byte)~(0xFF >> remainingBits);
            if ((addressBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
                return false;
        }

        return true;
    }
}
