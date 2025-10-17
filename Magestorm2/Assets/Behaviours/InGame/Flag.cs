using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Flag : Trigger
{
    public Team Team;
    private Vector3 _worldLocation;

    private void Awake()
    {
        _worldLocation = transform.position;
    }

    public void FlagReturned()
    {
        transform.position = _worldLocation;
    }

    public void FlagDropped(Vector3 position)
    {
        transform.position = position;
    }

    public bool IsTaken()
    {
        return transform.position == _worldLocation;
    }

    public override void EnterAction()
    {
        if(IsTaken() && Team == MatchParams.MatchTeam)
        {
            Game.SendInGameBytes(InGame_Packets.FlagReturnedPacket((byte)Team));
        }
        if (!IsTaken() && Team != MatchParams.MatchTeam) 
        {
            Game.Send
        }
        base.EnterAction()
    }
}
