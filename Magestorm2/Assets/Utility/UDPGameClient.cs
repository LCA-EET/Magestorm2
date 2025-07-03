using UnityEngine;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
public class UDPGameClient
{
    private ConcurrentQueue<byte[]> _received;
    private IPEndPoint _remote;
    private UdpClient _client;
    private bool _listening;
    public UDPGameClient(IPEndPoint remote)
    {
        _listening = false;
        _received = new ConcurrentQueue<byte[]>();
        _client = new UdpClient(remote.Port);
        _remote = remote;
    }
    public void Listen()
    {
        _listening = true;
        Debug.Log("Starting UDP Listener thread, port " + _remote.Port);
        new Thread(ListenerThread).Start();
    }
    private void ListenerThread()
    {
        bool wasReceived = false;
        while (_listening)
        {
            try
            {
                wasReceived = false;
                byte[] received = _client.Receive(ref _remote);
                wasReceived = true; 
                _received.Enqueue(Cryptography.DecryptReceived(received));
            }
            catch(Exception ex) {
                if (wasReceived)
                {
                    Debug.LogException(ex);
                }  
            }
        }
    }
    public void StopListening()
    {
        _listening = false;
        _client.Close();
    }
    public void Send(byte[] toSend)
    {
        _client.Send(toSend, toSend.Length, _remote);
    }
    public bool HasPacketsPending
    {
        get
        {
            return _received.Count > 0;
        }
    }
    public List<byte[]> PacketsReceived()
    {
        List<byte[]> toReturn = new List<byte[]>();
        while (!_received.IsEmpty)
        {
            byte[] receivedBytes;
            if (_received.TryDequeue(out receivedBytes))
            {
                toReturn.Add(receivedBytes);
            }
        }
        return toReturn;
    }
}
