using System;
using UnityEngine;
public class Sigil : SpawnedSpell, ITrigger
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
    }
    public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
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
