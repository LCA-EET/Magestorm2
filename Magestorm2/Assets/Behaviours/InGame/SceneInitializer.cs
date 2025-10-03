using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    private void Awake()
    {
        Match.Init();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Teams.Init();
        Game.Init();
        Language.Init();
        LayerManager.Init();
        Debug.Log("Sending Match Joined Packet");
        Game.ChatMode = false;
        Game.MenuMode = false;
        ComponentRegister.InGamePacketProcessor.SendBytes(InGame_Packets.MatchJoinedPacket());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
