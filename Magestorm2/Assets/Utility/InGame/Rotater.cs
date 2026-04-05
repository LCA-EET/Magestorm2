using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float RotationPerSecond;
    public bool xAxis, yAxis, zAxis;

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.Rotate(xAxis ? RotationPerSecond * deltaTime : 0, yAxis ? RotationPerSecond * deltaTime : 0, zAxis ? RotationPerSecond * deltaTime : 0);
    }

}
