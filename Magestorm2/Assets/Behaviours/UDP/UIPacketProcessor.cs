using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class UIPacketProcessor : MonoBehaviour
{
    private int _listeningPort;
    private UDPGameClient _udp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_udp.HasPacketsPending)
        {
            List<byte[]> toProcess = _udp.PacketsReceived();
            foreach (byte[] decryptedPayload in toProcess)
            {
                OpCode_Receive opCode = (OpCode_Receive)decryptedPayload[0];
                switch (opCode)
                {
                    case OpCode_Receive.AccountCreationFailed:

                        break;
                    case OpCode_Receive.AccountCreated:

                        break;
                }
            }
        }
    }

    public void Init(int port)
    {
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
    }
}
