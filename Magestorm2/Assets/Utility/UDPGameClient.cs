using UnityEngine;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class UDPGameClient
{
    private ConcurrentQueue<byte[]> _received;
    private IPEndPoint _remote;
    private UdpClient _client;
    private bool _listening;
    private int _localPort;
    public UDPGameClient(IPEndPoint remote)
    {
        _listening = false;
        _received = new ConcurrentQueue<byte[]>();
        _remote = remote;
        Listen();
    }
    private void Listen()
    {
        bool bound = false;
        byte bindAttempts = 0;
        while (!bound && bindAttempts < 10)
        {
            try
            {
                bindAttempts++;
                _localPort = SharedFunctions.RandomInt(10000, 20000);
                _client = new UdpClient(_localPort);
                bound = true;
            }
            catch (SocketException) { }
        }
        if (bound)
        {
            _listening = true;
            Debug.Log("Starting UDP Listener thread, remote port " + _remote.Port);
            new Thread(ListenerThread).Start();
        }
        else
        {
            Game.Quit();
        }
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
        _received.Clear();
        _client.Close();
        _client.Dispose();
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
    public IPEndPoint RemoteEnd
    {
        get{
            return _remote;
        } 
    }

    public int LocalPort
    {
        get { return _localPort; }
    }
}
