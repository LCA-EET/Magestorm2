using UnityEngine;

public class SlidingDoor : ActivateableObject
{
    public GameObject EndPosition;
    private Vector3 _defaultPosition, _endPosition;
    private Vector3 _a, _b;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _defaultPosition = transform.position;
        _endPosition = EndPosition.transform.position;
    }
    void Start()
    {
        RegisterObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (_actuating)
        {
            if(SharedFunctions.ProcessLerp(ref _actuationElapsed, _actuationTime, _a, _b, transform))
            {
                _actuating = false;
            }
        }
    }

    protected override void ApplyStateChange(bool force)
    {
        if (!force)
        {
            _actuating = true;
            _a = _currentState == 0 ? _endPosition : _defaultPosition;
            _b = _currentState == 0 ? _defaultPosition : _endPosition;
            if(ActuationAudio.clip != null)
            {
                ActuationAudio.Play();
            }
        }
        else
        {
            transform.position = _currentState == 0 ? _defaultPosition : _endPosition;
        }
    }
}
