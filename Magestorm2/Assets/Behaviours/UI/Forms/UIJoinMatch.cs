using UnityEngine;

public class UIJoinMatch : MonoBehaviour
{
    public TeamPlayerList ChaosPlayerList;
    public TeamPlayerList BalancePlayerList;
    public TeamPlayerList OrderPlayerList;
    private void Awake()
    {
        ComponentRegister.UIJoinMatch = this;
        //Game.SendBytes(Packets.MatchDetailsPacket((MatchEntry)SharedFunctions.Params[0]));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillPlayers(RemotePlayerData[] chaos, RemotePlayerData[] balance, RemotePlayerData[] order)
    {
        ChaosPlayerList.FillTeam(chaos);
        BalancePlayerList.FillTeam(balance);
        OrderPlayerList.FillTeam(order);
    }
}
