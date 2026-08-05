using UnityEngine;

public class HitScan : OutwardCast
{
    private Vector3 _start, _end;
    private PeriodicAction _action;
    public float AnimationTimer;
    public HitScanAnimator Animator;
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        _start = _casterReference.transform.position;
        _end = ByteUtils.BytesToVector3(payload, 0);
        transform.position = _end;
    }
    private void Awake()
    {
        _action = AnimationTimer > 0 ? new PeriodicAction(AnimationTimer, DestroyAnimator, null) : null;
    }
    private void DestroyAnimator()
    {
        if(Animator != null)
        {
            Destroy(Animator.gameObject);
        }
    }
    public void Start()
    {
        
        if(Animator != null)
        {
            Animator.AnimateHitScan(this);
        }
    }
    public Vector3 StartPosition
    {
        get { return _start; }
    }

    public Vector3 EndPosition
    {
        get { return _end; }
    }
    public override void Update()
    {
        base.Update();
        if(_action != null)
        {
            _action.ProcessAction(Time.deltaTime);
        }
    }

}
