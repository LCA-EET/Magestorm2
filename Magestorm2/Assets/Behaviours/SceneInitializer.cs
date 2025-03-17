using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LayerManager.Init();
        InputControls.Init();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
