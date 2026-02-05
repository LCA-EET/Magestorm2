using UnityEngine;

public class AvatarAnimation : MonoBehaviour{

    public RuntimeAnimatorController[] MaleAnimations;
    public RuntimeAnimatorController[] FemaleAnimations;

    private Animator _animator;
    private PeriodicAction _action;
    private bool _isDone, _male, _priorMove;
    private byte _nextAnimation, _currentAnimation, _priorPosture;
    
    public void Init(Animator animator, bool male)
    {
        _animator = animator;
        _isDone = false;
        _male = male;
        _nextAnimation = AnimationKeys.None;
        _action = new PeriodicAction(0.05f, CheckComplete, null);
    }
    public void Animate(PMDByte pmd)
    {
        byte animationKey = AnimationKeys.None;
        byte currentPosture = pmd.Posture;
        bool currentlyMoving = pmd.IsMoving;
        if (currentlyMoving)
        {
            switch (currentPosture)
            {
                case Postures.Standing:
                    if (pmd.IsMovingForward)
                    {
                        animationKey = pmd.IsRunning ? AnimationKeys.Run : AnimationKeys.Walk_Forward;
                    }
                    else
                    {
                        animationKey = AnimationKeys.Walk_Backward;
                    }
                    break;
                case Postures.Jump:
                    animationKey = AnimationKeys.Jump;
                    break;
                case Postures.Crouched:
                    animationKey = pmd.IsMovingForward ? AnimationKeys.CrouchWalk_Forward : AnimationKeys.CrouchWalk_Backward;
                    break;
            }
        }
        else
        {
            switch (currentPosture)
            {
                case Postures.Standing:
                    animationKey = AnimationKeys.Idle_Standing;
                    break;
                case Postures.Jump:
                    animationKey = AnimationKeys.Jump;
                    break;
                case Postures.Crouched:
                    animationKey = AnimationKeys.Idle_Crouching;
                    break;
                
            }
        }
        SetAnimation(animationKey, currentlyMoving != _priorMove || currentPosture != _priorPosture);
        _priorPosture = currentPosture;
        _priorMove = pmd.IsMoving;
    }
    public void SetAnimation(byte key, bool stopCurrent)
    {
        if(key == AnimationKeys.None)
        {
            return;
        }
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
        _currentAnimation = key;
        _animator.runtimeAnimatorController = _male ? MaleAnimations[key] : FemaleAnimations[key];
        //Debug.Log("AnimationID: " + _animationID + ", " + key);
    }
    private void CheckComplete()
    {
        _isDone = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        if (_isDone)
        {
            if(_nextAnimation == AnimationKeys.None)
            {
                if(_priorPosture == Postures.Standing)
                {
                    _nextAnimation = AnimationKeys.Idle_Standing;
                }
                else if(_priorPosture == Postures.Crouched)
                {
                    _nextAnimation = AnimationKeys.Idle_Crouching;
                }
            }
            SetAnimation(_nextAnimation, true);
            _nextAnimation = AnimationKeys.None;
        }
    }
}

