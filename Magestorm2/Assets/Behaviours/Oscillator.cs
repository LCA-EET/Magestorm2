using UnityEngine;

public class Oscillator : MonoBehaviour
{
    private float _distanceMoved;
    private float _xDirection = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _distanceMoved = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        float distanceToMove = _xDirection * deltaTime;
        transform.Translate(new Vector3(distanceToMove, 0.0f, 0.0f));
        _distanceMoved += Mathf.Abs(distanceToMove);
        if(_distanceMoved > 2.0f)
        {
            _distanceMoved = 0.0f;
            _xDirection *= -1;
        }
    }
}
