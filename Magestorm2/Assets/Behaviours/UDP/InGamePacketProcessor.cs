using UnityEngine;

public class InGamePacketProcessor : UDPProcessor
{
    private int _listeningPort;
    private UDPGameClient _udp;
    private bool _checking;

    private void Awake()
    {
        ComponentRegister.InGamePacketProcessor = this;
        Init((int)SharedFunctions.Params[2]);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
\\
}
