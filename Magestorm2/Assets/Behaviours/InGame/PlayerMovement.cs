using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;

    private float _jumpSpeed = 6.0f;
    private float gravityValue = 9.81f;
    private float _lateralSpeed = 0.0f;
    private float _maxLateralSpeed = 4.0f;
    private float _lateralAcceleration = 6.0f;
    private float _forwardSpeed = 0.0f;
    private float _maxForwardSpeed = 2.0f;
    private float _forwardAcceleration = 6.0f;
    private float _verticalSpeed = 0.0f;
    private float _maxVerticalSpeed = 30.0f;
    private float _verticalAcceleration = 0.0f;
    private float _distanceTravelled = 0.0f;
    private float _distanceTravelledSinceLastStep = 0.0f;

    private bool _positionChanged = false;
    private bool _midJump = false;

    private Vector3 _priorPosition;
    private RaycastHit _hitInfo;
    
   // private float _maxVerticalSpeed = 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _priorPosition = transform.position;
        _controller = GetComponent<CharacterController>();
        ComponentRegister.PlayerTransform = transform;
        ComponentRegister.PlayerMovement = this;
        ComponentRegister.PlayerController = _controller;
       
    }
    void Update()
    {
        bool grounded = isGrounded(out _hitInfo);
        if (_priorPosition != transform.position)
        {
            _distanceTravelled += Vector3.Distance(transform.position, _priorPosition);
            _priorPosition = transform.position;
            _positionChanged = true;
            if (grounded)
            {
                PlayStepSound();
            }
        }
        else
        {
            _positionChanged = false;
        }
        if (grounded)
        {
            _verticalAcceleration = 0.0f;
            _verticalSpeed = 0.0f;
        }
        float speedModifier = SpeedModifier;
        MoveAlongAxis(ref _lateralSpeed, _maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, _lateralAcceleration, speedModifier);
        float forwardAcceleration = _forwardAcceleration;
        float maxForwardSpeed = _maxForwardSpeed;
        if (InputControls.Run)
        {
            forwardAcceleration *= 3;
            maxForwardSpeed *= 3;
        }
        MoveAlongAxis(ref _forwardSpeed, maxForwardSpeed, transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, speedModifier);
        if (InputControls.Jump && grounded)
        {
            _verticalSpeed = _verticalSpeed + _jumpSpeed;
            _controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
            _midJump = true;
        }
        else
        {
            if (!grounded)
            {
                Accelerate(ref _verticalSpeed, _maxVerticalSpeed, -1.0f, gravityValue);
                _controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
            }
            if (grounded)
            {
                _midJump = false;
            }
        }
    }
    private void PlayStepSound()
    {
        if(_distanceTravelled - _distanceTravelledSinceLastStep > 2.0f)
        {
            _distanceTravelledSinceLastStep = _distanceTravelled;
            Debug.Log("Standing On: " + _hitInfo.collider.gameObject.name);
            Surface standingOn = _hitInfo.collider.gameObject.GetComponent<Surface>();
            if (standingOn != null && InputControls.Run)
            {
                ComponentRegister.Player.PlayAudioClip(standingOn.FootstepClip);
            }
            else
            {
                Debug.Log("Null Surface");
            }
        }
    }
    private bool isGrounded()
    {
        return SharedFunctions.CastDown(transform, LayerManager.SurfaceMask, 2.0f);
    }
    private bool isGrounded(out RaycastHit hitInfo)
    {
        return SharedFunctions.CastDown(transform, LayerManager.SurfaceMask, 2.0f, out hitInfo);
    }
    private void Accelerate(ref float speed, float maxSpeed, float directionFactor, float acceleration)
    {
        speed += Time.deltaTime * directionFactor * acceleration;
        if(speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        if(speed < -maxSpeed)
        {
            speed = -maxSpeed;
        }
    }
   
    private void MoveAlongAxis(ref float speed, float maxSpeed, Vector3 directionVector, InputControl negative, InputControl positive, float acceleration, float speedModifier)
    {
        float directionFactor = 0.0f;
        if (_midJump)
        {
            directionFactor = 0; // maintain the previous speed.
        }
        else if ((!InputControls.IsPressed(negative) && !InputControls.IsPressed(positive)) ||
            InputControls.IsPressed(negative) && InputControls.IsPressed(positive))
        {
            directionFactor = speed > 0.0f ? -1.0f : 1.0f;
        }
        else if (InputControls.IsPressed(negative) || InputControls.IsPressed(positive))
        {
            directionFactor = InputControls.IsPressed(negative) ? -1.0f : 1.0f;
        }
        Accelerate(ref speed, maxSpeed, directionFactor, acceleration);
        _controller.Move(directionVector * speed * Time.deltaTime * speedModifier);
    }

    private bool MovingOnMultipleAxes
    {
        get{
            if ((InputControls.Forward || InputControls.Backward) && (InputControls.StrafeLeft || InputControls.StrafeRight))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    private float SpeedModifier
    {
        get
        {
            float toReturn = 1.0f;
            if (MovingOnMultipleAxes)
            {
                toReturn = toReturn * 0.7071f;
            }
            // haste, slow, etc.
            return toReturn;
        }
    }
        
    public float DistanceTravelled
    {
        get
        {
            return _distanceTravelled;
        }
    }
    public bool PositionChanged
    {
        get
        {
            return _positionChanged;
        }
        set
        {
            _positionChanged = value;
        }
    }
}
