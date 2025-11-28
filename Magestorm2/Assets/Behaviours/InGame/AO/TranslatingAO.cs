using UnityEngine;

public class TranslatingAO : ActuatingAO
{
    public GameObject EndPosition;
    protected override void Start()
    {
        base.Start();
        _default = ActuatingObject.transform.position;
        _end = EndPosition.transform.position;
        _a = _default;
        _b = _end;
        _actuationTime = Vector3.Distance(_a, _b) / ActuationSpeed;
    }
    protected override void Update()
    {
        base.Update();
        if (_resetCountDown)
        {
            _resetCountdownPA.ProcessAction(Time.deltaTime);
        }
        if (_actuating)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _actuationElapsed, _actuationTime, _a, _b, ActuatingObject.transform))
            {
                _actuating = false;
                if (ActuatingObject.transform.position == _end)
                {
                    _resetCountDown = true;
                }
            }
        }
    }
    protected override void ApplyStateChange(bool force)
    {
        if (force)
        {
            ActuatingObject.transform.position = _end;
        }
        else
        {
            base.ApplyStateChange(force);
            _a = _currentState == 0 ? _end : _default;
            _b = _currentState == 0 ? _default : _end;
        }
            
    }
}
