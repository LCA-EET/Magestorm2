using UnityEngine;
public class Potion : Trigger
{
    public byte PotionType;
    public Rigidbody RigidBody;
    private float _elapsed, _period;

    protected override void Awake()
    {
        _period = 2 * Mathf.PI;
    }
    public override void EnterAction()
    {
        if (Game.PCAvatar.IsAlive)
        {
            Game.SendInGameBytes(InGame_Packets.TakePotionPacket(PotionType));
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        _elapsed += Time.deltaTime;
        transform.Translate(0, (Mathf.Sin(_elapsed) * 0.25f) - transform.position.y, 0);
        if(_elapsed > _period)
        {
            _elapsed =  0;
        }
    }
}
