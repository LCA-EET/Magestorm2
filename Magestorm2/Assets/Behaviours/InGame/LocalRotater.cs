using UnityEngine;
public class LocalRotater : MonoBehaviour
{
    public int minXDegreesPerSecond, maxXDegreesPerSecond;
    public int minYDegreesPerSecond, maxYDegreesPerSecond;
    public int minZDegreesPerSecond, maxZDegreesPerSecond;

    private int _x, _y, _z;
    private void Awake()
    {
        _x = SharedFunctions.RandomInt(minXDegreesPerSecond, maxXDegreesPerSecond);
        _y = SharedFunctions.RandomInt(minYDegreesPerSecond, maxYDegreesPerSecond);
        _z = SharedFunctions.RandomInt(minZDegreesPerSecond, maxZDegreesPerSecond);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        float rX = deltaTime * _x;
        float rY = deltaTime * _y;
        float rZ = deltaTime * _z;

        transform.Rotate(rX, rY, rZ);
    }
}
