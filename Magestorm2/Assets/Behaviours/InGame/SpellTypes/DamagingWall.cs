using UnityEngine;
public class DamagingWall : NonSolidWall
{
    protected PeriodicAction _damageTick;
    public override void Awake()
    {
        base.Awake();
        _damageTick = new PeriodicAction(1.0f, DamageTick, null);
    }
    public override void Update()
    {
        base.Update();
        if (Game.PCAvatar.IsAlive)
        {
            if (_entered && !_exited)
            {
                _damageTick.ProcessAction(Time.deltaTime);
            }
        }
    }

    private void DamageTick()
    {
        Debug.Log("Damage Tick.");
        Game.SendInGameBytes(InGame_Packets.ReportHitByWallPacket(_castID));
    }

}
