using UnityEngine;
using System.Collections.Generic;
using System;
using System.Numerics;
using System.Linq;
public class DHKEPacketProcessor : UDPProcessor
{
    private BigInteger _kPr, _alpha, _p;
    private void Awake()
    {
        ComponentRegister.DHKEPacketProcessor = this;
        if(ComponentRegister.PregamePacketProcessor != null)
        {
            Destroy(ComponentRegister.PregamePacketProcessor.gameObject);
        }
        Init(Game.GameServerPort, ControlCodes.CipherCode_None, null);
    }

    public override void Init(int remotePort, byte cipherCode, byte[] secretKey)
    {
        base.Init(remotePort, cipherCode, secretKey);
        SendBytes(DHKE_Packets.DPRequestPacket());
        Debug.Log("Sent DHKE Request to Server.");
    }

    private void Update()
    {
        if (_udp.HasPacketsPending)
        {
            List<byte[]> toProcess = _udp.PacketsReceived();
            foreach (byte[] encrypted in toProcess)
            {
                byte[] payload = _cryptographer.Decrypt(encrypted);
                byte opCode = payload[0];
                Debug.Log("DHKE OpCode: " + opCode);
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
        Debug.Log("AES XOR BI (Hex): " + aesXORBI.ToString("X"));
        Debug.Log("AES XOR BI (Decimal): " + aesXORBI.ToString());
        int checksumIndex = 3 + kpubLength + 2 + xorKeyLength;
        Debug.Log("Checksum Index: " + checksumIndex);

        if (Cryptographer.VerifyChecksum(payload, checksumIndex)){
            BigInteger serverKpub = new BigInteger(kpub, false, true);
            BigInteger sharedSecret = BigInteger.ModPow(serverKpub, _kPr, _p);
            Debug.Log("Shared Secret (Hex): " + sharedSecret.ToString("X"));
            Debug.Log("Shared Secret (Decimal): " + sharedSecret.ToString());
            BigInteger first16 = Cryptographer.First16Bytes(sharedSecret);
            Debug.Log("First 16 (Hex): " + first16.ToString("X"));
            Debug.Log("First 16 (Decimal): " + first16.ToString());
            BigInteger aesKey = aesXORBI ^ first16;
            aesKey = new BigInteger(aesKey.ToByteArray(false, true), false, true);
            byte[] keyBytes = aesKey.ToByteArray(false, true);
            Cryptographer.StoreKey(PlayerPrefs.GetString("server"), keyBytes);
            Debug.Log("AES Key (Hex): " + aesKey.ToString("X"));
            Debug.Log("AES Key (Decimal): " + aesKey.ToString());
            Game.PregameKey = keyBytes;
            ByteUtils.PrintByteArray(keyBytes);
            if (ComponentRegister.PregamePacketProcessor == null)
            {
                ComponentRegister.UIPrefabManager.InstantiatePregamePacketProcessor();
                ComponentRegister.UILoginForm.ToPerform();
            }
        }
        else
        {
            Game.MessageBoxReference(414);
        }
    }
    private void ProcessDP(byte[] payload) 
    {
        short pLength = BitConverter.ToInt16(payload, 1);
        short aLength = BitConverter.ToInt16(payload, 3);
        _p = ByteUtils.BytesToBigInteger(payload, 5, pLength);
        _alpha = ByteUtils.BytesToBigInteger(payload, 5 + pLength, aLength);
        int checksumIndex = 5 + pLength + aLength;
        if(Cryptographer.VerifyChecksum(payload, checksumIndex))
        {
            BigInteger sum = (_p + _alpha);
            _kPr = Cryptographer.RandomBigInteger(new BigInteger(2), _p - 2);
            BigInteger clientPublicKey = BigInteger.ModPow(_alpha, _kPr, _p);
            Debug.Log("Public Key: " + clientPublicKey);
            SendBytes(DHKE_Packets.ClientPublicKeyPacket(clientPublicKey));
        }
        else
        {
            Debug.Log("fail A");
            Game.MessageBoxReference(414);
        }
    }
}
