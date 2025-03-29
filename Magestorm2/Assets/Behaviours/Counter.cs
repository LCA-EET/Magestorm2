using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private TMP_Text _tmText;
    public int StringReference;
    void Awake()
    {
        _tmText = GetComponent<TMP_Text>();
        Language.Init();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCount(0);                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetCount(byte count)
    {
        _tmText.text = Language.BuildString(StringReference, 0);
    }
}
