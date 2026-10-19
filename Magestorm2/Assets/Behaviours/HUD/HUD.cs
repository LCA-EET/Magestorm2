using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject componentContainer, pwPanel;
    public DamagePanel DamagePanel;
    private bool _hudActive;
    void Start()
    {
        ComponentRegister.HUD = this;
        ComponentRegister.DamagePanel = DamagePanel;
        _hudActive = true;
    }

    public void HidePleaseWait()
    {
        pwPanel.SetActive(false);
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
