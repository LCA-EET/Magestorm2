using UnityEngine;
public class MainCamera : MonoBehaviour 
{
    public Camera Camera, MinimapCamera;

    public void Start()
    {
        if(ComponentRegister.PC.CharacterClass == PlayerClass.Cleric)
        {
            Debug.Log("Changing culling masks");
            Camera.cullingMask |= LayerManager.DeadPlayerLayer;
            MinimapCamera.cullingMask |= LayerManager.DeadPlayerLayer;
        }
    }
}
