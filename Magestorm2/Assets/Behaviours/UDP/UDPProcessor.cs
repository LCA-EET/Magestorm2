using System;
using System.Net;
using UnityEngine;

public class UDPProcessor : MonoBehaviour
{
    protected int _remotePort;
    protected byte[] _decrypted;
    protected byte _opCode;

    public void Init(int remotePort)
    {
        Debug.Log("Initializing UDP client, remote port: " + remotePort);
        Game.UDP = new UDPGameClient(new IPEndPoint(Game.GameServerAddress, remotePort));
        _remotePort = remotePort;
    }

    public void SendBytes(byte[] unencrypted)
    {
        //Debug.Log("Sending in-game packet on port " + _udp.RemoteEnd().ToString());
        if (unencrypted[0] == InGame_Send.JoinedMatch)
        {
            Debug.Log("Sending UDP packet to " + Game.UDP.RemoteEnd.Address.ToString() + ":" + Game.UDP.RemoteEnd.Port + " from " + Game.UDP.LocalPort);
        }
        Cryptography.EncryptAndSend(unencrypted);
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

