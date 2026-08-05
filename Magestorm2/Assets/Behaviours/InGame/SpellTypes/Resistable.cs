using UnityEngine;
public class Resistable : Bolt
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        _impactMask = LayerManager.MindImpactMask;
    }
    protected override void ReportHit()
    {
        Game.SendInGameBytes(InGame_Packets.ReportResistableHit(_castID));
    }
    protected override void SpawnImpactPrefab()
    {
        return;
    }
}
