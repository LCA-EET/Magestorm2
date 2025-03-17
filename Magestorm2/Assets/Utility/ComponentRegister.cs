using UnityEngine;

public static class ComponentRegister
{
    private static Transform _playerTransform;
    private static PlayerMovement _playerMovement;
    private static CharacterController _playerController;
    private static Camera _mainCamera;
    public static Transform PlayerTransform
    {
        get
        {
            return _playerTransform;
        }
        set
        {
            _playerTransform = value;
        }
    }
    public static PlayerMovement PlayerMovement
    {
        get
        {
            return _playerMovement;
        }
        set
        {
            _playerMovement = value;
        }
    }

    public static CharacterController PlayerController
    {
        get
        {
            return _playerController;
        }
        set
        {
            _playerController = value;
        }
    }

    public static Camera MainCamera
    {
        get
        {
            return _mainCamera;
        }
        set
        {
            _mainCamera = value;
        }
    }
}
