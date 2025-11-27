using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;

    private float _jumpSpeed = 6.0f;
    private float gravityValue = 9.81f;
    private float _lateralSpeed = 0.0f;
    private float _maxLateralSpeed = 3.0f;
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
    private bool _grounded = false;
    private bool _running = false;

    private Vector3 _priorPosition;
    private RaycastHit _hitInfo;
    private PC _pc;
   // private float _maxVerticalSpeed = 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _priorPosition = transform.position;
        Controller = GetComponent<CharacterController>();
        ComponentRegister.PlayerTransform = transform;
        ComponentRegister.PlayerMovement = this;
        ComponentRegister.PlayerController = Controller;
       
    }
    public void SetPC(PC pc)
    {
        _pc = pc;
    }
    private void LivingMovement()
    {
        float forwardAcceleration = _forwardAcceleration;
        float lateralAcceleration = _lateralAcceleration;
        float maxForwardSpeed = _maxForwardSpeed;
        float maxLateralSpeed = _maxLateralSpeed;

        if (InputControls.Run && _pc.CurrentStamina > 0)
        {
            _running = true;
            forwardAcceleration *= 3;
            maxForwardSpeed *= 3;
        }

        bool moving = MoveAlongAxes(ref _lateralSpeed, ref _forwardSpeed, maxLateralSpeed, maxForwardSpeed, lateralAcceleration, forwardAcceleration);

        if (_running && moving)
        {
            _pc.UseStamina(Time.deltaTime * 10.0f);
        }
        if (!_running)
        {
            _pc.RegenStamina(Time.deltaTime, moving);
        }

        if (!_grounded)
        {
            Accelerate(ref _verticalSpeed, _maxVerticalSpeed, -1.0f, gravityValue);
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        else if (InputControls.Jump && _grounded)
        {
            _verticalSpeed = _verticalSpeed + _jumpSpeed;
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
            _midJump = true;
        }

        UpdateGroundedStatus();
    }
    private void DeadMovement()
    {
        float forwardAcceleration = _forwardAcceleration * 0.5f;
        float lateralAcceleration = _lateralAcceleration * 0.5f;
        float maxForwardSpeed = _maxForwardSpeed * 0.5f;
        float maxLateralSpeed = _maxLateralSpeed * 0.5f;

        MoveAlongAxes(ref _lateralSpeed, ref _forwardSpeed, maxLateralSpeed, maxForwardSpeed, lateralAcceleration, forwardAcceleration);        
    }
    private bool MoveAlongAxes(ref float lateralSpeed, ref float forwardSpeed, float maxLateralSpeed, float maxForwardSpeed, float lateralAcceleration, float forwardAcceleration)
    {
        bool xAxisInput = MoveAlongAxis(ref _lateralSpeed, maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, lateralAcceleration, SpeedModifier);
        bool zAxisInput = MoveAlongAxis(ref _forwardSpeed, maxForwardSpeed, transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, SpeedModifier);
        return xAxisInput || zAxisInput;
    }
    void Update()
    {
        if (!_pc.JoinedMatch)
        {
            return;
        }
        _running = false;
        if (_pc.IsAlive)
        {
            LivingMovement();
        }
        else
        {
            DeadMovement();
        }
    }
    private void UpdateGroundedStatus()
    {
        bool priorState = _grounded;
        _grounded = isGrounded(out _hitInfo);
        if (_grounded)
        {
            _midJump = false;
            _verticalAcceleration = 0.0f;
            _verticalSpeed = 0.0f;
            _distanceTravelled += Vector3.Distance(transform.position, _priorPosition);
            _priorPosition = transform.position;
            PlayStepSound();
        }
        if (priorState != _grounded)
        {
            Debug.Log("Grounded: " + _grounded);
        }
    }
    
    private void PlayStepSound()
    {
        if(_distanceTravelled - _distanceTravelledSinceLastStep > 2.0f)
        {
            _distanceTravelledSinceLastStep = _distanceTravelled;
            if(_hitInfo.collider != null)
            {
                Debug.Log("Standing On: " + _hitInfo.collider.gameObject.name);
                Surface standingOn = _hitInfo.collider.gameObject.GetComponent<Surface>();
                if (standingOn != null)
                {
                    if (_running)
                    {
                        ComponentRegister.PC.PlaySFX(standingOn.FootstepClip);
                        Debug.Log("Play Footstep");
                    }
                    else
                    {
                        Debug.Log("Not Running");
                    }
                }
            }
            
        }
    }

    private bool isGrounded(out RaycastHit hitInfo)
    {
        return _pc.DownwardCaster.CastForward(LayerManager.SurfaceMask, 0.1f, out hitInfo);
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
   
    private bool MoveAlongAxis(ref float speed, float maxSpeed, Vector3 directionVector, InputControl negative, InputControl positive, float acceleration, float speedModifier)
    {
        bool movementInput = false;
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
            movementInput = true;
        }
        Accelerate(ref speed, maxSpeed, directionFactor, acceleration);
        Controller.Move(directionVector * speed * Time.deltaTime * speedModifier);
        return movementInput;
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
