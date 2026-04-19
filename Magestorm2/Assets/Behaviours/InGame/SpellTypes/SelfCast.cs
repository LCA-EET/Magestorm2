using UnityEngine;

public class SelfCast : SpawnedSpell
{
    public VFXCode VFXCode;
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        if(VFXCode != VFXCode.None)
        {
            ComponentRegister.Spawner.SpawnVFX(VFXCode, _casterReference.transform);
        }
    }
}
