using TMPro;
using UnityEngine;

public class AvatarStatus : MonoBehaviour
{
    private bool _isShown = true;
    public GameObject DeathIcon; 
    public TMP_Text Name;
    public TMP_Text PlayerClass;
    public TMP_Text Level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateStatus(Avatar updated)
    { 
        DeathIcon.SetActive(!updated.IsAlive);
        Name.text = updated.Name;
        Level.text = updated.Level.ToString();
        PlayerClass.text = updated.PlayerClassString;
        Show(true);
    }
    private void Show(bool showStatus)
    {
        Name.gameObject.SetActive(showStatus);
        Level.gameObject.SetActive(showStatus);
        PlayerClass.gameObject.SetActive(showStatus);  
        _isShown = showStatus;
    }
    public void Deactivate()
    {
        Show(false);
        DeathIcon.SetActive(false);
    }
    public void SetID(byte playerID)
    {

    }
}
