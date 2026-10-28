using System;
using TMPro;
using UnityEngine;
public class PingDisplay : MonoBehaviour 
{
    private PeriodicAction _ping;
    private long _timeSent, _timeOfReceipt;
    public TMP_Text MillisecondText;
    private void Awake()
    {
        ComponentRegister.PingDisplay = this;
        _ping = new PeriodicAction(1.0f, SendPingRequest, null);
        MillisecondText.text = "0 ms";
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        _ping.ProcessAction(Time.deltaTime);
    }

    private void SendPingRequest()
    {
        _timeSent = DateTime.Now.Ticks;
        Game.SendInGameBytes(InGame_Packets.PingRequestPacket());
    }

    public void ShowPing()
    {
        _timeOfReceipt = DateTime.Now.Ticks;
        long elapsed = (_timeOfReceipt - _timeSent) / TimeSpan.TicksPerMillisecond;
        MillisecondText.text = elapsed + " ms";
    }
}
