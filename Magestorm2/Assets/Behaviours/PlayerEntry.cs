using TMPro;
using UnityEngine;

public class PlayerEntry : MonoBehaviour
{
    public TMP_Text PlayerNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        PlayerNameText.text = text;
    }
}
