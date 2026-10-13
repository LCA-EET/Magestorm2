using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class Flag : Trigger
{
    private Team _team;
    private Vector3 _worldLocation;
    public GameObject ChaosFlag;
    public GameObject BalanceFlag;
    public GameObject OrderFlag;
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
            _worldLocation = transform.position;
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
    }
    public void FlagReturned()
    {
        gameObject.SetActive(true);
        transform.position = _worldLocation;
    }

    public bool IsSafe()
    {
        return transform.position == _worldLocation;
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
                if (!IsSafe() && (Team == MatchParams.MatchTeam))
                {
                    Game.SendInGameBytes(InGame_Packets.FlagReturnedPacket((byte)Team));
                }
                if (Team != MatchParams.MatchTeam)
                {
                    Game.SendInGameBytes(InGame_Packets.FlagTakenPacket((byte)Team));
                }
                if (IsSafe() && (Team == MatchParams.MatchTeam))
                {
                    if (FlagManager.FlagHeldByPlayer != Team.Neutral)
                    {
                        Game.SendInGameBytes(InGame_Packets.FlagCapturedPacket((byte)FlagManager.FlagHeldByPlayer));
                    }
                }
            }
        }
    }

    public void Reposition(Vector3 worldPosition)
    {
        gameObject.SetActive(true);
        transform.position = worldPosition;
    }
}
