using TMPro.EditorUtilities;
using UnityEngine;
public class RotatingAO : ActuatingAO 
{
    public GameObject EndRotation;
    protected override void Start()
    {
        _default = ActuatingObject.transform.localEulerAngles;
        _end = EndRotation.transform.localEulerAngles;
        //Debug.Log("Default Rotation: " + _default + ", End Rotation: " + _end);
        base.Start();
    }
    protected override void Update()
    {
        base.Update();

        if (_actuating)
        {
            Vector3 calculatedLerp = SharedFunctions.CalculateVector3Lerp(ref _actuationElapsed, _actuationTime, _a, _b);
            //Debug.Log("AE: " + _actuationElapsed + ", AT: " + _actuationTime + ", " + _a + ", " + _b + ", calculated: " + calculatedLerp);
            SharedFunctions.ApplyVector3Lerp(calculatedLerp, ActuatingObject.transform, true, false);
            if (_actuationElapsed == 0) // this is 0 when it is reset by the CalculateVector3Lerp function above. It is reset when elapsed >= actuationTime.
            {
                _actuating = false;
                if (ActuatingObject.transform.localEulerAngles == _end)
                {
                    _resetCountDown = true;
                    //Debug.Log("RCD is TRUE.");
                }
            }
        }
    }

    protected override void ApplyStateChange(bool force)
    {
        if (force)
        {
            ActuatingObject.transform.localEulerAngles = _end;
        }
        else
        {
            base.ApplyStateChange(force);
        }
    }
}
