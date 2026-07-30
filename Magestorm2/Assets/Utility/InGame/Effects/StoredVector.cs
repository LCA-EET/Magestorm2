using System;
using UnityEngine;

public class StoredVector
{
    private long _creationTime, _expiration;
    private Vector3 _vector;
    public StoredVector(Vector3 vector)
    {
        _vector = vector;
        _creationTime = DateTime.Now.Ticks;
        _expiration = _creationTime + 60000;
    }

    public bool IsExpired(long currentTime)
    {
        return currentTime >= _expiration;
    }

    public Vector3 Vector
    {
        get { return _vector; }
    }
}
