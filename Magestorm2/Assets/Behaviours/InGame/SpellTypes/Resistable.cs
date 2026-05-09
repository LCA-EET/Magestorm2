using UnityEngine;
public class Resistable : Bolt
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        _impactMask = LayerManager.MindImpactMask;
    }
    protected override void SpawnImpactPrefab()
    {
        return;
    }
}
