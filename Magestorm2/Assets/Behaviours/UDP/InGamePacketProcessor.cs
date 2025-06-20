using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InGamePacketProcessor : UDPProcessor
{
    private void Awake()
    {
        ComponentRegister.InGamePacketProcessor = this;
        Init(MatchParams.ListeningPort);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_listeningPort > 0)
        {
            if (_udp.HasPacketsPending)
            {
                List<byte[]> toProcess = _udp.PacketsReceived();
                foreach (byte[] decryptedPayload in toProcess)
                {
                    PreProcess(decryptedPayload);
                    switch (_opCode)
                    {
                        case OpCode_Receive.ObjectStateChange:
                            ProcessObjectChangePacket();
                            break;
                        case OpCode_Receive.ShrineHealth:
                            ProcessShrineHealthPacket();
                            break;
                        case OpCode_Receive.AllShrineHealth:
                            ProcessAllShrineHealthPacket();
                            break;
                    }
                }
            }
        }
    }
    private void ProcessAllShrineHealthPacket()
    {
        Match.ChangeShrineHealth((byte)Team.Chaos, _decrypted[1]);
        Match.ChangeShrineHealth((byte)Team.Balance, _decrypted[2]);
        Match.ChangeShrineHealth((byte)Team.Order, _decrypted[3]);
    }
    private void ProcessShrineHealthPacket()
    {
        Match.ChangeShrineHealth(_decrypted[1], _decrypted[2]);
    }
    private void ProcessObjectChangePacket()
    {
        Match.ChangeObjectState(_decrypted[1], _decrypted[2]);
    }
}

