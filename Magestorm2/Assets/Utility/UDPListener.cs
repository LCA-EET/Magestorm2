using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
public class UDPListener 
{
    private ConcurrentQueue<byte[]> _receivedPackets = new ConcurrentQueue<byte[]>();
    private IPEndPoint _server;    
    private UdpClient _listener;
    private int _port;
    public UDPListener(int port, IPEndPoint remote)
    {
        ComponentRegister.UDPListener = this;
        _receivedPackets = new ConcurrentQueue<byte[]>();
        _port = port;
        _server = remote;
        new Thread(Listen).Start();
    }
    private void Listen()
    {
        _listener = new UdpClient(_port);
        while (Game.Running)
        {
            byte[] received = _listener.Receive(ref _server);
            _receivedPackets.Enqueue(received);
        }
    }
    public List<byte[]> PacketsReceived()
    {
        List<byte[]> toReturn = new List<byte[]>();
        while (!_receivedPackets.IsEmpty)
        {
            byte[] receivedBytes;
            if (_receivedPackets.TryDequeue(out receivedBytes))
            {
                toReturn.Add(receivedBytes);
            }
        }
        return toReturn;
    }
}
