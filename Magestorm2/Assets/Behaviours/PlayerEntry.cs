using TMPro;
using UnityEngine;

public class PlayerEntry : MonoBehaviour
{
    private TMP_Text _text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
