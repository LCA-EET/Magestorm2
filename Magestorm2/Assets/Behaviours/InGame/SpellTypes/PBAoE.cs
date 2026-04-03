using UnityEngine;

public class PBAoE : SpawnedSpell
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        bool reportHit = false;
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        ComponentRegister.Spawner.SpawnMarker(transform.position, 1.0f);
        if(SharedFunctions.IsPlayerInRadius(transform.position, _spellReference.EffectRadius))
        {
            if (_spellReference.IsFriendly && castingTeam == MatchParams.MatchTeam)
            {
                reportHit = true;
            }
            if (!_spellReference.IsFriendly && casterID != MatchParams.IDinMatch)
            {
                if(MatchParams.MatchType == ControlCodes.MatchTypes_FreeForAll)
                {
                    reportHit = true;
                }
                else
                {
                    if(castingTeam != MatchParams.MatchTeam)
                    {
                        reportHit = true;
                    }
                }
            }
        }
        if (reportHit)
        {
            Game.SendInGameBytes(InGame_Packets.ReportHitPacket(castID));
        }
    }

}
