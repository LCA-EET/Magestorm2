using UnityEngine;
public class Wall : SpawnedSpell
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        Match.AddWall(_castID, this);
    }
    public void DestroyWall()
    {
        Destroy(gameObject);
    }
}
