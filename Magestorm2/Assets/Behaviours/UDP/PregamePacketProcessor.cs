using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public class PregamePacketProcessor : MonoBehaviour
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
                        case OpCode_Receive.CreationFailed:
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
                            HandleLogInSuccessfulPacket(decryptedPayload);
                            break;
                        case OpCode_Receive.ProhibitedLanguage:
                            MessageBox(29);
                            break;
                        case OpCode_Receive.AlreadyLoggedIn:
                            MessageBox(30);
                            break;
                        case OpCode_Receive.RemovedFromServer:
                            MessageBox(31);
                            _udp.StopListening();
                            Application.Quit();
                            break;
                        case OpCode_Receive.CharacterExists:
                            MessageBox(33);
                            break;
                    }
                }
            }
        }
    }
    private void MessageBox(int stringReference)
    {
        Game.MessageBox(Language.GetBaseString(stringReference));
    }
    public void Init(int port)
    {
        Debug.Log("Initialized UI packet listener on port: " + port);
        _listeningPort = port;
        _udp = UDPBuilder.GetClient(port);
    }

    private void HandleLogInSuccessfulPacket(byte[] decrypted)
    {
        int accountID = BitConverter.ToInt32(decrypted, 1);
        PlayerAccount.Init(accountID);
        if (decrypted.Length > 5)
        {
            byte numCharacters = decrypted[5];
            int charIndex = 0;
            int index = 6;
            while (charIndex < numCharacters)
            {
                int characterID = BitConverter.ToInt32(decrypted, index);
                index += 4;
                byte charClass = decrypted[index];
                index++;
                byte nameLength = decrypted[index];
                index++;
                string charname = Encoding.UTF8.GetString(decrypted, index, nameLength);
                index += nameLength;
                PlayerAccount.AddCharacter(characterID, charname, charClass, 1);
            }

        }
    }
}
