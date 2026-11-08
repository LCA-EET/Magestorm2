using UnityEngine;

namespace Magus
{
    public class Wall : SpawnedSpell
    {

        public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
        {
            base.InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
            Match.AddWall(_castID, this);
        }
        public void DestroyWall()
        {
            Destroy(gameObject);
        }
        
    }

}
