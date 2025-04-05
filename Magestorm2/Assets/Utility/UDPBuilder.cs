using System.Net;
using System.Collections.Generic;
using UnityEngine;
public static class UDPBuilder
{
    private static List<UDPGameClient> _clients;
    private static IPAddress _serverAddress;
    public static void Init(string hostname)
    {
        //_serverAddress = Dns.GetHostAddresses(hostname)[0];
        _serverAddress = IPAddress.Parse("192.168.1.93");
        Debug.Log("Server IP: " + _serverAddress.ToString());
        _clients = new List<UDPGameClient>();
    }
    public static UDPGameClient CreateClient(int port)
    {
        UDPGameClient toAdd = new UDPGameClient(new IPEndPoint(_serverAddress, port));
        _clients.Add(toAdd);
        return toAdd;
    }
    public static void StopAllListeners()
    {
        foreach (UDPGameClient client in _clients)
        {
            client.StopListening();
        }
    }
}
