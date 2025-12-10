using TMPro;
using UnityEngine;

public class Counter : LanguageUpdater
{
    private byte _lastCount = 0;
    void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Teams.SetTextColor(TextFields[0]);
        SetCount(0);                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void UpdateLanguage()
    {
        SetCount(_lastCount);
    }
    public void SetCount(byte count)
    {
        _lastCount = count;
        TextFields[0].text = Language.BuildString(StringReferences[0], count);
    }


}
