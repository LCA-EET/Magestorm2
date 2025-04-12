using UnityEngine;

public class CharacterCard : MonoBehaviour
{
    public GameObject NewPanel;
    public GameObject ExistingPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateNewPanel()
    {
        NewPanel.SetActive(true);
        ExistingPanel.SetActive(false);
    }
    public void ActivateExistingPanel()
    {
        NewPanel.SetActive(false);
        ExistingPanel.SetActive(true);
    }
}
