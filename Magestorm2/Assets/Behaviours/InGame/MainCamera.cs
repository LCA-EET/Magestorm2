using UnityEngine;
public class MainCamera : MonoBehaviour 
{
    public Camera Camera, MinimapCamera;

    private bool _isShaking;
    private Vector3 _shakeVector, _preRenderPosition,  _shakeStrength;
    private AnimationCurve _shakeCurve;
    private float _shakeElapsed, _priorShake, _shakeDuration, _shakeFrame;
    public float GlobalShakeMultiplier = 1.0f;
    private void Awake()
    {
        ComponentRegister.MainCamera = this;
    }
    public void Start()
    {
        
        if(ComponentRegister.PC.CharacterClass == PlayerClass.Cleric)
        {
            Debug.Log("Changing culling masks");
            Camera.cullingMask |= LayerManager.DeadPlayerLayerMask;
            MinimapCamera.cullingMask |= LayerManager.DeadPlayerLayerMask;
        }
    }
    private void Update()
    {
        if (_isShaking)
        {
            _shakeElapsed += Time.deltaTime;
            if(_shakeElapsed - _priorShake >= _shakeFrame)
            {
                _priorShake = _shakeElapsed;
                ComputeShakeVector(_shakeElapsed);
                Camera.transform.localPosition += _shakeVector;
                //Camera.transform.localPosition += Camera.transform.rotation * _shakeVector;
                //Camera.transform.localEulerAngles += Camera.transform.rotation * _shakeVector;
            }
            if (_shakeElapsed >= _shakeDuration)
            {
                StopShake();
            }
        }
    }
    private void ComputeShakeVector(float delta)
    {
        Vector3 random = new Vector3(Random.value, Random.value, Random.value);
        Vector3 shakeVec = Vector3.Scale(random, _shakeStrength) * (Random.value > 0.5f ? -1 : 1);
        _shakeVector = shakeVec * _shakeCurve.Evaluate(delta) * GlobalShakeMultiplier;
    }
    public void Shake()
    {
        Shake(Random.Range(0.05f, 0.1f));
    }
    public void Shake(float shakeStrength)
    {
        _shakeStrength = new Vector3(shakeStrength, shakeStrength, shakeStrength);
        _preRenderPosition = Camera.transform.localPosition;
        if (_isShaking)
        {
            StopShake();
        }
        _shakeDuration = Random.Range(0.5f, 1.0f);
        _shakeCurve = AnimationCurve.Linear(0, 1, _shakeDuration, 0);
        _shakeFrame = _shakeDuration / 20.0f;
        _isShaking = true;
    }

    private void StopShake()
    {
        _isShaking = false;
        _shakeElapsed = _priorShake = 0.0f;
        Camera.transform.localPosition = _preRenderPosition;
        //Camera.transform.localEulerAngles = _preRenderPosition;
    }
}
