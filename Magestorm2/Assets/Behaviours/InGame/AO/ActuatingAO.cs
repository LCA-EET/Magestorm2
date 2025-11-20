using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActuatingAO : ActivateableObject
{
    public GameObject ActuatingObject;
    public byte ActuationSpeed; // The distance between the start and end positions, divided by the ActuationSpeed, results in the time it will take to transition from one state to the next.
    protected Vector3 _default, _end;
    protected Vector3 _a, _b;
   
    protected bool _actuating;
    protected float _actuationTime;
    protected float _actuationElapsed;

    protected override void ApplyStateChange(bool force)
    {
        base.ApplyStateChange(force);
        _actuating = true;
    }
}
