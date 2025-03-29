using TMPro;
using UnityEngine;

public class TranslatableTextObject : MonoBehaviour
{
    private TMP_Text _tmText;
    public int StringReference = -1;
    public bool UsePlayerTeamColor;

    private void Awake()
    {
        _tmText = GetComponent<TMP_Text>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
