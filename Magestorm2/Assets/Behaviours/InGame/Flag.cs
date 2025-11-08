using UnityEngine;

public class Flag : Trigger
{
    public Team Team;
    private Vector3 _worldLocation;

    private void Start()
    {
        if (!MatchParams.IncludeFlags)
        {
            Debug.Log("Destroying Flag " + Team);
            Destroy(gameObject);
        }
        else
        {
            _worldLocation = transform.position;
            FlagManager.Register(this);
        }
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

    public override void EnterAction()
    {
        if (ComponentRegister.PC.IsAlive)
        {
            if (!IsSafe() && (Team == MatchParams.MatchTeam))
            {
                Game.SendInGameBytes(InGame_Packets.FlagReturnedPacket((byte)Team));
            }
            if (Team != MatchParams.MatchTeam)
            {
                Game.SendInGameBytes(InGame_Packets.FlagTakenPacket((byte)Team));
            }
            if(IsSafe() && (Team == MatchParams.MatchTeam))
            {
                if(FlagManager.FlagHeldByPlayer != Team.Neutral)
                {
                    Game.SendInGameBytes(InGame_Packets.FlagCapturedPacket((byte)FlagManager.FlagHeldByPlayer));
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
