using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class MatchEntry : ValidatableForm
{
    public TMP_Text ID, Arena, Creator, TimeLeft;
    public Image Background;
    public Button SelectButton;
    private UIMatchList _owningList;
    private bool _selected;
    public bool Selectable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Background.color = Colors.EntryUnselected;
        if (Selectable)
        {
            SelectButton.onClick.AddListener(NotifyOwner);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void NotifyOwner()
    {
        _owningList.EntrySelected(this);
    }
    public void MarkSelected(bool selected)
    {
        _selected = selected;
        Background.color = Colors.ApplyMatchSelectionColor(_selected);
    }
    public void SetOwningList(UIMatchList owningList)
    {
        _owningList = owningList;
    }
    public bool IsSelected
    {
        get { return _selected; }
    }
}
