using System;
using UnityEngine;
public class Sigil : SpawnedSpell, ITrigger
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
    }
    public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.InitializeNoCaster(castingTeam, castID, parent, spellReference);
        Match.AddSigil(castID, this);
    }
    public virtual void EnterAction()
    {
        if (Game.PCAvatar.IsAlive)
        {
            ReportTrigger();
        }
    }
    protected void ReportTrigger()
    {
        Game.SendInGameBytes(InGame_Packets.TriggeredSigilPacket(_castID));
    }
    public void ExitAction()
    {
        return;
    }

    public int GetTriggerID()
    {
        return -1;
    }

    public bool HasEntered()
    {
        return false;
    }

    public bool HasExited()
    {
        return false;
    }
    public void DestroySigil()
    {
        Destroy(gameObject);
    }
}
