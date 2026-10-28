using UnityEngine;

public class Flag : Trigger
{
    private Team _team;
    public GameObject ChaosFlag;
    public GameObject BalanceFlag;
    public GameObject OrderFlag;
    public bool IsReturned = true;
    private GameObject _assigned;
    public Team Team
    {
        get { return _team; }
    }
    public void AssignToTeam(Team team)
    {
        
        if (!MatchParams.IncludeFlags)
        {
            //Debug.Log("Destroying Flag " + Team);
            Destroy(gameObject);
        }
        else
        {
            HideAll();
            _team = team;
            switch (Team)
            {
                case Team.Chaos:
                    _assigned = ChaosFlag;      
                    break;
                case Team.Balance:
                    _assigned = BalanceFlag;
                    break;
                case Team.Order:
                    _assigned = OrderFlag;
                    break;
            }
            _assigned.SetActive(true);
            FlagManager.Register(this);
        }
    }
    private void HideAll()
    {
        ChaosFlag.SetActive(false);
        BalanceFlag.SetActive(false);
        OrderFlag.SetActive(false);
    }
    public void FlagTaken()
    {
        gameObject.SetActive(false);
        IsReturned = false;
    }
    public void FlagReturned()
    {
        gameObject.SetActive(true);
        IsReturned = true;
    }

    public override void ExitAction()
    {
        Debug.Log("Flag Exit Action");
        if (FlagManager.FlagJustDropped)
        {
            FlagManager.FlagJustDropped = false;
        }
    }

    public override void EnterAction()
    {
        Debug.Log("Flag Enter Action");
        if (Game.PCAvatar.IsAlive)
        {
            if (!FlagManager.FlagJustDropped)
            {
                if (IsReturned && (Team == MatchParams.MatchTeam))
                {
                    if (Game.PCAvatar.HeldFlag != Team.Neutral)
                    {
                        Debug.Log("Sending flag captured packet.");
                        Game.SendInGameBytes(InGame_Packets.FlagCapturedPacket((byte)Game.PCAvatar.HeldFlag));
                    }
                }
                else if (!IsReturned && (Team == MatchParams.MatchTeam))
                {
                    Debug.Log("Sending flag returned packet.");
                    Game.SendInGameBytes(InGame_Packets.FlagReturnedPacket((byte)Team));
                }
                else if (Team != MatchParams.MatchTeam && Game.PCAvatar.HeldFlag == Team.Neutral)
                {
                    Debug.Log("Sending flag taken packet.");
                    Game.SendInGameBytes(InGame_Packets.FlagTakenPacket((byte)Team));
                }
            }
            else
            {
                Debug.Log("FlagJustDropped is true.");
            }
        }
    }

    public void Reposition(Vector3 worldPosition)
    {
        gameObject.SetActive(true);
        transform.position = worldPosition;
    }
}
