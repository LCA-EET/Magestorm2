using UnityEngine;

public class LeyInfluencer : Trigger
{
    public SphereCollider SphereCollider;
    private float _influenceRadius;

    private BiasableTrigger _owner;
    private byte _key;
    private byte _power;
    public void AssignOwner(BiasableTrigger owner, byte power, byte key)
    {
        _key = key;
        _triggerType = TriggerType.LeyInfluencer;
        _owner = owner;
        power = _power;
        _influenceRadius = SphereCollider.radius;
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
        float distanceFactor = 1.0f - (distance / _influenceRadius);
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
