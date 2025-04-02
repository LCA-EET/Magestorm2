using System.Net.Sockets;
using UnityEngine;

public class UDPSender
{
    private UdpClient _udpClient;
    public UDPSender()
    {
        ComponentRegister.UDPSender = this;
        _udpClient = new UdpClient();
    }
}
