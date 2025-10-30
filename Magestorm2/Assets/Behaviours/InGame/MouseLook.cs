using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Camera mainCamera;
    public float mouseSensitivity = 300f; //You can change the number any numbers you want, but always put f after.
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ComponentRegister.MainCamera = mainCamera;
    }

    void Update()
    {
        if (Game.GameMode)
        {
            if (ComponentRegister.PlayerMovement.PositionChanged)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 1.5f + Mathf.Sin(ComponentRegister.PlayerMovement.DistanceTravelled) * 0.1f, transform.localPosition.z);
            }

            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            ComponentRegister.PlayerTransform.Rotate(Vector3.up * mouseX);
        }
        
    }

}