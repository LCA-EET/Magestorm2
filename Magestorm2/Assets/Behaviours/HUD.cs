using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject componentContainer;
    
    private bool _hudActive;
    void Start()
    {
        ComponentRegister.HUD = this;
        _hudActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.HUD)
        {
            _hudActive = !_hudActive;
            componentContainer.SetActive(_hudActive);
        }
    }
}
