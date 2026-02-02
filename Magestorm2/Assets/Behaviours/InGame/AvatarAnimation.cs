using UnityEngine;

public class AvatarAnimation : MonoBehaviour{

    public RuntimeAnimatorController[] MaleAnimations;
    public RuntimeAnimatorController[] FemaleAnimations;

    private Animator _animator;
    private PeriodicAction _action;
    private bool _isDone, _male;
    private byte _nextAnimation, _currentAnimation;
    private int _animationID;
    public void Init(Animator animator, bool male)
    {
        _animator = animator;
        _isDone = false;
        _male = male;
        _nextAnimation = AnimationKeys.None;
        _action = new PeriodicAction(0.05f, CheckComplete, null);
    }
    public void SetAnimation(byte key, bool stopCurrent)
    {
        _isDone = false;
        if (stopCurrent)
        {
            if(_currentAnimation != key)
            {
                SwitchRTAC(key);
            }
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
    private void SwitchRTAC(byte key)
    {
        _animationID++;
        _currentAnimation = key;
        _animator.runtimeAnimatorController = _male ? MaleAnimations[key] : FemaleAnimations[key];
        //_animator.StartPlayback();
        Debug.Log("AnimationID: " + _animationID + ", " + key);
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
            _nextAnimation = AnimationKeys.None;
        }
    }
}

