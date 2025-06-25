using System;
using System.Collections.Generic;
using System.Text;
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
                        case OpCode_Receive.BroadcastMessage:
                            ProcessBroadcastMessagePacket();
                            break;
                    }
                }
            }
        }
    }
    private void ProcessBroadcastMessagePacket()
    {
        int messageLength = BitConverter.ToInt32(_decrypted, 2);
        byte[] messageBytes = new byte[messageLength];
        Array.Copy(_decrypted, 6, messageBytes, 0, messageLength);
        string message = Encoding.UTF8.GetString(messageBytes);
        ComponentRegister.Notifier.DisplayNotification(message);
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

