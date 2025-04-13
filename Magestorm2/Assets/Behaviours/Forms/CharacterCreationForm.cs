using TMPro;
using UnityEngine;

public class CharacterCreationForm : ValidatableForm
{
    public TMP_Dropdown ClassDropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        Debug.Log(ClassDropdown.itemText.text);
    }
}
