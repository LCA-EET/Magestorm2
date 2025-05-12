using UnityEngine;

public class UIJoinMatch : MonoBehaviour
{
    private void Awake()
    {
        Game.SendBytes(Packets.MatchDetailsPacket((MatchEntry)SharedFunctions.Params[0]));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
