using System;
using UnityEngine;

public class UDPProcessor : MonoBehaviour
{
    protected int _listeningPort;
    protected UDPGameClient _udp;
    protected byte[] _decrypted;
    protected byte _opCode;

    public void Init(int port)
    {
        Debug.Log("Initializing UDP client, listening on port " + port);
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
        _udp.Listen();
    }

    public void SendBytes(byte[] unencrypted)
    {
        //Debug.Log("Sending in-game packet on port " + _udp.RemoteEnd().ToString());
        Cryptography.EncryptAndSend(unencrypted, _udp);
    }

    protected byte[] FillSegment(byte[] source, int sourceIndex, int length)
    {
        byte[] statBytes = new byte[length];
        Array.Copy(source, sourceIndex, statBytes, 0, length);
        return statBytes;
    }

    protected void PreProcess(byte[] decrypted)
    {
        _decrypted = decrypted;
        _opCode = _decrypted[0];
    }
    public UDPGameClient GameClient
    {
        get { return _udp; }
    }
}

