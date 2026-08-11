using System;
using System.Collections.Generic;
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
    private float _distanceTravelled = 0.0f;
    private float _distanceTravelledSinceLastStep = 0.0f;
    private float _positionLimit = 0.067f;
    private float _rotationLimit = 10f;
    private float _csElapsed = 0.0f;
    private float _csInterval = 0.33f;
    private float _yRotateCheck, _priorY;
    private float _controllerHeight, _controllerCrouchHeight;
    private int _prPacketID = 0;
    private Vector3 _controllerCenter, _controllerCrouchCenter, _cameraLocalPosition, _cameraCrouchedPosition;
    private Vector3 _moveCheck;
    private Vector3 _priorStep, _priorPosition;
    private Vector3 _forceDirection;
    private bool _isSlowed, _isFrozen, _isHasted, _isEntangled;
    private bool _inFlight = false;
    private bool _positionChanged = false;
    private bool _midJump = false;
    private bool _onGround = false;
    private bool _csChanging = false;
    private byte _pmdCheck;
    private RaycastHit _hitInfo;
    private PC _pc;
    private byte _insideWallCount;
    private float _forceElapsed = 0;
    private float _forceAcceleration = 0;
    private float _accelerationPeriod;
    private PMDByte _pmd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _pmd = new PMDByte();
        _priorPosition = transform.position;
        _reportMovement = new PeriodicAction(Game.MovementPolling, ReportMovement, null);
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
    public void ApplyForceVector(float forceMagnitude, float seconds, Vector3 forceDirection)
    {
        _forceAcceleration = forceMagnitude / seconds;
        _forceElapsed = 0;
        _accelerationPeriod = seconds;
        _forceDirection = forceDirection;
        if(forceDirection.y > 0)
        {
            _verticalSpeed = 0;
            _midJump = true;
        }
    }
    public void IncrementInsideWallCount()
    {
        _insideWallCount++;
    }
    public void DecrementInsideWallCount()
    {
        _insideWallCount--;
    }
    private void ReportMovement()
    {
        byte pmdByte = _pmd.ToByte();
        if (_moveCheck != transform.position || _yRotateCheck != transform.eulerAngles.y || _pmdCheck != pmdByte)
        {
            _moveCheck = transform.position;
            _yRotateCheck = transform.eulerAngles.y;
            
            bool positionExceedance = MinimumReportingExceedance(transform.position, ref _priorPosition, _positionLimit);
            bool rotationExceedance = MinimumReportingExceedance(_yRotateCheck, ref _priorY, _rotationLimit);
            if (positionExceedance && rotationExceedance)
            {
                byte[] prData = new byte[16];
                ByteUtils.FillArray(ref prData, 0, _priorPosition);
                ByteUtils.FillArray(ref prData, 12, _yRotateCheck);
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(2, prData, pmdByte, ref _prPacketID));
            }
            else if (positionExceedance)
            {
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(0, ByteUtils.Vector3ToBytes(_priorPosition), pmdByte, ref _prPacketID));
            }
            else if (rotationExceedance)
            {
                Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(1, BitConverter.GetBytes(_priorY), pmdByte, ref _prPacketID));
            }
            else if (_pmdCheck != pmdByte)
            {
                Debug.Log("PMDUpdate IsMoving: " + _pmd.IsMoving + " " + _pmd.IsMovingForward + " " + _pmd.IsMovingBackward);
                Game.SendInGameBytes(InGame_Packets.PostureChangePacket(pmdByte));
            }
            _pmdCheck = pmdByte;
        }
    }
    public void MarkSlow(bool slow)
    {
        _isSlowed = slow;
    }
    public void MarkHaste(bool haste)
    {
        _isHasted = haste;
    }
    public void MarkFrozen(bool frozen)
    {
        _isFrozen = frozen;
    }
    public void MarkEntangled(bool entangled)
    {
        _isEntangled = entangled;
        _midJump = false;
        if(_verticalSpeed > 0)
        {
            _verticalSpeed = 0;
        }
    }
    private bool MinimumReportingExceedance(float current, ref float prior, float limit)
    {
        float distance = current - prior;
        if (distance < -180)
        {
            distance += 360;
            //365
        }
        else if (distance > 180)
        {
            distance -= 360;
            //365?
        }
        if (Mathf.Abs(distance) > limit)
        {
            prior = current;
            return true;
        }
        return false;
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
    private bool UprightMovement()
    {
        float forwardAcceleration = _forwardAcceleration;
        float lateralAcceleration = _lateralAcceleration;
        float maxForwardSpeed = _maxForwardSpeed;
        float maxLateralSpeed = _maxLateralSpeed;
        
        if (Game.InputSet(InputControl.Run, Game.GameMode) && _pc.CurrentStamina > 0)
        {
            _pmd.SetRunning(true);
            forwardAcceleration *= 3;
            maxForwardSpeed *= 3;
        }
        bool moving = MoveAlongAxes(ref _lateralSpeed, ref _forwardSpeed, maxLateralSpeed, maxForwardSpeed, lateralAcceleration, forwardAcceleration);
        
        if (_pmd.IsRunning && moving)
        {
            _pc.UseStamina(Time.deltaTime * 10.0f);
        }
        if (!_pmd.IsRunning)
        {
            _pc.RegenStamina(Time.deltaTime, moving);
        }

        if (!_onGround && _forceDirection.y <= 0)// && _accelerationPeriod == 0)
        {
            Accelerate(ref _verticalSpeed, _maxVerticalSpeed, -1.0f, gravityValue);//_midJump ? gravityValue : _downwardAcceleration);
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        else if ((Game.GameInputSet(InputControl.Jump)) && !_isEntangled)
        {
            _verticalSpeed = _verticalSpeed + _jumpSpeed;
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
            _midJump = true;
            _pmd.SetLocalPosture(Postures.Jump);
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

        if (!_onGround && _forceDirection.y <=0)
        {
            Accelerate(ref _verticalSpeed, _maxVerticalSpeed, -1.0f, gravityValue);
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        UpdateGroundedStatus();
        return moving;
    }
    private void FlightMovement()
    {
        float forwardAcceleration = _forwardAcceleration;
        float lateralAcceleration = _lateralAcceleration;
        float maxForwardSpeed = _maxForwardSpeed;
        float maxLateralSpeed = _maxLateralSpeed;
        bool fastmove = false;
        if (Game.GameInputSet(InputControl.Run) && _pc.CurrentStamina > 0)
        {
            forwardAcceleration *= 2;
            maxForwardSpeed *= 2;
            fastmove = true;
        }
        if (Game.GameInputSet(InputControl.Jump))
        {
            _verticalSpeed = 1.0f;
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        else if (Game.GameInputSet(InputControl.Crouch))
        {
            _verticalSpeed = -1.0f;
            Controller.Move(transform.up * _verticalSpeed * Time.deltaTime);
        }
        float x = MoveAlongAxis(ref _lateralSpeed, maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, lateralAcceleration, 1f);
        float z = MoveAlongAxis(ref _forwardSpeed, maxForwardSpeed, Camera.main.transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, 1f);
        bool moving = x != 0 || z != 0;
        if (moving)
        {
            if (fastmove)
            {
                _pc.UseStamina(Time.deltaTime * 12.5f);
            }
            else
            {
                _pc.RegenStamina(Time.deltaTime * 0.1f, false);
            }
        }
        else
        {
            _pc.RegenStamina(Time.deltaTime * 0.5f, false);
        }
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
    public void MarkInFlight(bool inFlight)
    {
        _inFlight = inFlight;
        _pmd.SetLocalPosture(inFlight?Postures.Airborne:Postures.Standing);
    }
    private bool MoveAlongAxes(ref float lateralSpeed, ref float forwardSpeed, float maxLateralSpeed, float maxForwardSpeed, float lateralAcceleration, float forwardAcceleration)
    {
        float xAxisInput = MoveAlongAxis(ref lateralSpeed, maxLateralSpeed, transform.right, InputControl.StrafeLeft, InputControl.StrafeRight, lateralAcceleration, SpeedModifier);
        float zDirection = MoveAlongAxis(ref forwardSpeed, maxForwardSpeed, transform.forward, InputControl.Backward, InputControl.Forward, forwardAcceleration, SpeedModifier);
        bool moving = Mathf.Abs(lateralSpeed) >= 0.2f || Mathf.Abs(forwardSpeed) >= 0.2f;
        _pmd.SetMovingAndDirection(moving, zDirection > 0);
        return moving;
    }
    private void HandleForceAcceleration()
    {
        if(_accelerationPeriod > 0)
        {
            float delta = Time.deltaTime;
            if(_forceElapsed + delta > _accelerationPeriod)
            {
                delta = _accelerationPeriod - _forceElapsed;
            }
            _forceElapsed += delta;
            Controller.Move(_forceDirection * delta * (_forceAcceleration - _forceAcceleration*(_forceElapsed / _accelerationPeriod)));
            if(_accelerationPeriod - _forceElapsed == 0)
            {
                _accelerationPeriod = 0;
                _forceDirection = Vector3.zero;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!MatchParams.JoinedMatch)
        {
            return;
        }
        _pmd.SetRunning(false);
        HandleForceAcceleration();
        if (Game.PCAvatar.IsAlive)
        {
            if (_inFlight)
            {
                FlightMovement();
            }
            else
            {
                if (Game.GameInputSet(InputControl.Crouch) && !_csChanging)
                {
                    _csChanging = true;
                }
                if (_csChanging)
                {
                    CrouchStandLerp(_cameraLocalPosition, _cameraCrouchedPosition);
                }
                if (_csChanging || _pmd.IsCrouched)
                {
                    CrouchedMovement();
                }
                else
                {
                    UprightMovement();
                }
            }
        }
        else
        {
            DeadMovement();
        }
        _reportMovement.ProcessAction(Time.deltaTime);
    }
    void Update()
    {
        if (!MatchParams.JoinedMatch)
        {
            return;
        }
    }
    private void CrouchStandLerp(Vector3 start, Vector3 end)
    {
        Vector3 a, b;
        if (_pmd.IsCrouched)
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
            
            if (_pmd.IsCrouched)
            {
                _pmd.SetLocalPosture(Postures.Standing);
                SetControllerHC(_controllerCenter, _controllerHeight);
            }
            else
            {
                _pmd.SetLocalPosture(Postures.Crouched); 
                SetControllerHC(_controllerCrouchCenter, _controllerCrouchHeight);
            }
            Game.SendInGameBytes(InGame_Packets.PostureChangePacket(_pmd.ToByte()));
        }
    }
    private void SetControllerHC(Vector3 center, float height)
    {
        Controller.center = center;
        Controller.height = height;
    }
    public void DeathResetCameraAndController()
    {
        _pmd.SetLocalPosture(Postures.Airborne);
        _csChanging = false;
        Camera.main.transform.localPosition = _cameraLocalPosition;
        SetControllerHC(_controllerCenter, _controllerHeight);
    }
    private void UpdateGroundedStatus()
    {
        bool priorState = _onGround;
        _onGround = isOnGround(out _hitInfo);
        if (_onGround)
        {
            if(_pmd.IsJumping && !_isEntangled)
            {
                _pmd.SetLocalPosture(Postures.Standing);
            }
            _midJump = false;
            _verticalSpeed = 0.0f;
            _distanceTravelled += Vector3.Distance(transform.position, _priorStep);
            _priorStep = transform.position;
            PlayStepSound();
            
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
                    if (_pmd.IsRunning)
                    {
                        ComponentRegister.PC.PlaySFX(standingOn.FootstepClip);
                    }
                }
            }
            
        }
    }

    private bool isOnGround(out RaycastHit hitInfo)
    {
        return _pc.DownwardCaster.CastForward(LayerManager.FloorMask, 0.1f, out hitInfo);
    }
    private void Accelerate(ref float speed, float maxSpeed, float directionFactor, float acceleration)
    {
        speed += Time.deltaTime * directionFactor * acceleration;
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        else if (speed < -maxSpeed)
        {
            speed = -maxSpeed;
        }
    }
   
    private float MoveAlongAxis(ref float speed, float maxSpeed, Vector3 directionVector, InputControl negative, InputControl positive, float acceleration, float speedModifier)
    {
        bool clamp = false;
        float directionFactor = 0.0f;
        if (_midJump)
        {
            directionFactor = 0; // maintain the previous speed.
        }
        else if ((!Game.GameInputSet(negative) && !Game.GameInputSet(positive)) ||
            Game.GameInputSet(negative) && Game.GameInputSet(positive))
        {
            directionFactor = speed > 0.0f ? -1.0f : 1.0f;
            clamp = true;
        }
        else if (Game.GameInputSet(negative) || Game.GameInputSet(positive))
        {
            directionFactor = Game.GameInputSet(negative) ? -1.0f : 1.0f;
        }
        Accelerate(ref speed, maxSpeed, directionFactor, acceleration);
        if (clamp)
        {
            if((directionFactor > 0 && speed > 0) || (directionFactor < 0 && speed < 0))
            {
                speed = 0;
            }
        }
        if(speed != 0)
        {
            Vector3 movementVector = directionVector * speed * Time.deltaTime * speedModifier;
            if (movementVector.magnitude >= 0.001)
            {
                Controller.Move(movementVector);
            }
        }
        return directionFactor;
    }

    private bool MovingOnMultipleAxes
    {
        get{
            return (Game.GameInputSet(InputControl.Forward) || Game.GameInputSet(InputControl.Backward)) && 
                (Game.GameInputSet(InputControl.StrafeLeft) || Game.GameInputSet(InputControl.StrafeRight));
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
            if (_isSlowed)
            {
                toReturn *= 0.75f;
            }
            if (_isFrozen)
            {
                toReturn *= 0.75f;
            }
            if (_isHasted)
            {
                toReturn *= 1.25f;
            }
            if(_insideWallCount > 0)
            {
                toReturn *= (float)(Math.Pow(0.75, _insideWallCount));
            }
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

    public PMDByte PMD
    {
        get
        {
            return _pmd;
        }
    }
}
