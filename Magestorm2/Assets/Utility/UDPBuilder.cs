using System.Net;
using System.Collections.Generic;
using UnityEngine;
public static class UDPBuilder
{
    private static Dictionary<int, UDPGameClient> _clients;
    private static IPAddress _serverAddress;
    public static void Init(string hostname)
    {
        //_serverAddress = Dns.GetHostAddresses(hostname)[0];
        _serverAddress = IPAddress.Parse("192.168.1.93");
        Debug.Log("Server IP: " + _serverAddress.ToString());
        _clients = new Dictionary<int, UDPGameClient>();
    }
    public static int CreateClient(int port)
    {
        UDPGameClient toAdd = new UDPGameClient(new IPEndPoint(_serverAddress, port));
        _clients.Add(port, toAdd);
        return port;
    }
    public static void StopAllListeners()
    {
        foreach (UDPGameClient client in _clients.Values)
        {
            client.StopListening();
        }
    }
    public static bool StartListening(int port)
    {
        if (_clients.ContainsKey(port))
        {
            _clients[port].Listen();
            return true;
        }
        return false;
    }
    public static UDPGameClient GetClient(int port)
    {
        if (!_clients.ContainsKey(port))
        {
            CreateClient(port);
        }
        return _clients[port];
    }
}
