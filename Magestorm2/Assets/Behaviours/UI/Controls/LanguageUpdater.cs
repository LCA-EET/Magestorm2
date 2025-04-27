using TMPro;
using UnityEngine;

public class LanguageUpdater : MonoBehaviour
{
    public TMP_Text[] TextFields;
    public int[] StringReferences;
    private void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Language.RegisterLanguageUpdater(this);
        UpdateLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void UpdateLanguage()
    {
        for (int i = 0; i < TextFields.Length; i++)
        {
            TextFields[i].text = Language.GetBaseString(StringReferences[i]);
        }
    }
}
