using UnityEngine;

public class Devour : SpawnedSpell
{
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference);
        Debug.Log("Devour.Initialize");
        RaycastHit hitInfo;
        if (SharedFunctions.CastForward(Camera.main.transform, LayerManager.DeadPlayerLayerMask, 5.0f, out hitInfo))
        {
            if(hitInfo.collider != null)
            {
                Avatar deadPlayer = hitInfo.collider.GetComponent<Avatar>();
                ComponentRegister.Spawner.SpawnVFX(VFXCode.Banish, deadPlayer.transform.position);
                Debug.Log("Casting Team = " + _castingTeam);
                Debug.Log("Dead Player Team = " + deadPlayer.PlayerTeam);
                if(deadPlayer.PlayerTeam != castingTeam || castingTeam == Team.Neutral)
                {
                    Game.SendInGameBytes(InGame_Packets.DevourPacket(_castID, deadPlayer.PlayerID));
                }
            }
        }
    }
}
