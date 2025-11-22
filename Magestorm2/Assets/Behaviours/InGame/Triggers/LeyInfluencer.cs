using UnityEngine;

public class LeyInfluencer : Trigger
{
    public SphereCollider SphereCollider;
    public float InfluenceRadius;

    private BiasableTrigger _owner;
    private byte _key;
    private byte _power;
    public void AssignOwner(BiasableTrigger owner, byte power, byte key)
    {
        _key = key;
        _triggerType = TriggerType.LeyInfluencer;
        _owner = owner;
        power = _power;
        SphereCollider.radius = InfluenceRadius;
    }
    
    public TriggerType OwningTriggerType
    {
        get
        {
            return _owner.TriggerType;
        }
    }
    public float GetLeyContribution()
    {
        if(_owner.BiasedToward == Team.Neutral)
        {
            return 0;
        }
        float distance = Vector3.Distance(transform.position, ComponentRegister.PC.transform.position);
        float distanceFactor = 1.0f - (distance / InfluenceRadius);
        return (distanceFactor * (_owner.BiasAmount / 100.0f)) * (_owner.BiasedToward == MatchParams.MatchTeam ? 1.0f : -1.0f);
    }
    public override void EnterAction()
    {
        ComponentRegister.PC.RegisterLeyInfluencer(_key, this);
        Debug.Log("Ley Influence Enter");
    }
    public override void ExitAction()
    {
        ComponentRegister.PC.DeregisterLeyInfluencer(_key);
        Debug.Log("Ley Influence Exit");
    }
}
