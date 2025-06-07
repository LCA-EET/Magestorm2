using System;
using UnityEngine;

public class UDPProcessor : MonoBehaviour
{
    protected int _listeningPort;
    protected UDPGameClient _udp;
    protected byte[] _decrypted;
    protected OpCode_Receive _opCode;

    public void Init(int port)
    {
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
    }

    public void SendBytes(byte[] unencrypted)
    {
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
        _opCode = (OpCode_Receive)_decrypted[0];
    }
}

