using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;
    private PeriodicAction _reportMovement;
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
    private float _verticalAcceleration = 6.0f;
    private float _distanceTravelled = 0.0f;
    private float _distanceTravelledSinceLastStep = 0.0f;
    private float _positionLimit = 0.067f;
    private float _rotationLimit = 5f;
    private float _csElapsed = 0.0f;
    private float _csInterval = 0.33f;
    private float _controllerHeight, _controllerCrouchHeight;
    private int _prPacketID = 0;
    private Vector3 _controllerCenter, _controllerCrouchCenter, _cameraLocalPosition, _cameraCrouchedPosition;
    private Vector3 _moveCheck, _rotateCheck;
    private Vector3 _priorPosition, _priorRotation;

    private bool _positionChanged = false;
    private bool _midJump = false;
    private bool _grounded = false;
    private bool _running = false;
    private bool _csChanging = false;
    private bool _moving = false;
    private bool _priorMoving = false;
    private byte _postureCheck;
    private RaycastHit _hitInfo;
    private PC _pc;
    private byte _priorPosture = Postures.Standing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _priorPosition = transform.position;
        _reportMovement = new PeriodicAction(Game.TickInterval, ReportMovement, null);
        ComponentRegister.PlayerTransform = transform;
        ComponentRegister.PlayerMovement = this;
        ComponentRegister.PlayerController = Controller;
        _controllerHeight = Controller.height;
        _controllerCenter = Controller.center;
        _controllerCrouchHeight = Controller.height / 1.66f;
        _controllerCrouchCenter = new Vector3(Controller.center.x, Controller.center.y, Controller.center.z);
        _cameraLocalPosition = Camera.main.transform.localPosition;
        _cameraCrouchedPosition = new Vector3(_cameraLocalPosition.x, _cameraLocalPosition.y / 1.66f, _cameraCrouchedPosition.z);
    }
    private void ReportMovement()
    {
        
        if (_moveCheck != transform.position || _rotateCheck != transform.eulerAngles || _postureCheck != Game.PlayerPMDByte.Posture)
        {
            _moveCheck = transform.position;
            _rotateCheck = transform.eulerAngles;
            _postureCheck = Game.PlayerPMDByte.Posture;
            bool positionExceedance = MinimumReportingExceedance(transform.position, ref _priorPosition, _positionLimit);
            bool rotationExceedance = MinimumReportingExceedance(transform.eulerAngles, ref _priorRotation, _rotationLimit);
            if (positionExceedance && rotationExceedance)
            {
                byte[] prData = new byte[24];
                ByteUtils.FillArray(ref prData, 0, _priorPosition);
                ByteUtils.FillArray(ref prData, 12, _priorRotation);
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(2, _postureCheck, prData, ref _prPacketID));
            }
            else if (positionExceedance)
            {
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(0, _postureCheck, ByteUtils.Vector3ToBytes(_priorPosition), ref _prPacketID));
            }
            else
            {
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(1, _postureCheck, ByteUtils.Vector3ToBytes(_priorRotation), ref _prPacketID));
            }
        }

    }
    private bool MinimumReportingExceedance(Vector3 current, ref Vector3 prior, float limit)
    {
        float distance = Vector3.Distance(current, prior);
        if (distance > limit)
        {
            prior = current;
            return true;
        }
        return false;
    }
    public void SetPC(PC pc)
    {
        _pc = pc;
    }
    public bool IsRunning
    {
        get { return _running; }
    }
    private bool UprightMovement()
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
            Game.PlayerPMDByte.SetLocalPosture(Postures.Jump);
        }
        UpdateGroundedStatus();
        return moving;
    }
    private bool CrouchedMovement()
    {
        float forwardAcceleration = _forwardAcceleration * 0.35f;
        float lateralAcceleration = _lateralAcceleration * 0.35f;
        float maxForwardSpeed = _maxForwardSpeed * 0.35f;
        float maxLateralSpeed = _maxLateralSpeed * 0.35f;
        bool moving = MoveAlongAxes(ref _lateralSpeed, ref _forwardSpeed, maxLateralSpeed, maxForwardSpeed, lateralAcceleration, forwardAcceleration);
        _pc.RegenStamina(Time.deltaTime, false);

        if (!_grounded)
        {
            Accelerate(ref _verticalSpeed, _maxVerticalSpeed, -1.0f, gravityValue);
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        UpdateGroundedStatus();
        return moving;
    }
    private void DeadMovement()
    {
        float forwardAcceleration = _forwardAcceleration * 0.5f;
        float lateralAcceleration = _lateralAcceleration * 0.5f;
        float maxForwardSpeed = _maxForwardSpeed * 0.5f;
        float maxLateralSpeed = _maxLateralSpeed * 0.5f;
        MoveAlongAxis(ref _lateralSpeed, maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, lateralAcceleration, 1f);
        MoveAlongAxis(ref _forwardSpeed, maxForwardSpeed, Camera.main.transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, 1f);
    }

    private bool MoveAlongAxes(ref float lateralSpeed, ref float forwardSpeed, float maxLateralSpeed, float maxForwardSpeed, float lateralAcceleration, float forwardAcceleration)
    {
        float xAxisInput = MoveAlongAxis(ref _lateralSpeed, maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, lateralAcceleration, SpeedModifier);
        float zDirection = MoveAlongAxis(ref _forwardSpeed, maxForwardSpeed, transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, SpeedModifier);
        bool moving = (xAxisInput != 0) || (zDirection != 0);
        Game.PlayerPMDByte.SetMovingAndDirection(moving, zDirection > 0);
        return moving;
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
            if (InputControls.Crouch && !_csChanging)
            {
                _csChanging = true;
            }
            if (_csChanging)
            {
                CrouchStandLerp(_cameraLocalPosition, _cameraCrouchedPosition);
            }
            if (_csChanging || Game.PlayerPMDByte.IsCrouched)
            {
                _moving = CrouchedMovement();
            }
            else
            {
                _moving = UprightMovement();
            }
        }
        else
        {
            DeadMovement();
        }
        _reportMovement.ProcessAction(Time.deltaTime);
        //if(_priorMoving != _moving)
        //{
            _priorMoving = _moving;
        Game.PlayerPMDByte.SetMoving(_moving);
            //Game.SetPCAnimation(_moving, Game.PCAvatar.Posture);
        //}
    }
    private void CrouchStandLerp(Vector3 start, Vector3 end)
    {
        Vector3 a, b;
        if (Game.PlayerPMDByte.IsCrouched)
        {
            a = end;
            b = start;
        }
        else
        {
            a = start;
            b = end;
        }
        if(SharedFunctions.ProcessVector3Lerp(ref _csElapsed, _csInterval, a, b, Camera.main.transform, true, true))
        {
            _csChanging = false;
            
            if (Game.PlayerPMDByte.IsCrouched)
            {
                Game.PlayerPMDByte.SetLocalPosture(Postures.Standing);
                SetControllerHC(_controllerCenter, _controllerHeight);
            }
            else
            {
                Game.PlayerPMDByte.SetLocalPosture(Postures.Crouched); 
                SetControllerHC(_controllerCrouchCenter, _controllerCrouchHeight);
            }
            Game.SendInGameBytes(InGame_Packets.PostureChangePacket(Game.PlayerPMDByte.ToByte()));
        }
    }
    private void SetControllerHC(Vector3 center, float height)
    {
        Controller.center = center;
        Controller.height = height;
    }
    public void DeathResetCameraAndController()
    {
        Game.PlayerPMDByte.SetLocalPosture(Postures.Airborne);
        _csChanging = false;
        Camera.main.transform.localPosition = _cameraLocalPosition;
        SetControllerHC(_controllerCenter, _controllerHeight);
    }
    private void UpdateGroundedStatus()
    {
        bool priorState = _grounded;
        _grounded = isGrounded(out _hitInfo);
        if (_grounded)
        {
            if(Game.PlayerPMDByte.IsJumping)
            {
                Game.PlayerPMDByte.SetLocalPosture(Postures.Standing);
            }
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
   
    private float MoveAlongAxis(ref float speed, float maxSpeed, Vector3 directionVector, InputControl negative, InputControl positive, float acceleration, float speedModifier)
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
        Controller.Move(directionVector * speed * Time.deltaTime * speedModifier);
        return directionFactor;
    }

    private bool MovingOnMultipleAxes
    {
        get{
            return (InputControls.Forward || InputControls.Backward) && (InputControls.StrafeLeft || InputControls.StrafeRight);
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
