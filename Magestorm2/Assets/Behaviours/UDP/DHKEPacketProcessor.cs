using UnityEngine;
using System.Collections.Generic;
using System;
using System.Numerics;
public class DHKEPacketProcessor : UDPProcessor
{
    private BigInteger _kPr, _alpha, _p;
    private void Awake()
    {
        ComponentRegister.DHKEPacketProcessor = this;
        Init(Game.GameServerPort);
    }

    public override void Init(int remotePort)
    {
        base.Init(remotePort);
        Game.UDP.Send(DHKE_Packets.DPRequestPacket());
        Debug.Log("Sent DHKE Request to Server.");
    }

    private void Update()
    {
        if (Game.UDP.HasPacketsPending)
        {
            List<byte[]> toProcess = Game.UDP.PacketsReceived();
            foreach (byte[] payload in toProcess)
            {
                byte opCode = payload[0];
                switch (opCode)
                {
                    case DHKE_Receive.DPReply:
                        ProcessDP(payload);
                        break;
                    case DHKE_Receive.KeyReply:
                        ProcessKeyReply(payload);
                        break;
                }

            }
        }
    }
    private void ProcessKeyReply(byte[] payload)
    {
        Debug.Log("Key reply received");
        short kpubLength = BitConverter.ToInt16(payload, 1);
        byte[] kpub = new byte[kpubLength];
        Array.Copy(payload, 3, kpub, 0, kpubLength);
        short xorKeyLength = BitConverter.ToInt16(payload, 3 + kpubLength);
        byte[] xorKey = new byte[xorKeyLength];
        Array.Copy(payload, 3 + kpubLength + 2, xorKey, 0, xorKeyLength);
        BigInteger aesXORBI = new BigInteger(xorKey, false, true);
        short remainder = BitConverter.ToInt16(payload, 3 + kpubLength + 2 + xorKeyLength);
        BigInteger serverKpub = new BigInteger(kpub, false, true);
        Debug.Log("Server public key: " + serverKpub);
        BigInteger sharedSecret = BigInteger.ModPow(serverKpub, _kPr, _p);
        Debug.Log("Shared Secret: " + sharedSecret);
        string hexString = sharedSecret.ToString("X");
        Debug.Log("Shared Secret Hex: " + hexString);
        BigInteger first16 = Cryptography.First16Bytes(sharedSecret);
        hexString = first16.ToString("X");

        BigInteger aesKey = aesXORBI ^ first16;
        Debug.Log("AES Key: " + aesKey);

        // uppercase hex
        Debug.Log(hexString);
    }
    private void ProcessDP(byte[] payload) 
    {
        short pLength = BitConverter.ToInt16(payload, 1);
        short aLength = BitConverter.ToInt16(payload, 3);
        _p = ByteUtils.BytesToBigInteger(payload, 5, pLength);
        _alpha = ByteUtils.BytesToBigInteger(payload, 5 + pLength, aLength);
        short remainder = BitConverter.ToInt16(payload, 5 + pLength + aLength);
        BigInteger sum = (_p + _alpha);
        if(remainder == Cryptography.ModChecksum(sum))
        {
            _kPr = Cryptography.RandomBigInteger(new BigInteger(2), _p - 2);
            BigInteger clientPublicKey = BigInteger.ModPow(_alpha, _kPr, _p);
            Debug.Log("Public Key: " + clientPublicKey);
            Game.UDP.Send(DHKE_Packets.ClientPublicKeyPacket(clientPublicKey));
        }
        else
        {
            ComponentRegister.UIPrefabManager.InstantiateMessageBox(Language.GetBaseString(414));
        }
    }
}
