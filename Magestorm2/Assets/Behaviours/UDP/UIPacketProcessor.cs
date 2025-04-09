using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class UIPacketProcessor : MonoBehaviour
{
    private int _listeningPort;
    private UDPGameClient _udp;
    private bool _checking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_listeningPort > 0)
        {
            if (!_checking)
            {
                _checking = true;
                Debug.Log("Checking for new packets.");
            }
            if (_udp.HasPacketsPending)
            {
                Debug.Log("Packets pending!");
                List<byte[]> toProcess = _udp.PacketsReceived();
                foreach (byte[] decryptedPayload in toProcess)
                {
                    OpCode_Receive opCode = (OpCode_Receive)decryptedPayload[0];
                    Debug.Log("OpCode Received: " + opCode);
                    switch (opCode)
                    {
                        case OpCode_Receive.AccountCreationFailed:
                            MessageBox(26);
                            break;
                        case OpCode_Receive.AccountCreated:
                            MessageBox(24);
                            break;
                        case OpCode_Receive.AccountAlreadyExists:
                            MessageBox(25);
                            break;
                        case OpCode_Receive.LogInFailed:
                            MessageBox(27);
                            break;
                        case OpCode_Receive.LogInSucceeded:
                            MessageBox(28);
                            break;
                    }
                }
            }
        }
    }
    private void MessageBox(int stringReference)
    {
        Game.MessageBox(Language.GetBaseString(stringReference), ComponentRegister.UILoginForm.gameObject);
    }
    public void Init(int port)
    {
        Debug.Log("Initialized UI packet listener on port: " + port);
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
    }
}
