using UnityEngine;

public class SelfCast : SpawnedSpell
{
    public byte VFXCode;
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        if(VFXCode != ControlCodes.VFX_None)
        {
            ComponentRegister.Spawner.SpawnVFX(VFXCode, _casterReference.transform);
        }
    }
}
