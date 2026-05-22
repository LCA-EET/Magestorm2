using UnityEngine;
public class Sigil : SpawnedSpell, ITrigger
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        Match.AddSigil(castID, this);
    }
    public void EnterAction()
    {
        if (Game.PCAvatar.IsAlive)
        {
            Game.SendInGameBytes(InGame_Packets.TriggeredSigilPacket(_castID));
        }
    }

    public void ExitAction()
    {
        throw new System.NotImplementedException();
    }

    public int GetTriggerID()
    {
        throw new System.NotImplementedException();
    }

    public bool HasEntered()
    {
        throw new System.NotImplementedException();
    }

    public bool HasExited()
    {
        throw new System.NotImplementedException();
    }
    public void DestroySigil()
    {
        Destroy(gameObject);
    }
}
