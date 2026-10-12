using System;
using System.Net;
using UnityEngine;

public class UDPProcessor : MonoBehaviour
{
    protected int _remotePort;
    protected byte[] _decrypted;
    protected byte _opCode;
    protected Cryptographer _cryptographer;
    protected UDPGameClient _udp;

    public virtual void Init(int remotePort, byte cipherCode, byte[] secretKey)
    {
        Debug.Log("Initializing UDP client, remote port: " + remotePort);
        _udp = new UDPGameClient(new IPEndPoint(Game.GameServerAddress, remotePort));
        _remotePort = remotePort;
        _cryptographer = new Cryptographer(cipherCode, secretKey);
    }

    public void SendBytes(byte[] unencrypted)
    {
        _udp.Send(_cryptographer.Encrypt(unencrypted));
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

}

