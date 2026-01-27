using UnityEngine;

public class AvatarAnimation : MonoBehaviour{

    public RuntimeAnimatorController[] MaleAnimations;
    public RuntimeAnimatorController[] FemaleAnimations;

    private Animator _animator;
    private PeriodicAction _action;
    private bool _isDone, _male;
    private byte _nextAnimation;

    public void Init(Animator animator, bool male)
    {
        _animator = animator;
        _isDone = false;
        _male = male;
        _nextAnimation = AnimationKeys.None;
        _animator.runtimeAnimatorController = _male ? MaleAnimations[AnimationKeys.Idle] : FemaleAnimations[AnimationKeys.Idle];
        _action = new PeriodicAction(0.05f, CheckComplete, null);
    }
    public void SetAnimation(byte key, bool stopCurrent)
    {
        _isDone = false;
        if (stopCurrent)
        {
            _animator.runtimeAnimatorController = _male ? MaleAnimations[key] : FemaleAnimations[key];
            _animator.StartPlayback();
        }
        else
        {
            _nextAnimation = key;
        }
    }
    public void SetElapsed(float deltaTime)
    {
        _action.ProcessAction(deltaTime);
    }

    private void CheckComplete()
    {
        _isDone = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        if (_isDone)
        {
            if(_nextAnimation == AnimationKeys.None)
            {
                _nextAnimation = AnimationKeys.Idle;
            }
            SetAnimation(_nextAnimation, true);
        }
    }
}

