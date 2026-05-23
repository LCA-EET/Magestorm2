using UnityEngine;

public class HarmfulSigil : Sigil
{
    public override void EnterAction()
    {
        if (Game.PCAvatar.IsAlive)
        {
            Debug.Log("A");
            if(_castingTeam == Team.Neutral)
            {
                Debug.Log("B");
                if (_casterID != MatchParams.IDinMatch)
                {
                    Debug.Log("C");
                    ReportTrigger();
                }
            }
            else if(_castingTeam != MatchParams.MatchTeam)
            {
                Debug.Log("D");
                ReportTrigger();
            }
            else
            {
                Debug.Log("Casting team is " + _castingTeam.ToString());
            }
        }
    }
}
