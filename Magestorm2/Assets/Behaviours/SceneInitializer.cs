using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Teams.Init();
        Game.Init();
        Language.Init();
        LayerManager.Init();
        InputControls.Init();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
