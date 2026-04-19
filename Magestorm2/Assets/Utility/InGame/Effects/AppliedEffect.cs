using UnityEngine;

public class AppliedEffect : MonoBehaviour
{
    public byte EffectCode;
    public VFXCode VFXCode;
    private Avatar _appliedTo, _applier;
    private float _duration;
    private byte _degree;
    private GameObject _vfxContainer;
    protected bool _appliedToPlayer;
    public void Initialize(Avatar applier, float duration, byte degree)
    {
        _applier = applier;
        _duration = duration;
        _degree = degree;
    }
    public bool Tick(float deltaTime)
    {
        _duration -= deltaTime;
        return _duration <= 0;
    }
    public virtual void ApplyEffect(Avatar appliedTo)
    {
        _appliedTo = appliedTo;
        _appliedToPlayer = SharedFunctions.IsPlayerAvatar(_appliedTo);
        if (!_appliedToPlayer)
        {
            ComponentRegister.Spawner.SpawnVFX(VFXCode, _appliedTo.transform, ref _vfxContainer);
        }
    }
    public void DestroyVFX()
    {
        if (_vfxContainer != null)
        {
            Destroy(_vfxContainer);
        }
    }
    public virtual void ReverseEffect()
    {
        DestroyVFX();
    }

    public float TimeRemaining
    {
        get { return _duration; }
    }
}
